using System.Collections.Generic;
using UnityEngine;

namespace BasicMatch3.Candies
{
    public class Candy : MonoBehaviour
    {
        [SerializeField] private CandyProperties candyProperties;
        [SerializeField] private List<Sprite> candySpriteList;
        [SerializeField] private SpriteRenderer candySprite;
        [field: SerializeField] public CandyType CandyType { get; private set; }

        [field: SerializeField] public bool IsMatching { get; set; } = false;

        [field: SerializeField] public int GridX { get; set; }
        [field: SerializeField] public int GridY { get; set; }

        public Candy Initialize(int width, int height)
        {
            var candyNumber = Random.Range(0, candySpriteList.Count - 2);
            candySprite.sprite = candySpriteList[candyNumber];
            CandyType = (CandyType)candyNumber;
            GridX = width;
            GridY = height;
            return this;
        }
    }
}