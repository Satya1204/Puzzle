using UnityEngine;
using UnityEngine.UI;

namespace PuzzleApp.UI
{
    public abstract class CardItemBase<TData> : MonoBehaviour where TData : class
    {
        [SerializeField] protected Button _button;
        [SerializeField] protected Image _iconImage;

        protected TData _data;

        protected virtual void Awake()
        {
            if (_button == null)
                _button = GetComponent<Button>();
        }

        protected virtual void OnEnable()
        {
            if (_button != null)
                _button.onClick.AddListener(OnPressed);
        }

        protected virtual void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnPressed);
        }

        public virtual void Bind(TData data)
        {
            _data = data;
            ApplyBinding(data);
        }

        protected abstract void ApplyBinding(TData data);

        void OnPressed() => OnClicked(_data);

        protected abstract void OnClicked(TData data);
    }
}
