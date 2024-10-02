using System;
using Core.Element;
using UnityEngine;

namespace Core.Input
{
    public interface IInputSystem
    {
        event Action<Vector3, GridGameElement> OnEndInput;
        void EnableInput();
        void DisableInput();
    }
}