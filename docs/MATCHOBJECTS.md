# Match Objects Game — Design & Flow

Drag-to-match puzzle with a **level-based progression** (80 levels by default). Each level shows a shuffled subset of the full pair pool; completed levels are persisted locally.

---

## High-Level Flow

```
Main Game Catalog (GameScreenCardsView)
  ↓  [user taps Match Objects card]
MatchObjectsLobby prefab (MatchObjectsController + MatchObjectsLobbyView)
  ↓  [SetLevels(totalLevels, isCompleted) on Awake]
Scroll view with N level cards (MatchObjectsLevelItem) — each shows completed/incomplete visual
  ↓  [user taps level X]
MatchObjectsLevelService.GetPairsForLevel(X) → deterministic pair subset
  ↓
MatchObjectsBoardView.StartGame(pairs, rootCanvas)
  ├─ Left column: 5 clue items (leftSprite)
  ├─ Right column: 5 drop zones (expect a specific pairId)
  └─ Bottom bar: 5 draggable items (rightSprite, shuffled)
  ↓  [all pairs matched]
GameWon event → Controller marks level completed, refreshes the lobby card, destroys board, re-shows scroll
```

---

## Architecture Summary

The feature follows the project-wide pattern: **config asset → DI → subsystem → controller → view**. No new signals or modules outside the feature's own `MatchObjectsModule`. The controller is auto-discovered by `GameScreenController` because it implements `IGameController`.

### Layers

| Layer | Namespace | Files |
|-------|-----------|-------|
| Data (SO + POCO) | `PuzzleApp.Features.MatchObjects` | `MatchObjectsItemPair`, `MatchObjectsLevelConfig` |
| Subsystems (DI) | `PuzzleApp.Features.MatchObjects` | `IMatchObjectsLevelService`, `IMatchObjectsProgress`, `MatchObjectsModule` |
| UI (lobby/cards) | `PuzzleApp.UI` | `MatchObjectsLevelItem`, `MatchObjectsLobbyView`, `MatchObjectsController` |
| Game (board) | `PuzzleApp.MatchObjects` | `MatchObjectsBoardView`, `MatchObjectsClueItem`, `MatchObjectsDropZone`, `MatchObjectsDraggableItem` |

---

## Data Layer

### `MatchObjectsItemPair` (serializable POCO)

```csharp
[Serializable]
public class MatchObjectsItemPair
{
    public string pairId;       // unique id — matches draggable with drop zone
    public Sprite leftSprite;   // clue (shown on left column)
    public Sprite rightSprite;  // answer (draggable on bottom bar)
    public string label;        // optional
}
```

### `MatchObjectsLevelConfig` (ScriptableObject)

```
Assets > Create > PuzzleApp > Match Objects Level Config
```

```csharp
[CreateAssetMenu(...)]
public class MatchObjectsLevelConfig : ScriptableObject
{
    [Min(1)] public int totalLevels = 80;
    [Min(1)] public int pairsPerLevel = 5;
    public MatchObjectsItemPair[] pairPool;   // full pool of available pairs
}
```

Referenced from `AppConfig.matchObjectsLevels`.

---

## Subsystems (resolved via DI)

### `IMatchObjectsLevelService`

```csharp
public interface IMatchObjectsLevelService
{
    int TotalLevels { get; }
    MatchObjectsItemPair[] GetPairsForLevel(int levelIndex);
}
```

- Picks `pairsPerLevel` pairs from the pool using a **Fisher-Yates shuffle seeded with `levelIndex`**.
- Seeded → **deterministic**: level 3 always produces the same set across sessions and installs.
- Returns at most `pairPool.Length` pairs when the pool is smaller than `pairsPerLevel`.

### `IMatchObjectsProgress`

```csharp
public interface IMatchObjectsProgress
{
    event Action<int> LevelCompleted;
    bool IsCompleted(int levelIndex);
    void MarkCompleted(int levelIndex);
}
```

