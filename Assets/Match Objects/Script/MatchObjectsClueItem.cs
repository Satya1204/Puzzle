using UnityEngine;
using UnityEngine.UI;

namespace PuzzleApp.MatchObjects
{
    public class MatchObjectsClueItem : MonoBehaviour
    {
        [SerializeField] Image _image;

        string _pairId;
        public string PairId => _pairId;

        public void Bind(string pairId, Sprite sprite)
        {
            _pairId = pairId;
            _image.sprite = sprite;
        }
    }
}
