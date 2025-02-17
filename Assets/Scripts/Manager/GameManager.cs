using System;
using PlayerSettings;
using UnityEngine;

namespace BasicMatch3.Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;

        private PlayerPrefController playerPrefController;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            playerPrefController = new PlayerPrefController();

            if (levelManager != null)
            {
                levelManager.Initialize(playerPrefController);
            }
        }
    }
}