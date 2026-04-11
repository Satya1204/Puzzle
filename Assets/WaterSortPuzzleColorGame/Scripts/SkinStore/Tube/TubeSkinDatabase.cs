using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    [CreateAssetMenu(fileName = "TubeSkinDatabase", menuName = "Content/Skins/Tube Skin Database")]
    public class TubeSkinDatabase : ScriptableObject
    {
        public List<TubeSkinData> skins;
    }
}
