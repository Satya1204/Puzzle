using UnityEngine;

namespace WaterSortPuzzleGame.LevelScripts
{
    public class LevelBottlesAligner : MonoBehaviour
    {
        public GameObject LevelParent { get; set; }
        public GameObject LastCreatedParent { get; set; }

        private GameObject BottlePreRotationPosition;
        private GameObject LeftRotationPoint;
        private GameObject RightRotationPoint;
        public void CreateLevelParentAndLineObjects(int numberOfColorInLevel)
        {
            LevelParent = new GameObject("LevelParent");
            LevelParent.AddComponent<LevelParent>();
            LevelParent.GetComponent<LevelParent>().NumberOfColor = numberOfColorInLevel;
            LastCreatedParent = LevelParent;
        }
        public void CreateAssumeNextPosition(GameObject firstBottle)
        {
            if (GameObject.Find("BottlePreRotationPosition"))
            {
                Destroy(GameObject.Find("BottlePreRotationPosition"));
            }

            BottlePreRotationPosition = new GameObject("BottlePreRotationPosition");
            BottlePreRotationPosition.transform.localScale = firstBottle.transform.localScale;
            LeftRotationPoint = new GameObject("LeftRotationPoint");
            RightRotationPoint = new GameObject("RightRotationPoint");
            LeftRotationPoint.transform.parent = RightRotationPoint.transform.parent = BottlePreRotationPosition.transform;
            LeftRotationPoint.transform.localPosition = firstBottle.transform.GetChild(1).localPosition;
            RightRotationPoint.transform.localPosition = firstBottle.transform.GetChild(2).localPosition;
            LeftRotationPoint.transform.localScale = firstBottle.transform.GetChild(1).localScale;
            RightRotationPoint.transform.localScale = firstBottle.transform.GetChild(2).localScale;
            LeftRotationPoint.SetActive(false);
            RightRotationPoint.SetActive(false);
           
        }
    }
   
}