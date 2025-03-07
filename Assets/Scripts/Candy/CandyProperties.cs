﻿using UnityEngine;

namespace BasicMatch3.Candies
{
    [CreateAssetMenu(fileName = "Candy Properties", menuName = "Basic Match3/Create New Candy Properties")]
    public class CandyProperties : ScriptableObject
    {
        [SerializeField] private Candy candyPrefab;
        [SerializeField] private Candy bombCandyPrefab;
        [SerializeField] private Candy rainbowCandyPrefab;
        [SerializeField] private float scaleFactor;
        [SerializeField] private float moveDuration;
        [SerializeField] private float destroyDuration;
        [SerializeField] private float colorChangeDuration;

        public Candy CandyPrefab => candyPrefab;
        public Candy BombCandyPrefab => bombCandyPrefab;
        public Candy RainbowCandyPrefab => rainbowCandyPrefab;
        public float ScaleFactor => scaleFactor;
        public float MoveDuration => moveDuration;
        public float DestroyDuration => destroyDuration;
        public float ColorChangeDuration => colorChangeDuration;
    }
}