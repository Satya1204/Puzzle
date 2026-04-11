using WaterSortPuzzleGame.DataClass;
using System.Collections.Generic;
using UnityEngine;
namespace WaterSortPuzzleGame
{
    [CreateAssetMenu(fileName = "Success Error Database", menuName = "Content/SuccessErrorDatabase")]
    public class SuccessErrorDatabase : ScriptableObject
    {
        public List<SuccessError> SuccessErrors;
    }
}
