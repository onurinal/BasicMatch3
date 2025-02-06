using BasicMatch3.CameraManager;
using BasicMatch3.Candies;
using BasicMatch3.Grid;
using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.Manager
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelProperties levelProperties;
        [SerializeField] private CandyProperties candyProperties;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Transform candiesParent;

        private GridSpawner gridSpawner;
        private GridChecker gridChecker;
        private GridMovement gridMovement;

        public void Start()
        {
            cameraController.Initialize(levelProperties, candyProperties.ScaleFactor);
            gridChecker = new GridChecker();
            gridSpawner = new GridSpawner();
            gridSpawner.Initialize(candyProperties, levelProperties, gridChecker, candiesParent);
        }
    }
}