- Implementation backs completion state with a `HashSet<int>` serialized to `PlayerPrefs` (`"MatchObjects.CompletedLevels"`) as a comma-separated list.
- `MarkCompleted` is idempotent (returns early if the level is already marked) and only raises `LevelCompleted` on the first mark.

### `MatchObjectsModule`

Registers three singletons at bootstrap:

```csharp
services.RegisterSingleton<IMatchObjectsDataService>(r =>
    new MatchObjectsDataService(r.Resolve<IGameDataProvider>()));

services.RegisterSingleton<IMatchObjectsLevelService>(r =>
    new MatchObjectsLevelService(r.Resolve<MatchObjectsLevelConfig>()));

services.RegisterSingleton<IMatchObjectsProgress>(r => new MatchObjectsProgress());
```

`Initialize` force-resolves all three so construction happens at bootstrap (not lazily on first level tap).

### Bootstrap wiring

- `AppConfig.matchObjectsLevels` holds the level config SO (alongside `gameCatalog` and `matchingPairCatalog`).
- `AppBootstrap` validates the slot and does `services.RegisterInstance<MatchObjectsLevelConfig>(_appConfig.matchObjectsLevels)`.
- The `MatchObjectsModule` is the last entry in the module array so anything it depends on (e.g. `IGameDataProvider`) is already registered.

---

## Lobby Layer (`Assets/Core/Scripts/UI/`)

### `MatchObjectsLevelItem`

One card per level in the scroll view.

```csharp
[SerializeField] Button _button;
[SerializeField] GameObject _completedImage;   // green/checked state
[SerializeField] GameObject _incompleteImage;  // default state
[SerializeField] TextMeshProUGUI _label;
public event Action<int> Clicked;
public void Bind(int levelIndex, bool isCompleted);
public void SetCompleted(bool isCompleted);
```

- `Bind` sets the label to `(levelIndex + 1).ToString()` (1-based display) and toggles the two image children.
- Button listener is wired in `OnEnable` / removed in `OnDisable` to keep the callback surface small.

### `MatchObjectsLobbyView` (extends `GameLobbyView`)

Pure view — owns no design-time data, only UI layout slots.

```csharp
[SerializeField] RectTransform _scrollContent;
[SerializeField] GameObject _scrollView;
[SerializeField] MatchObjectsLevelItem _levelItemPrefab;
public event Action<int> LevelSelected;

public void SetLevels(int totalLevels, Func<int, bool> isCompleted);
public void RefreshCompletion(int levelIndex, bool isCompleted);
public void SetScrollViewActive(bool active);
public void Clear();
```

- `SetLevels` instantiates one `MatchObjectsLevelItem` per level under `_scrollContent`, binds with current completion state, and subscribes to `Clicked`.
- `RefreshCompletion` flips the completed/incomplete visual for a single card after a win (no full rebuild).
- `SetScrollViewActive(false)` hides the lobby while the game is running; `(true)` restores it when the player wins or backs out.
- Re-emits `LevelSelected(index)` upward to the controller.

### `MatchObjectsController` (`MonoBehaviour, IGameController`)

Lives on the lobby prefab root. Auto-discovered by `GameScreenController`.

```csharp
[SerializeField] MatchObjectsLobbyView _lobbyView;
[SerializeField] Transform _gameParent;
[SerializeField] Canvas _rootCanvas;
[SerializeField] MatchObjectsBoardView _boardPrefab;

IMatchObjectsLevelService _levelService;
IMatchObjectsProgress _progress;
MatchObjectsBoardView _activeBoard;
int _activeLevelIndex = -1;
public event Action CloseRequested;
```

**Awake:**
1. Resolve `IMatchObjectsLevelService` and `IMatchObjectsProgress` from `AppBootstrap.Services`.
2. Push data to view: `_lobbyView.SetLevels(_levelService.TotalLevels, _progress.IsCompleted)`.

