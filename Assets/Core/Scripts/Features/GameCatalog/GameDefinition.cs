using System;
using UnityEngine;

namespace PuzzleApp.Features.GameCatalog
{
    /// <summary>
    /// Inspector-driven definition of a game.
    /// <list type="bullet">
    /// <item><see cref="catalogCardPrefab"/> — tile prefab for the Game tab grid (required; configured on <c>GameScreenCardsView</c>).</item>
    /// <item><see cref="lobbyPrefab"/> — full lobby UI instantiated when the player taps that game.</item>
    /// </list>
    /// </summary>
    [Serializable]
    public class GameDefinition
    {
        public int gameId;
        public string title;
        public Sprite icon;
        public GameObject catalogCardPrefab;
        public GameObject lobbyPrefab;
    }
}
