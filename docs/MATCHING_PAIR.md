## Matching Pair Game – Design & Flow

This document explains how the Matching Pair game is wired into the app, how the board is generated, and how the gameplay loop works.

### High-Level Overview

- **Feature entrypoint**: `MatchingPairController` — `MonoBehaviour` + `IGameController`, lives on the feature's lobby prefab. Auto-discovered by `GameScreenController` when the main catalog spawns the lobby.
- **Lobby view**: `MatchingPairLobbyView` extends `GameLobbyView`, shows difficulty cards (5 pieces, 6 pieces, …) driven by data it receives at runtime — it owns no design-time data.
- **Data layer** (Scriptable-Object backed, resolved through DI):
  - `MatchingPairDefinition` — one difficulty variant (pieceCount, title, icon, card prefab, game prefab).
  - `MatchingPairCatalogConfig` — SO bundle of all variants.
  - `IMatchingPairCatalog` — runtime lookup (`GetVariants`, `TryGetVariant(pieceCount, …)`).
  - `MatchingPairModule` — registers the catalog at bootstrap.
- **Game board**: Each difficulty's `gamePrefab` (`5 Pieces`, `6 Pieces`, …) has:
  - Several child `Block` instances (using the shared `Block.prefab`).
  - A root `MatchingPairBoardView` component that controls that board.
- **Block view**: Each `Block` has a `MatchingPairBlock` component that:
  - Holds its `pairId`, sprites and state.
  - Plays the flip animation and fade-out.
  - Raises `Clicked` to the board.

At a high level, the flow is:

