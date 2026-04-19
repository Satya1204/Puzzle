using System;
using UnityEngine;

namespace PuzzleApp.Features.MatchObjects
{
    [Serializable]
    public class MatchObjectsItemPair
    {
        public string pairId;
        public Sprite leftSprite;
        public Sprite rightSprite;
        public string label;
    }
}
