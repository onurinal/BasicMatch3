using UnityEngine;

namespace BasicMatch3.Player
{
    [CreateAssetMenu(fileName = "Player Input", menuName = "Basic Match3/Create New Player Input")]
    public class PlayerInput : ScriptableObject
    {
        public float HorizontalInput { get; private set; }
        public float VerticalInput { get; private set; }
        public Vector2 MousePosition { get; private set; }

        private Camera mainCamera;

        public void Initialize()
        {
            mainCamera = Camera.main;
        }

        public void UpdateInput()
        {
            HorizontalInput = Input.GetAxisRaw("Horizontal");
            VerticalInput = Input.GetAxisRaw("Vertical");
            MousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}