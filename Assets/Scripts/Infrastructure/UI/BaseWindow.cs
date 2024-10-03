using System;
using UnityEngine;

namespace Infrastructure.UI
{
    public abstract class BaseWindow : MonoBehaviour
    {
        public event Action<BaseWindow> OnShow;
        public event Action<BaseWindow> OnHide;

        public bool CanInteract { get; private set; }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            OnShow?.Invoke(this);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            OnHide?.Invoke(this);
        }

        public virtual void ChangeInteractionState(bool canInteract)
        {
            CanInteract = canInteract;   
        }
    }
}