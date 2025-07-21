using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BitorTools
{
    [RequireComponent(typeof(Button))]
    public abstract class ImageButtonUI<T> : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        [SerializeField] protected Image _iconImage;
        [SerializeField] protected TextMeshProUGUI _label;
        private Button _button;

        protected T StoredValue;

        public event Action<T> OnHover;
        public event Action<T> OnSelect;
        public event Action<T> OnClick;

        protected virtual void Start()
        {
            _button.onClick.AddListener(HandleClick);
        }

        public void Setup(string label, Sprite icon, T storedValue)
        {
            this.StoredValue = storedValue;
            _iconImage.sprite = icon;
            _iconImage.gameObject.SetActive(icon);
            _label.text = label;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover?.Invoke(StoredValue);
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            OnSelect?.Invoke(StoredValue);
        }

        void HandleClick()
        {
            OnClick?.Invoke(StoredValue);
        }
    }
}
