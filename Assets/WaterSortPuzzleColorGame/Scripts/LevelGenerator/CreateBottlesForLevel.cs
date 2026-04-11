
using UnityEngine;

namespace WaterSortPuzzleGame.LevelGenerator
{
    public class CreateBottlesForLevel : MonoBehaviour
    {
        public void CreateBottles(int numberOfBottleToCreate, bool matchState, bool rainbowBottle,ref int totalWaterCount,
            LevelColorController levelColorController, Data data, int createdBottles)
        {
            for (int i = 0; i < numberOfBottleToCreate; i++)
            {
                GenerateBottle tempBottle = new GenerateBottle(i);
                
                LevelMakerHelper.DecreaseTotalWaterCount(tempBottle,ref totalWaterCount);
                
                levelColorController.GetRandomColorForBottle(tempBottle, matchState, rainbowBottle, data);
                
                

                data.CreatedBottles.Add(tempBottle);

                createdBottles++;
            }
        }
    }
}