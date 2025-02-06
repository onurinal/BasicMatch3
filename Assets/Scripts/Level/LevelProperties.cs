using UnityEngine;

namespace BasicMatch3.Level
{
    [CreateAssetMenu(fileName = "Level 1", menuName = "Basic Match3/Create New Level Properties")]
    public class LevelProperties : ScriptableObject
    {
        [SerializeField] private int gridWidth;
        [SerializeField] private int gridHeight;

        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
    }
}