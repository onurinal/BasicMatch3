using UnityEngine;

namespace BasicMatch3.Level
{
    [CreateAssetMenu(fileName = "Level 1", menuName = "Basic Match3/Create Level Properties")]
    class LevelProperties : ScriptableObject
    {
        [SerializeField] private int gridHeight, gridWidth;

        public int GridHeight => gridHeight;
        public int GridWidth => gridWidth;
    }
}