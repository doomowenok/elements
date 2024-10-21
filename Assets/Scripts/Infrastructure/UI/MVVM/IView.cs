using UnityEngine;

namespace Infrastructure.UI.MVVM
{
    public interface IView
    {
        GameObject ViewObject { get; }
        void Subscribe();
        void Unsubscribe();
    }
}