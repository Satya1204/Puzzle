using System;
using System.Collections.Generic;
using UnityEngine;
using PuzzleApp.App.Services;

namespace PuzzleApp.Features.MatchObjects
{
    public sealed class MatchObjectsDataService : IMatchObjectsDataService
    {
        const string DataResourcePath = "match_objects_data";

        readonly IGameDataProvider _dataProvider;
        MatchObjectsData _cachedData;

        public MatchObjectsDataService(IGameDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public MatchObjectsItemPair[] GetRoundPairs(MatchObjectsDefinition definition)
        {
            if (definition == null || definition.pairs == null || definition.pairs.Length == 0)
                return null;

            if (_cachedData == null)
            {
                _cachedData = _dataProvider.LoadJson<MatchObjectsData>(DataResourcePath);
                if (_cachedData?.categories == null)
                {
                    Debug.LogError("MatchObjectsDataService: failed to load categories.");
                    return null;
                }
            }

            foreach (var category in _cachedData.categories)
            {
                if (category.id != definition.id || category.rounds == null || category.rounds.Length == 0)
                    continue;

                var round = category.rounds[UnityEngine.Random.Range(0, category.rounds.Length)];
                return MapIndicesToPairs(round.pairIndices, definition.pairs);
            }

            Debug.LogWarning($"MatchObjectsDataService: no rounds found for category '{definition.id}'.");
            return null;
        }

        static MatchObjectsItemPair[] MapIndicesToPairs(int[] indices, MatchObjectsItemPair[] pairs)
        {
            if (indices == null)
                return null;

            var result = new List<MatchObjectsItemPair>(indices.Length);
            foreach (var idx in indices)
            {
                if (idx >= 0 && idx < pairs.Length)
                    result.Add(pairs[idx]);
            }
            return result.ToArray();
        }

        [Serializable]
        class MatchObjectsData
        {
            public MatchObjectsCategory[] categories;
        }

        [Serializable]
        class MatchObjectsCategory
        {
            public string id;
            public MatchObjectsRound[] rounds;
        }

        [Serializable]
        class MatchObjectsRound
        {
            public int[] pairIndices;
        }
    }
}
