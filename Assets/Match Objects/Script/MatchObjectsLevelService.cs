using System;
using UnityEngine;

namespace PuzzleApp.Features.MatchObjects
{
    public interface IMatchObjectsLevelService
    {
        int TotalLevels { get; }
        MatchObjectsItemPair[] GetPairsForLevel(int levelIndex);
    }

    public sealed class MatchObjectsLevelService : IMatchObjectsLevelService
    {
        readonly MatchObjectsLevelConfig _config;

        public MatchObjectsLevelService(MatchObjectsLevelConfig config)
        {
            _config = config;
        }

        public int TotalLevels => _config != null ? _config.totalLevels : 0;

        public MatchObjectsItemPair[] GetPairsForLevel(int levelIndex)
        {
            if (_config == null || _config.pairPool == null || _config.pairPool.Length == 0)
            {
                Debug.LogError("MatchObjectsLevelService: pair pool is empty.");
                return Array.Empty<MatchObjectsItemPair>();
            }

            int count = Mathf.Min(_config.pairsPerLevel, _config.pairPool.Length);
            var rng = new System.Random(levelIndex);
            var indices = new int[_config.pairPool.Length];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = i;

            for (int i = indices.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (indices[i], indices[j]) = (indices[j], indices[i]);
            }

            var result = new MatchObjectsItemPair[count];
            for (int i = 0; i < count; i++)
                result[i] = _config.pairPool[indices[i]];

            return result;
        }
    }
}
