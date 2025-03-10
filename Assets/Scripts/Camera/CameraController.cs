using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.CameraManager
{
    public class CameraController : MonoBehaviour
    {
        public void Initialize(LevelProperties levelProperties, float candyScaleFactor)
        {
            var newCameraPosition = CalculateCameraPosition(levelProperties.GridWidth, levelProperties.GridHeight, candyScaleFactor);
            transform.position = new Vector3(newCameraPosition.x, newCameraPosition.y, transform.position.z);
        }

        private Vector2 CalculateCameraPosition(int gridWith, int gridHeight, float candyScaleFactor)
        {
            var newPositionX = (gridWith / 2f * candyScaleFactor) - (candyScaleFactor / 2);
            var newPositionY = (gridHeight / 2f * candyScaleFactor) - (candyScaleFactor / 2);
            return new Vector2(newPositionX, newPositionY);
        }
    }
}