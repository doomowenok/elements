using UnityEngine;

namespace CommonTools
{
    public class StretchSpriteComponent : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            Stretch();
        }

        private void Stretch()
        {
            
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            Vector3 topLeft = _camera.ScreenToWorldPoint(new Vector3(0, screenHeight, _camera.nearClipPlane));
            Vector3 bottomRight = _camera.ScreenToWorldPoint(new Vector3(screenWidth, 0, _camera.nearClipPlane));

            float worldScreenWidth = bottomRight.x - topLeft.x;
            float worldScreenHeight = topLeft.y - bottomRight.y;

            float scaleX = worldScreenWidth / _renderer.sprite.bounds.size.x;
            float scaleY = worldScreenHeight / _renderer.sprite.bounds.size.y;
            
            float finalScale = Mathf.Max(scaleX, scaleY);

            transform.localScale = new Vector3(finalScale, finalScale, 1);
        }
    }
}