**OnEnable/OnDisable:** sub/unsub from view's `BackClicked` + `LevelSelected`.

**OnLevelSelected(int levelIndex):**
1. `_levelService.GetPairsForLevel(levelIndex)` → pair array.
2. Instantiate `_boardPrefab` under `_gameParent` (falls back to own transform).
3. `_lobbyView.SetScrollViewActive(false)`.
4. Subscribe to board's `GameWon` and call `StartGame(pairs, rootCanvas ?? GetComponentInParent<Canvas>())`.

**OnGameWon:**
1. `_progress.MarkCompleted(_activeLevelIndex)` — persists to `PlayerPrefs`.
2. `_lobbyView.RefreshCompletion(_activeLevelIndex, true)` — flips the tile visual immediately.
3. Destroy board, re-show scroll view.

**OnBackClicked:**
- If a board is active → destroy it, show the scroll again (same "back" button doubles as "return to level list").
- Otherwise → `CloseRequested?.Invoke()` (tells `GameScreenController` to close the whole feature).

The controller uses the **static service locator** (`AppBootstrap.Services?.Resolve<…>()`) because it's a MonoBehaviour on a runtime-instantiated prefab — constructor injection isn't available.

---

## Game Layer (`Assets/Match Objects/Script/`, namespace `PuzzleApp.MatchObjects`)

### `MatchObjectsBoardView`

```csharp
public void StartGame(MatchObjectsItemPair[] pairs, Canvas rootCanvas)
```

1. Shuffle the pair list.
2. `SpawnClueItems` → one `MatchObjectsClueItem` per pair under `_leftColumn`.
3. `SpawnDropZones` → one `MatchObjectsDropZone` per pair under `_rightColumn`, each capturing a callback that calls `OnPairMatched`.
4. **Shuffle again** so the bottom bar order is independent of the left/right order.
5. `SpawnDraggableItems` → one `MatchObjectsDraggableItem` per pair under `_bottomBar`, each given the full drop-zone array and the `rootCanvas`.

Win: every correct drop increments `_solvedCount`; when `_solvedCount >= _totalPairs` it fires `GameWon`.

### `MatchObjectsClueItem`
Static image on the left column. `Bind(pairId, sprite)` only.

### `MatchObjectsDropZone`
Empty slot on the right. Tracks `ExpectedPairId`, `IsOccupied`, and an `onCorrectDrop` callback. Exposes:
- `ContainsScreenPoint(pos, cam)` — hit-test against its RectTransform.
- `Accept(item)` — reparent, center, lock, show green badge, invoke callback.

### `MatchObjectsDraggableItem` (`IPointerDownHandler, IDragHandler, IPointerUpHandler`)

- **Pointer Down**: record original parent and anchored position, reparent to `rootCanvas.transform` so drag renders above everything, set alpha to 0.7.
- **Drag**: `RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.transform, ...)` → set `anchoredPosition`.
- **Pointer Up**: iterate zones, find first `ContainsScreenPoint` hit. If `zone.ExpectedPairId == PairId`, `zone.Accept(this)` and lock. Otherwise run `ReturnToOrigin()` (lerp back, reparent).

---

## Prefab Wiring Checklist

**`MatchObjectsLobby` prefab** (shown when the Match Objects card is tapped):
- `MatchObjectsController` on the root.
  - `_lobbyView` → child with `MatchObjectsLobbyView`.
  - `_gameParent` → dedicated sibling `GameRoot` (NOT the Scroll View's `Content` — that'll be hidden with the scroll).
  - `_rootCanvas` → leave empty if there's only one HUD canvas; the fallback `GetComponentInParent<Canvas>()` picks it up.
  - `_boardPrefab` → the `MatchObjectsBoard` prefab.
- `MatchObjectsLobbyView` on the scroll view root.
  - `_scrollContent` → Scroll View's `Content` RectTransform.
  - `_scrollView` → the Scroll View GameObject (toggles with the active board).
  - `_levelItemPrefab` → the `MatchObjectsLevelItem` prefab.

