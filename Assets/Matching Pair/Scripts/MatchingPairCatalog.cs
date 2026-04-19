using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleApp.Features.MatchingPair
{
    public interface IMatchingPairCatalog
    {
        IReadOnlyList<MatchingPairDefinition> GetVariants();
        bool TryGetVariant(int pieceCount, out MatchingPairDefinition definition);
    }

    public sealed class MatchingPairCatalog : IMatchingPairCatalog
    {
        readonly IReadOnlyList<MatchingPairDefinition> _variants;
        readonly Dictionary<int, MatchingPairDefinition> _byPieceCount;

        public MatchingPairCatalog(IReadOnlyList<MatchingPairDefinition> variants)
        {
            _variants = variants ?? Array.Empty<MatchingPairDefinition>();
            _byPieceCount = new Dictionary<int, MatchingPairDefinition>(_variants.Count);

            int nullCount = 0;
            for (int i = 0; i < _variants.Count; i++)
            {
                var def = _variants[i];
                if (def != null)
                {
                    _byPieceCount[def.pieceCount] = def;
                    Debug.Log($"[MatchingPair] Catalog variant[{i}]: pieceCount={def.pieceCount} title='{def.title}' cardPrefab={(def.cardPrefab != null ? def.cardPrefab.name : "NULL")} gamePrefab={(def.gamePrefab != null ? def.gamePrefab.name : "NULL")}");
                }
                else
                {
                    nullCount++;
                }
            }

            Debug.Log($"[MatchingPair] Catalog built: variants.Count={_variants.Count} nullEntries={nullCount} uniqueByPieceCount={_byPieceCount.Count}");
        }

        public IReadOnlyList<MatchingPairDefinition> GetVariants() => _variants;

        public bool TryGetVariant(int pieceCount, out MatchingPairDefinition definition) =>
            _byPieceCount.TryGetValue(pieceCount, out definition);
    }
}
