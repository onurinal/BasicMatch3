using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BasicMatch3.CameraManager;
using BasicMatch3.Candies;
using BasicMatch3.Grid;
using BasicMatch3.Level;
using BasicMatch3.Player;
using PlayerSettings;
using UnityEngine;

namespace BasicMatch3.Manager
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private CandyProperties candyProperties;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Transform candiesParent;

        private GridSpawner gridSpawner;
        private GridChecker gridChecker;
        private GridMovement gridMovement;
        private PlayerPrefController playerPrefController;

        public bool IsGridInitializing { get; private set; } = true;
        private IEnumerator scanGridCoroutine;
        private IEnumerator newLevelCoroutine;

        [SerializeField] private List<LevelProperties> levelPropertiesList;
        private LevelProperties currentLevelProperties;
        private readonly Dictionary<int, LevelProperties> levelPropertiesDictionary = new Dictionary<int, LevelProperties>();

        private void OnDestroy()
        {
            if (CoroutineHandler.Instance != null)
            {
                StopNewLevel();
                StopScanGrid();
            }
        }

        public void Initialize(PlayerPrefController playerPrefController)
        {
            this.playerPrefController = playerPrefController;
            UpdateLevelPropertiesDictionary();
            GetLevelPropertiesForCurrentLevel();
            cameraController.Initialize(currentLevelProperties, candyProperties.ScaleFactor);
            gridMovement = new GridMovement();
            gridChecker = new GridChecker();
            gridSpawner = new GridSpawner();
            gridSpawner.Initialize(this, gridChecker, gridMovement, candyProperties, currentLevelProperties, candiesParent);
            playerController.Initialize(gridMovement, gridSpawner, currentLevelProperties);

            StartNewLevel();
        }

        private void UpdateLevelPropertiesDictionary()
        {
            for (int i = 0; i < levelPropertiesList.Count; i++)
            {
                int level = i + 1;
                levelPropertiesDictionary.Add(level, levelPropertiesList[i]);
            }
        }

        private void GetLevelPropertiesForCurrentLevel()
        {
            var currentLevel = playerPrefController.GetMasterLevel();
            if (!levelPropertiesDictionary.TryGetValue(currentLevel, out var levelProperties))
            {
                Debug.LogWarning("No level properties found for active scene: " + currentLevel);
                return;
            }

            currentLevelProperties = levelProperties;
        }

        private IEnumerator ScanGridCoroutine()
        {
            var duration = IsGridInitializing ? 0f : candyProperties.MoveDuration;

            do
            {
                gridChecker.DestroyMatchedCandies();
                yield return new WaitForSeconds(0.1f);
                yield return gridMovement.StartFillCandyToEmptySlot(duration);
                yield return gridSpawner.StartCreateNewCandies(duration / 3f);
                gridChecker.CheckAllCandies();
                yield return null;
            } while (gridChecker.MatchedCandyList.Count > 0);
        }

        public IEnumerator StartScanGrid()
        {
            StopScanGrid();
            scanGridCoroutine = ScanGridCoroutine();
            yield return CoroutineHandler.Instance.StartCoroutine(scanGridCoroutine);
        }

        private void StopScanGrid()
        {
            if (scanGridCoroutine != null)
            {
                CoroutineHandler.Instance.StopCoroutine(scanGridCoroutine);
                scanGridCoroutine = null;
            }
        }

        private IEnumerator NewLevelCoroutine()
        {
            yield return CoroutineHandler.Instance.StartCoroutine(StartScanGrid());
            gridMovement.MoveAllCandiesToTheTop();
            gridSpawner.ShowAllCandies();
            yield return gridSpawner.StartCreateNewGrid(candyProperties.MoveDuration / 1.5f);
            IsGridInitializing = false;
            newLevelCoroutine = null;
        }

        // THIS IS FOR MANUAL GRID - TESTING - DISABLE IT IF NOT USING
        // private IEnumerator NewLevelCoroutine()
        // {
        //     IsGridInitializing = false;
        //     yield return CoroutineHandler.Instance.StartCoroutine(StartScanGrid());
        //     yield return new WaitForSeconds(candyProperties.MoveDuration);
        //     // yield return gridSpawner.StartCreateNewGrid(candyProperties.MoveDuration / 1.5f);
        //     newLevelCoroutine = null;
        // }

        private void StartNewLevel()
        {
            if (newLevelCoroutine != null)
            {
                return;
            }

            newLevelCoroutine = NewLevelCoroutine();
            CoroutineHandler.Instance.StartCoroutine(newLevelCoroutine);
        }

        private void StopNewLevel()
        {
            if (newLevelCoroutine != null)
            {
                CoroutineHandler.Instance.StopCoroutine(newLevelCoroutine);
                newLevelCoroutine = null;
            }
        }

        //----------------- TESTING -------------------

        public void RunNextLevel()
        {
            var currentLevel = playerPrefController.GetMasterLevel();
            if (currentLevel + 1 >= levelPropertiesList.Count)
            {
                Debug.LogWarning("No level properties found for next level");
                return;
            }

            playerPrefController.SetMasterLevel(currentLevel + 1);
            sceneLoader.LoadSameScene();
        }

        public void RunPreviousLevel()
        {
            var currentLevel = playerPrefController.GetMasterLevel();
            if (currentLevel - 1 <= 0)
            {
                Debug.LogWarning("No level properties found for previous level");
                return;
            }

            playerPrefController.SetMasterLevel(currentLevel - 1);
            sceneLoader.LoadSameScene();
        }
    }
}