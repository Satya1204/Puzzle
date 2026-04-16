using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PuzzleApp.MatchingPair
{
    /// <summary>
    /// Manages one game board (e.g. "5 Pieces", "6 Pieces", etc.).
    /// Discovers child <see cref="MatchingPairBlock"/> instances, reads a
    /// pre-generated layout from <c>mock_data.json</c> (loaded via Resources),
    /// and drives the reveal-compare-resolve loop.
    ///
    /// Block lifecycle (same as MatchPairsMemoryGame reference):
    ///   Start  → Y=0, back sprite, clickable.
    ///   Click  → Reveal() flips 0→180, shows front.
    ///   Match  → Resolve() fades out.
    ///   Miss   → HideCard() flips 180→0, shows back again.
    ///
    /// Odd block count: (N-1)/2 pairs + 1 deception card with a unique pairId (no match exists).
    /// The deception card looks and behaves like any other card — the player won't know
    /// which one it is. Game ends when all real pairs are matched; only the trick card remains.
    /// </summary>
    public class MatchingPairBoardView : MonoBehaviour
    {
        [Header("Card Art")]
        [SerializeField] Sprite _backSprite;
        [SerializeField] Sprite[] _frontSprites;

        [Tooltip("Optional. If you assign entries here, they define slot order. If empty, blocks are found with GetComponentsInChildren.")]
        [SerializeField] MatchingPairBlock[] _blocksExplicit;

        [Header("Timing")]
        [SerializeField] float _mismatchDelay = 0.6f;

        readonly Dictionary<int, Sprite> _spriteById = new();
        readonly List<MatchingPairBlock> _blocks = new();
        MatchingPairBlock _firstSelection;
        int _pairsToMatch;
        int _pairsMatched;
        bool _inputLocked;
        bool _gameActive;

        MockData _mockData;

        public event Action GameWon;
        public event Action<int, int> PairMatched;

        public bool IsGameActive => _gameActive;

        #region JSON schema matching mock_data.json

        [Serializable]
        class MockEntry
        {
            public int[] cards;
        }

        [Serializable]
        class MockData
        {
            public MockEntry[] entries;
        }

        #endregion

        void Awake()
        {
            BuildSpriteDictionary();
            LoadMockData();
        }

        void OnDestroy()
        {
            foreach (var block in _blocks)
            {
                if (block != null)
                    block.Clicked -= OnBlockClicked;
            }
        }

        public void StartGame()
        {
            DiscoverBlocks();
            if (_blocks.Count < 2)
            {
                Debug.LogWarning("MatchingPairBoardView: need at least 2 blocks to play.");
                return;
            }

            AssignPairs();
            _pairsMatched = 0;
            _gameActive = true;
            _inputLocked = false;
        }

        void BuildSpriteDictionary()
        {
            _spriteById.Clear();

            if (_frontSprites == null)
                return;

            int max = Mathf.Min(12, _frontSprites.Length);
            for (int i = 0; i < max; i++)
            {
                Sprite s = _frontSprites[i];
                if (s == null)
                    continue;

                _spriteById[i + 1] = s;
            }
        }

        void LoadMockData()
        {
            var asset = Resources.Load<TextAsset>("mock_data");
            if (asset == null)
            {
                Debug.LogError("MatchingPairBoardView: Assets/Resources/mock_data.json not found.");
                return;
            }

            _mockData = JsonUtility.FromJson<MockData>(asset.text);
        }

        void DiscoverBlocks()
        {
            foreach (var b in _blocks)
            {
                if (b != null)
                    b.Clicked -= OnBlockClicked;
            }

            _blocks.Clear();

            if (_blocksExplicit != null && _blocksExplicit.Length > 0)
            {
                for (int i = 0; i < _blocksExplicit.Length; i++)
                {
                    var block = _blocksExplicit[i];
                    if (block == null)
                        continue;

                    block.Clicked += OnBlockClicked;
                    _blocks.Add(block);
                }
            }
            else
            {
                var found = GetComponentsInChildren<MatchingPairBlock>(true);
                for (int i = 0; i < found.Length; i++)
                {
                    found[i].Clicked += OnBlockClicked;
                    _blocks.Add(found[i]);
                }
            }
        }

        void AssignPairs()
        {
            int total = _blocks.Count;
            int pairCount = total / 2;
            _pairsToMatch = pairCount;

            int[] cardLayout = PickLayout(total);
            if (cardLayout == null)
            {
                Debug.LogError($"MatchingPairBoardView: no mock_data entry found for {total} cards. Falling back to random.");
                AssignPairsFallback();
                return;
            }

            // Build pairId map from card numbers.
            // Cards sharing the same number get the same pairId.
            // A card number appearing only once (deception) gets a unique pairId = pairCount.
            var numberToPairId = new Dictionary<int, int>();
            var numberCount = new Dictionary<int, int>();

            foreach (int num in cardLayout)
                numberCount[num] = numberCount.TryGetValue(num, out int c) ? c + 1 : 1;

            int nextPairId = 0;
            foreach (var kvp in numberCount)
            {
                if (kvp.Value >= 2)
                    numberToPairId[kvp.Key] = nextPairId++;
            }

            foreach (var kvp in numberCount)
            {
                if (kvp.Value == 1)
                    numberToPairId[kvp.Key] = pairCount;
            }

            for (int i = 0; i < total; i++)
            {
                int cardNum = cardLayout[i];
                int pairId = numberToPairId[cardNum];
                Sprite front = _spriteById.TryGetValue(cardNum, out var s) ? s : _frontSprites[0];
                _blocks[i].Setup(i, pairId, front, _backSprite);
            }
        }

        /// <summary>Pick a random entry from mock_data whose cards length == blockCount.</summary>
        int[] PickLayout(int blockCount)
        {
            if (_mockData == null || _mockData.entries == null)
                return null;

            var candidates = new List<MockEntry>();
            foreach (var entry in _mockData.entries)
            {
                if (entry.cards != null && entry.cards.Length == blockCount)
                    candidates.Add(entry);
            }

            if (candidates.Count == 0)
                return null;

            return candidates[Random.Range(0, candidates.Count)].cards;
        }

        /// <summary>Fallback: old random pair generation if no JSON entry matches.</summary>
        void AssignPairsFallback()
        {
            int total = _blocks.Count;
            bool isOdd = total % 2 != 0;
            int pairCount = total / 2;

            List<int> pool = new List<int>();
            for (int i = 0; i < _frontSprites.Length; i++)
                pool.Add(i);
            Shuffle(pool);

            int[] spriteIndices = new int[pairCount];
            for (int i = 0; i < pairCount; i++)
                spriteIndices[i] = pool[i % pool.Count];

            int[] slotPairId = new int[total];
            Sprite[] slotSprite = new Sprite[total];

            List<int> slots = new List<int>();
            for (int i = 0; i < total; i++)
                slots.Add(i);
            Shuffle(slots);

            int slotIdx = 0;
            for (int p = 0; p < pairCount; p++)
            {
                int sIdx = spriteIndices[p];
                slotPairId[slots[slotIdx]] = p;
                slotSprite[slots[slotIdx]] = _frontSprites[sIdx];
                slotIdx++;
                slotPairId[slots[slotIdx]] = p;
                slotSprite[slots[slotIdx]] = _frontSprites[sIdx];
                slotIdx++;
            }

            if (isOdd)
            {
                int loneSlot = slots[slotIdx];
                int loneSpriteIdx = spriteIndices[pairCount - 1];
                slotPairId[loneSlot] = pairCount;
                slotSprite[loneSlot] = _frontSprites[loneSpriteIdx];
            }

            for (int i = 0; i < total; i++)
                _blocks[i].Setup(i, slotPairId[i], slotSprite[i], _backSprite);
        }

        void OnBlockClicked(MatchingPairBlock block)
        {
            if (!_gameActive || _inputLocked)
                return;

            if (block.IsResolved || block.IsFaceUp)
                return;

            block.Reveal();

            if (_firstSelection == null)
            {
                _firstSelection = block;
                return;
            }

            _inputLocked = true;
            StartCoroutine(EvaluateMatch(_firstSelection, block));
            _firstSelection = null;
        }

        IEnumerator EvaluateMatch(MatchingPairBlock a, MatchingPairBlock b)
        {
            yield return new WaitForSeconds(_mismatchDelay);

            if (a.PairId == b.PairId)
            {
                a.Resolve();
                b.Resolve();
                _pairsMatched++;
                PairMatched?.Invoke(_pairsMatched, _pairsToMatch);

                if (_pairsMatched >= _pairsToMatch)
                {
                    _gameActive = false;
                    GameWon?.Invoke();
                }
            }
            else
            {
                a.HideCard();
                b.HideCard();
            }

            yield return new WaitForSeconds(0.3f);
            _inputLocked = false;
        }

        static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
