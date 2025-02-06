using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.CameraManager
{
    public class CameraController : MonoBehaviour
    {
        private LevelProperties levelProperties;
        private float candyScaleFactor;

        public void Initialize(LevelProperties levelProperties, float candyScaleFactor)
        {
            this.levelProperties = levelProperties;
            this.candyScaleFactor = candyScaleFactor;
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            var newCameraPosition = CalculateCameraPosition(new Vector2(levelProperties.GridWidth, levelProperties.GridHeight));
            transform.position = new Vector3(newCameraPosition.x, newCameraPosition.y, transform.position.z);
        }

        private Vector2 CalculateCameraPosition(Vector2 gridSize)
        {
            var newX = (gridSize.x / 2f * candyScaleFactor) - (candyScaleFactor / 2);
            var newY = (gridSize.y / 2f * candyScaleFactor) - (candyScaleFactor / 2);
            return new Vector2(newX, newY);
        }
    }
}