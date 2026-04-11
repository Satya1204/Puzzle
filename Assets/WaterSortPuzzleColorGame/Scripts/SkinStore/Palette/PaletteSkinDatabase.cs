using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    [CreateAssetMenu(fileName = "PaletteSkinDatabase", menuName = "Content/Skins/Palette Skin Database")]
    public class PaletteSkinDatabase : ScriptableObject
    {
        public List<PaletteSkinData> skins;
    }
}
