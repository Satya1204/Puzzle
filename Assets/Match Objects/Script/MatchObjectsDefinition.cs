using System;
using UnityEngine;

namespace PuzzleApp.Features.MatchObjects
{
    [Serializable]
    public class MatchObjectsDefinition
    {
        public string id;
        public string title;
        public Sprite icon;
        public GameObject cardPrefab;
        public MatchObjectsItemPair[] pairs;
    }
}
