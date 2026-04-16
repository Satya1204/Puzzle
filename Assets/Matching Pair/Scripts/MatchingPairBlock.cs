using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleApp.MatchingPair
{
    /// <summary>
    /// Per-block view on each Block prefab instance.
    ///
    /// Idle state: rotation Y=0, showing back sprite, clickable.
    /// Revealed:   rotation Y=180 (via animated 0→90 swap sprite 90→180), showing front sprite.
    /// Mismatch:   animated back to Y=0, showing back sprite again.
    /// Matched:    fades out.
    ///
    /// Same pattern as MatchPairsMemoryGame._Card.
    /// </summary>
    [RequireComponent(typeof(Image), typeof(Button))]
    public class MatchingPairBlock : MonoBehaviour
    {
        [SerializeField] Image _image;
        [SerializeField] Button _button;

        int _pairId = -1;
        int _blockIndex;
        bool _faceUp;
        bool _turning;
        bool _resolved;
        Sprite _frontSprite;
        Sprite _backSprite;

        public int PairId => _pairId;
        public int BlockIndex => _blockIndex;
        public bool IsFaceUp => _faceUp;
        public bool IsResolved => _resolved;

        public event Action<MatchingPairBlock> Clicked;

        public void Setup(int blockIndex, int pairId, Sprite front, Sprite back)
        {
            EnsureRefs();

            _blockIndex = blockIndex;
            _pairId = pairId;
            _frontSprite = front;
            _backSprite = back;
            _faceUp = false;
            _resolved = false;
            _turning = false;

            _image.sprite = _backSprite;
            _image.color = Color.white;
            transform.localRotation = Quaternion.identity;
        }

        void Awake()
        {
            EnsureRefs();

            if (_button != null)
            {
                _button.onClick.RemoveListener(OnPress);
                _button.onClick.AddListener(OnPress);
            }
        }

        void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnPress);
        }

        void OnPress()
        {
            if (_faceUp || _turning || _resolved)
                return;

            Clicked?.Invoke(this);
        }

        /// <summary>Flip from back (Y=0) to front (Y=180). Swaps sprite at the 90° midpoint.</summary>
        public void Reveal()
        {
            if (_faceUp || _turning || _resolved)
                return;

            _turning = true;
            StartCoroutine(Flip90(0.25f, swapSprite: true, revealFront: true));
        }

        /// <summary>Flip from front (Y=180) back to back (Y=0). Swaps sprite at the 90° midpoint.</summary>
        public void HideCard()
        {
            if (!_faceUp || _turning || _resolved)
                return;

            _turning = true;
            StartCoroutine(Flip90(0.25f, swapSprite: true, revealFront: false));
        }

        /// <summary>Fade out after a successful match.</summary>
        public void Resolve()
        {
            _resolved = true;
            StartCoroutine(FadeOut());
        }

        /// <summary>Immediately show front sprite at Y=0 (no animation, used for lone odd card).</summary>
        public void SetFaceUpImmediate()
        {
            EnsureRefs();
            _faceUp = true;
            _image.sprite = _frontSprite;
            transform.localRotation = Quaternion.identity;
        }

        void EnsureRefs()
        {
            if (_image == null) _image = GetComponent<Image>();
            if (_button == null) _button = GetComponent<Button>();
        }

        /// <summary>
        /// Rotate 90° around Y, optionally swap sprite at the midpoint, then rotate another 90°.
        /// Total rotation per call = 180°.
        /// First half:  current → +90°  (card becomes edge-on, invisible)
        /// Swap sprite at midpoint.
        /// Second half: +90° → +180°    (card faces camera again with new sprite)
        /// </summary>
        IEnumerator Flip90(float halfDuration, bool swapSprite, bool revealFront)
        {
            Quaternion start = transform.localRotation;
            Quaternion mid = start * Quaternion.Euler(0f, 90f, 0f);

            float rate = 1f / halfDuration;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * rate;
                transform.localRotation = Quaternion.Slerp(start, mid, t);
                yield return null;
            }

            if (swapSprite)
            {
                _faceUp = revealFront;
                _image.sprite = _faceUp ? _frontSprite : _backSprite;
            }

            Quaternion end = mid * Quaternion.Euler(0f, 90f, 0f);
            t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * rate;
                transform.localRotation = Quaternion.Slerp(mid, end, t);
                yield return null;
            }

            _turning = false;
        }

        IEnumerator FadeOut()
        {
            float rate = 1f / 1.5f;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * rate;
                _image.color = Color.Lerp(Color.white, Color.clear, t);
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
