## Matching Pair Game – Design & Flow

This document explains how the Matching Pair game is wired into the app, how the board is generated, and how the gameplay loop works.

### High-Level Overview

- **Feature entrypoint**: `MatchingPairController` (UI controller created from the app bootstrap/module system).
- **Lobby view**: `MatchingPairLobbyView` shows multiple difficulty cards (5 pieces, 6 pieces, …).
- **Game board**: Each difficulty card instantiates an `N Pieces.prefab` (`5 Pieces`, `6 Pieces`, …) which has:
  - Several child `Block` instances (using the shared `Block.prefab`).
  - A root `MatchingPairBoardView` component that controls that board.
- **Block view**: Each `Block` has a `MatchingPairBlock` component that:
  - Holds its `pairId`, sprites and state.
  - Plays the flip animation and fade-out.
  - Raises `Clicked` to the board.

At a high level, the flow is:

1. Player selects a mode in the Matching Pair lobby.
2. `MatchingPairController` instantiates the corresponding `N Pieces.prefab`.
3. `MatchingPairBoardView.StartGame()` discovers all child blocks and assigns their content.
4. Player clicks cards; the board reveals, compares and resolves them.
5. When all real pairs are matched, the board fires `GameWon` and the controller cleans up / returns to lobby.

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
        { "cards": [ 11, 10, 1, 10, 1, 11 ] },
        ...
      ]
    }
    ```
  - `cards` is an array of integers in the range `1..12`, one per block on the board.
- On startup, `MatchingPairBoardView.Awake()`:
  - Builds `_spriteById` from `_frontSprites`.
  - Loads `mock_data` via `Resources.Load<TextAsset>` and deserializes it into:
    - `MockData.entries[]` of `MockEntry { int[] cards; }`.

#### 3. Picking a layout for a given mode

When `StartGame()` is called:

1. The board counts how many blocks it has (`total`).
2. It looks into the loaded JSON and filters all entries whose `cards.Length == total`.
3. It randomly selects one of those candidates.
4. That `cards` array becomes the **layout** for this round:
   - `cards[i]` (1..12) is the **card number** for block at slot `i`.

If there is no layout with the required length (should not happen with the generated data), the board falls back to the old in-code random generation as a safety net.

### Pair Logic & Deception Card

Given the chosen `cards` array:

- The board first counts how many times each number appears.
- **Real pairs**:
  - Any number that appears at least twice is considered a “real pair”.
  - All blocks with that number share the same `pairId` (0, 1, 2, …).
- **Deception card (odd sizes)**:
  - When the total block count is odd, one number will appear only once.
  - That single card is the **trick card**:
    - It gets a special `pairId = pairCount` that no other block shares.
    - It uses a normal front sprite from `_spriteById[cardNumber]`.
    - It behaves like any other card (clickable, flippable), but it can never match.
- **Win condition**:
  - `pairCount = total / 2` (integer division).
  - `_pairsToMatch = pairCount`.
  - Each time a real pair is matched, `_pairsMatched++`.
  - When `_pairsMatched >= _pairsToMatch`, the board fires `GameWon`.
  - In odd modes, this means all real pairs are faded out and the only card left is the deception card.

### Block Behaviour & Animation

Each `MatchingPairBlock`:

- Holds:
  - `PairId` (int): which logical pair it belongs to.
  - `BlockIndex` (int): slot index on the board.
  - `_frontSprite`, `_backSprite`: current sprites.
  - State flags: `_faceUp`, `_turning`, `_resolved`.
- Is wired in the prefab with:
  - `_image` (card `Image`).
  - `_button` (card `Button`).
- On setup:
  - `Setup(index, pairId, front, back)` sets sprites and resets state.
  - Starts at rotation Y = 0, showing the **back** sprite.

**Click flow:**

1. User taps a block’s button.
2. `MatchingPairBlock` raises `Clicked(this)` if it is not already face-up, turning, or resolved.
3. `MatchingPairBoardView.OnBlockClicked`:
   - Ignores clicks while `_inputLocked` or if the block is already resolved/face-up.
   - Calls `Reveal()` on the block.
   - If this is the first selection, it stores it and waits for the second.
   - On the second selection, it locks input and starts `EvaluateMatch(first, second)`.

**Animation style (flip):**

- `Reveal()` / `HideCard()` both:
  - Rotate the card 0→90 degrees (edge-on), swap the sprite, then 90→180 (or back to 0).
  - This mimics the flip from the original `MatchPairsMemoryGame` asset.
- `Resolve()`:
  - Starts a fade-out coroutine, tweening alpha to zero, then deactivates the GameObject.

### Match Evaluation

`EvaluateMatch(a, b)` coroutine:

1. Waits for `_mismatchDelay` (so the player can see both cards).
2. Compares `a.PairId` and `b.PairId`:
   - **If equal**:
     - Calls `Resolve()` on both blocks.
     - Increments `_pairsMatched` and raises `PairMatched(pairsMatched, pairsToMatch)`.
     - If all real pairs are matched, sets `_gameActive = false` and raises `GameWon`.
   - **If different**:
     - Calls `HideCard()` on both, flipping them back to backs.
3. After a short extra delay, unlocks input so the player can continue.

### Integration with the Rest of the App

- `MatchingPairController`:
  - Subscribes to `MatchingPairLobbyView` to know which mode (piece count) was selected.
  - Instantiates the corresponding `N Pieces.prefab` under the game screen root.
  - Locates the `MatchingPairBoardView` on that prefab and calls `StartGame()`.
  - Listens to `GameWon` from the board and handles:
    - Showing any win UI or transitions.
    - Destroying the active board and returning to the lobby when appropriate.
- This follows the same pattern as other features:
  - Controller coordinates views and game logic.
  - Views are dumb UI that expose events and bind simple data.
  - No scene loading; everything runs within the shell architecture and modules.

### Notes for Extending

- **Adding new sprites**:
  - Extend `_frontSprites` in the Inspector and make sure the first 12 entries correspond to IDs 1–12.
  - Regenerate `mock_data.json` if you want to use new IDs beyond 12 (and adjust the dictionary logic).
- **Changing difficulty**:
  - Add/remove entries in `mock_data.json` or adjust the generator so specific patterns appear more/less frequently.
  - Create additional `N Pieces.prefab` variants with different visual layouts; the board logic will still work as long as there are N `MatchingPairBlock` children.
- **Telemetry / UX tweaks**:
  - Use `PairMatched` events to drive score, move counters, or progress bars.
  - Use `GameWon` to trigger celebration animations, confetti, or navigation to a results screen.

