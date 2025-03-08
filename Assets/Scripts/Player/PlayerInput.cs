using UnityEngine;

namespace BasicMatch3.Player
{
    [CreateAssetMenu(fileName = "Player Input", menuName = "Basic Match3/Create New Player Input")]
    public class PlayerInput : ScriptableObject
    {
        public Vector2 MousePosition { get; private set; }
        public Vector2 TouchPosition { get; private set; }

        public Vector2 FirstMousePosition { get; set; }
        public Vector2 FirstTouchPosition { get; set; }

        private Camera mainCamera;

        public void Initialize()
        {
            mainCamera = Camera.main;
        }

        public void UpdateInput()
        {
#if UNITY_EDITOR
            MousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

#else
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                TouchPosition = mainCamera.ScreenToWorldPoint(touch.position);
            }
#endif
        }
    }
}