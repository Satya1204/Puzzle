using System;
using UnityEngine;

namespace PuzzleApp.Features.MatchingPair
{
    /// <summary>
    /// Inspector-driven definition of a Matching Pair variant.
    /// <list type="bullet">
    /// <item><see cref="cardPrefab"/> — tile shown in the lobby scroll grid (must have <c>MatchingPairCardItem</c>).</item>
    /// <item><see cref="gamePrefab"/> — game UI instantiated when the player taps that card.</item>
    /// </list>
    /// </summary>
    [Serializable]
    public class MatchingPairDefinition
    {
        public int pieceCount;
        public string title;
        public Sprite icon;
        public GameObject cardPrefab;
        public GameObject gamePrefab;
    }
}
