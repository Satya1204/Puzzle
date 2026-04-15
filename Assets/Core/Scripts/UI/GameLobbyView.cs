using System;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleApp.UI
{
    /// <summary>
    /// Base lobby view with shared Play / Back button wiring.
    /// Subclass per game to add game-specific UI fields.
    /// </summary>
    public class GameLobbyView : MonoBehaviour
    {
        [SerializeField] Button _playButton;
        [SerializeField] Button _backButton;

        public event Action PlayClicked;
        public event Action BackClicked;

        void OnEnable()
        {
            if (_playButton != null)
                _playButton.onClick.AddListener(OnPlay);

            if (_backButton != null)
                _backButton.onClick.AddListener(OnBack);
        }

        void OnDisable()
        {
            if (_playButton != null)
                _playButton.onClick.RemoveListener(OnPlay);

            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBack);
        }

        protected virtual void OnPlay() => PlayClicked?.Invoke();
        protected virtual void OnBack() => BackClicked?.Invoke();
    }
}
