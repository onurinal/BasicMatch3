using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.CameraManager
{
    public class CameraController : MonoBehaviour
    {
        public void Initialize(LevelProperties levelProperties, float candyScaleFactor)
        {
            UpdateCameraPosition(levelProperties, candyScaleFactor);
        }

        private void UpdateCameraPosition(LevelProperties levelProperties, float candyScaleFactor)
        {
            var newCameraPosition = CalculateCameraPosition(new Vector2(levelProperties.GridWidth, levelProperties.GridHeight), candyScaleFactor);
            transform.position = new Vector3(newCameraPosition.x, newCameraPosition.y, transform.position.z);
        }

        private Vector2 CalculateCameraPosition(Vector2 gridSize, float candyScaleFactor)
        {
            var newPositionX = (gridSize.x / 2f * candyScaleFactor) - (candyScaleFactor / 2);
            var newPositionY = (gridSize.y / 2f * candyScaleFactor) - (candyScaleFactor / 2);
            return new Vector2(newPositionX, newPositionY);
        }
    }
}