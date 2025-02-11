using UnityEngine;

namespace BasicMatch3.Candies
{
    [CreateAssetMenu(fileName = "Candy Properties", menuName = "Basic Match3/Create New Candy Properties")]
    public class CandyProperties : ScriptableObject
    {
        [SerializeField] private Candy candyPrefab;
        [SerializeField] private float scaleFactor;
        [SerializeField] private float fallDuration;
        [SerializeField] private float swapDuration;

        public Candy CandyPrefab => candyPrefab;
        public float ScaleFactor => scaleFactor;
        public float FallDuration => fallDuration;
        public float SwapDuration => swapDuration;
    }
}