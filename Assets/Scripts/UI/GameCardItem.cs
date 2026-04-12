using System;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleApp.UI
{
    /// <summary>
    /// Attach to the Game Card prefab root. Optional: wire <see cref="_button"/> in the Inspector.
    /// </summary>
    public class GameCardItem : MonoBehaviour
    {
        [SerializeField] Button _button;

        public event Action<int> Clicked;

        public int Index { get; private set; }

        void Awake()
        {
            if (_button == null) _button = GetComponent<Button>();
        }

        public void Bind(int index)
        {
            Index = index;
            if (_button == null) _button = GetComponent<Button>();
            if (_button == null) return;
            _button.onClick.RemoveListener(OnPressed);
            _button.onClick.AddListener(OnPressed);
        }

        public void Bind(in GameCardDescriptor data, int index)
        {
            Bind(index);
            // Extend here: title, icon from data when you add UI references.
        }

        void OnPressed() => Clicked?.Invoke(Index);
    }
}
