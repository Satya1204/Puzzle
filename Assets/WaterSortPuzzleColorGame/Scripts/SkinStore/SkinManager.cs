using System.Linq;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    public class SkinManager : MonoBehaviour
    {
        public static SkinManager Instance { get; private set; }

        [SerializeField] private ThemeSkinController themeSkinController;
        [SerializeField] private TubeSkinController tubeController;
        [SerializeField] private PaletteSkinController paletteController;

        public ThemeSkinController ThemeSkinController => themeSkinController;
        public TubeSkinController TubesSkinController => tubeController;
        public PaletteSkinController PaletteSkinController => paletteController;

        public void Init()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            themeSkinController.Init();
            tubeController.Init();
            paletteController.Init();
        }
      
    }
}