1. App bootstrap registers the `MatchingPairCatalogConfig` (held inside `AppConfig`) and the `MatchingPairModule` builds an `IMatchingPairCatalog` from it.
2. User opens the Matching Pair card in the main game catalog → `GameScreenController` instantiates the Matching Pair lobby prefab.
3. `MatchingPairController.Awake` resolves `IMatchingPairCatalog` from `AppBootstrap.Services` and pushes its variants to `MatchingPairLobbyView.SetVariants(...)`.
4. View spawns one `MatchingPairCardItem` per variant (using each variant's `cardPrefab`) under the scroll content.
5. Player selects a mode → controller looks up the variant, instantiates its `gamePrefab`, locates the `MatchingPairBoardView` and calls `StartGame()`.
6. Board completes → `GameWon` fires → controller cleans up / returns to lobby.

### Data Layer & DI Wiring

This feature follows the project-wide pattern: **config asset → DI → subsystem → controller → view**. The view never owns design-time data.

#### 1. `MatchingPairDefinition` (ScriptableObject)

```
Assets > Create > PuzzleApp > Matching Pair Definition
```
Fields: `pieceCount`, `title`, `icon`, `cardPrefab` (lobby card), `gamePrefab` (`N Pieces.prefab`).

#### 2. `MatchingPairCatalogConfig` (ScriptableObject)

```
Assets > Create > PuzzleApp > Matching Pair Catalog Config
```
Holds a single array `MatchingPairDefinition[] variants`. This asset is referenced from `AppConfig.matchingPairCatalog`.

#### 3. `IMatchingPairCatalog` (runtime subsystem)

```csharp
public interface IMatchingPairCatalog
{
    IReadOnlyList<MatchingPairDefinition> GetVariants();
    bool TryGetVariant(int pieceCount, out MatchingPairDefinition definition);
}
```

The implementation builds a `Dictionary<int, MatchingPairDefinition>` keyed by `pieceCount` for O(1) lookup.

#### 4. `MatchingPairModule` (IAppModule)

`Register` binds `IMatchingPairCatalog` as a singleton factory that resolves `MatchingPairCatalogConfig` and hands its `variants` to the catalog ctor. `Initialize` force-resolves it so construction (and its diagnostic logs) happen deterministically during bootstrap.

#### 5. Bootstrap flow

- `AppBootstrap` holds a single `[SerializeField] AppConfig _appConfig`.
- It fans out `_appConfig.matchingPairCatalog` into the registry as `RegisterInstance<MatchingPairCatalogConfig>(…)`.
- The `MatchingPairModule` is added after `GameCatalogModule` in the module array so the catalog is available before any controller tries to resolve it.

### Controller / View Contract

```
MatchingPairController         MatchingPairLobbyView (pure view)
─────────────────────────     ────────────────────────────────
Awake:                          SetVariants(variants) -> spawn card items
  resolve IMatchingPairCatalog  SetScrollViewActive(bool)
  view.SetVariants(variants)    event CardClicked(int pieceCount)
  sub CardClicked, BackClicked   event BackClicked   (inherited)
OnCardClicked(pieceCount):
  catalog.TryGetVariant
  Instantiate variant.gamePrefab
  locate MatchingPairBoardView
  board.StartGame()
  view.SetScrollViewActive(false)
OnBackClicked:
  if active game  -> destroy + show scroll
  else            -> CloseRequested?.Invoke()
OnGameWon:
  cleanup hooks
```

The controller resolves the catalog via the static service locator (`AppBootstrap.Services?.Resolve<…>()`) because it lives on a prefab instantiated at runtime — `[SerializeField]` constructor injection is not available for that.

`MatchingPairLobbyView` exposes **no design-time data slots** for variants. It only owns UI-layout fields (`_scrollContent`, `_scrollView`). All content is pushed in via `SetVariants`.

### Board Generation

#### 1. Blocks & sprites

- Each `N Pieces.prefab` contains exactly **N** children with `MatchingPairBlock`.
- `MatchingPairBoardView` discovers the blocks:
  - If `_blocksExplicit` array is set in the Inspector, its order defines slot order.
  - Otherwise, it calls `GetComponentsInChildren<MatchingPairBlock>(true)` and uses hierarchy order.
- The board has:
  - `_backSprite`: shared back art.
  - `_frontSprites[0..11]`: up to 12 different front sprites.
  - A runtime dictionary `_spriteById: int → Sprite` where:
    - ID `1` uses `_frontSprites[0]`, `2` uses `_frontSprites[1]`, …, up to `12`.

#### 2. Layout source – `mock_data.json`

- A JSON file `Assets/Resources/mock_data.json` contains many pre-generated layouts:
  - Shape:
    ```json
    {
      "entries": [
        { "cards": [ 11, 8, 3, 5, 10, 9, 10, 4, 11, 9, 5, 8, 2, 4, 3, 2 ] },
        { "cards": [ 11, 10, 1, 10, 1, 11 ] }
      ]
    }
    ```
  - `cards` is an array of integers in the range `1..12`, one per block on the board.
- On startup, `MatchingPairBoardView.Awake()`:
  - Builds `_spriteById` from `_frontSprites`.
  - Loads `mock_data` via `Resources.Load<TextAsset>` and deserializes it into `MockData.entries[]`.

#### 3. Picking a layout for a given mode

When `StartGame()` is called:

1. The board counts how many blocks it has (`total`).
2. It filters all mock entries whose `cards.Length == total`.
3. It randomly selects one of those candidates.
4. That `cards` array becomes the **layout** for this round — `cards[i]` (1..12) is the card number for block at slot `i`.

If there is no layout with the required length (should not happen with the generated data), the board falls back to the old in-code random generation as a safety net.

### Pair Logic & Deception Card

Given the chosen `cards` array:

- **Real pairs**: any number that appears at least twice — blocks with that number share the same `pairId` (0, 1, 2, …).
- **Deception card (odd sizes)**: when the total block count is odd, one number appears only once. That single card gets a unique `pairId = pairCount` and uses a normal front sprite, but it can never match.
- **Win condition**:
  - `pairCount = total / 2` (integer division).
  - `_pairsToMatch = pairCount`.
  - Each matched real pair increments `_pairsMatched`; when `_pairsMatched >= _pairsToMatch`, `GameWon` fires.
  - In odd modes, only the deception card is left standing.

### Block Behaviour & Animation

Each `MatchingPairBlock`:

- Holds: `PairId`, `BlockIndex`, `_frontSprite`, `_backSprite`, `_faceUp`, `_turning`, `_resolved`.
- Is wired in the prefab with `_image` (card Image) and `_button` (card Button).
- `Setup(index, pairId, front, back)` sets sprites, resets state, starts at rotation Y = 0 showing the back.

**Click flow:**

1. User taps a block's button.
2. `MatchingPairBlock` raises `Clicked(this)` if it's not already face-up, turning, or resolved.
3. `MatchingPairBoardView.OnBlockClicked`:
   - Ignores clicks while `_inputLocked` or if the block is resolved/face-up.
   - Calls `Reveal()` on the block.
   - First selection is stored; on the second it locks input and runs `EvaluateMatch(first, second)`.

**Animation style (flip):**

- `Reveal()` / `HideCard()` rotate the card 0→90 (edge-on), swap the sprite, then 90→180 (or back to 0).
- `Resolve()` tweens alpha to zero then deactivates the GameObject.

### Match Evaluation

`EvaluateMatch(a, b)` coroutine:

1. Waits for `_mismatchDelay`.
2. Compares `a.PairId` and `b.PairId`:
   - **Equal** → `Resolve()` both, increment `_pairsMatched`, raise `PairMatched(pairsMatched, pairsToMatch)`. If all real pairs matched, set `_gameActive = false` and raise `GameWon`.
   - **Different** → `HideCard()` both.
3. Short extra delay, then unlocks input.

### Integration with the Rest of the App

- `AppBootstrap` registers `MatchingPairCatalogConfig` and adds `MatchingPairModule` to the module list.
- `MatchingPairController`:
  - Resolves `IMatchingPairCatalog` from `AppBootstrap.Services` in `Awake`.
  - Pushes variants into `MatchingPairLobbyView.SetVariants(...)`.
  - Subscribes to `CardClicked(pieceCount)` → looks up the variant, spawns its `gamePrefab` under `_gameParent`, locates `MatchingPairBoardView`, calls `StartGame()`, hides the scroll view.
  - Listens to `GameWon` from the board and handles cleanup / return to lobby.
- `MatchingPairLobbyView` is a pure UI view: it knows how to render a list of `MatchingPairDefinition` and emit click events, nothing more.
- This follows the same pattern as MatchObjects and other features: Controller coordinates services and views; Views are dumb UI that expose events; no scene loading.

### Prefab Wiring Checklist

The `MatchingPairLobby` prefab should have:

- `MatchingPairController` on the root (not just the view — we hit this bug before).
  - `_lobbyView` → child with `MatchingPairLobbyView`.
  - `_gameParent` → a **dedicated sibling** GameRoot (not the Scroll View's `Content`, otherwise hiding the scroll also hides the spawned game).
- `MatchingPairLobbyView` on the scroll view root with:
  - `_scrollContent` → Scroll View's `Content` RectTransform.
  - `_scrollView` → the Scroll View GameObject (to toggle visibility).

### Notes for Extending

- **Adding a new difficulty**:
  1. Create a new `MatchingPairDefinition` asset (Assets > Create > PuzzleApp > Matching Pair Definition).
  2. Set `pieceCount`, card prefab (with `MatchingPairCardItem`), and game prefab (with N `MatchingPairBlock` children + `MatchingPairBoardView`).
  3. Add it to `MatchingPairCatalogConfig.variants`.
  4. If the new `pieceCount` is not already in `mock_data.json`, regenerate or extend the JSON so there are layouts for that size.

- **Adding new sprites**:
  - Extend `_frontSprites` in the Inspector (first 12 entries map to IDs 1–12).
  - Regenerate `mock_data.json` if using IDs beyond 12 and adjust the dictionary logic.

- **Telemetry / UX tweaks**:
  - Use `PairMatched` for score / move counters / progress bars.
  - Use `GameWon` for celebration animations, confetti, or navigation to a results screen.
