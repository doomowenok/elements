using UnityEngine;

namespace Core.Element
{
    public class AnimationEventChecker : MonoBehaviour
    {
        [SerializeField] private GridGameElement _element;

        public void SendAnimationEndEvent()
        {
            _element.SetAnimationReceivedState(true);
        }
    }
}