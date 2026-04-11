using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame.DataClass
{
    [System.Serializable]
    public class Theme
    {
        public string id;
        public string image;
        public int coin;
        public int levelText;

    }

    [System.Serializable]
    public class Themes
    {
        //employees is case sensitive and must match the string "employees" in the JSON.
        public Theme[] themes;
        public Theme[] tubes;
        public Theme[] palettes;
        public Shop[] shops;
    }

    [System.Serializable]
    public class Shop
    {
        public string id;
        public string image;
        public string title;
        public bool isads;
        public bool ispurchsed;
        public int coin;
        public int tube;
        public int move;
        public int amount;

    }

    [System.Serializable]
    public class RecentlyAddedScreen
    {
        public string name;
        public GameObject gameObject;

    }


    [System.Serializable]
    public class SuccessError
    {
        public ToasterState type;
        public string title;
        public Sprite sprite;
        public string message;

    }
}