**`MatchObjectsLevelItem` prefab**: Button + two child GameObjects (`_completedImage`, `_incompleteImage`) + optional `TextMeshProUGUI` label.

**`MatchObjectsBoard` prefab**: `MatchObjectsBoardView` + three empty Transforms (`_leftColumn`, `_rightColumn`, `_bottomBar`) + prefab refs for clue/dropzone/draggable.

**Clue / Drop-zone / Draggable prefabs**: see their respective components — each is a single script with a few Image / CanvasGroup slots.

### Level config asset

```
Assets > Create > PuzzleApp > Match Objects Level Config
```

Set `totalLevels = 80`, `pairsPerLevel = 5`, fill `pairPool` with your full list of `MatchObjectsItemPair` entries (the more, the better, since each level picks a random subset). Assign the asset to `AppConfig.matchObjectsLevels`.

### Catalog registration

On `GameScreenCardsView`, add a `GameDefinition` entry for Match Objects pointing at the `MatchObjectsLobby` prefab as its `lobbyPrefab`.

---

## Why a Seeded Level Selection?

Because `GetPairsForLevel(X)` is keyed off `X` itself (not `Random.Range`), the player sees the same pair subset for level 5 every time they play it. This makes:
- Completion meaningful (you played *that specific* puzzle).
- Replays deterministic (good for screenshots, bug reports, QA).
- Cross-device progression sensible even though we only persist the completion set, not the specific pair arrangement.

---

## Progress Persistence

Key: `"MatchObjects.CompletedLevels"` in `PlayerPrefs`.
Format: comma-separated ints (`"0,3,4,12"`).
Load: `PlayerPrefs.GetString(key, "")` → split on `,` → `int.TryParse` each.
Save: `PlayerPrefs.SetString(key, string.Join(",", _completed))` → `PlayerPrefs.Save()`.

Simple, synchronous, no async IO. Good enough for a completion set that's at most a few hundred integers. If per-level scoring or timing is ever needed, switch to a JSON blob (one key, deserialized into a richer object).

---

## Extending

- **More levels**: bump `totalLevels` on the config asset. Pool size can stay the same; the level service picks subsets.
- **Bigger pool**: add entries to `pairPool`. More variety per level; each level still picks `pairsPerLevel` of them.
- **Per-level difficulty**: extend `MatchObjectsLevelService` to return a variable pair count based on `levelIndex` (e.g., 3 → 5 → 7 as levels increase). Keep the seed so the result stays deterministic.
- **Star ratings / timing**: swap `IMatchObjectsProgress` for a richer interface (`GetStars(int)`, `MarkCompleted(int, int stars, float timeSec)`) and switch the backing store to JSON.
- **Reset progress**: add a method `void Reset()` on the progress service that clears the hashset and saves.

---

## Common Gotchas

| Symptom | Cause | Fix |
|---|---|---|
| Scroll hides along with the board when a level starts | `_gameParent` is wired to the Scroll View's `Content` | Point `_gameParent` at a dedicated sibling GameRoot |
| "level service or progress not available" error in `Awake` | `MatchObjectsModule` not registered or bootstrap didn't run | Check `AppBootstrap` module array contains `new MatchObjectsModule()` and `_appConfig.matchObjectsLevels` is assigned |
| Every level shows the same pairs | `GetPairsForLevel` called with the same index by mistake, OR pool size < `pairsPerLevel` | Verify `levelIndex` is forwarded correctly; expand `pairPool` |
| Completion doesn't persist across sessions | Another script is overwriting the `PlayerPrefs` key, or `PlayerPrefs.Save()` isn't being reached | Search for the key string; ensure `MarkCompleted` actually runs (breakpoint in `OnGameWon`) |
| Drag doesn't follow pointer | Root canvas is wrong (e.g. a nested sub-canvas picked up by the fallback) | Assign `_rootCanvas` explicitly on the controller |
