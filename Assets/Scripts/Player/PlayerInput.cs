using UnityEngine;

namespace BasicMatch3.Player
{
    [CreateAssetMenu(fileName = "Player Input", menuName = "Basic Match3/Create New Player Input")]
    public class PlayerInput : ScriptableObject
    {
        public Vector2 MousePosition { get; private set; }

        private Camera mainCamera;

        public void Initialize()
        {
            mainCamera = Camera.main;
        }

        public void UpdateInput()
        {
            MousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}