using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    [CreateAssetMenu(fileName = "ThemeSkinDatabase", menuName = "Content/Skins/Theme Skin Database")]
    public class ThemeSkinDatabase : ScriptableObject
    {
        public List<ThemeSkinData> skins;
    }
}
