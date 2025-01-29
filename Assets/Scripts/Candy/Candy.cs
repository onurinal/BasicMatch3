using System.Collections.Generic;
using UnityEngine;

namespace BasicMatch3.Candies
{
    public class Candy : MonoBehaviour
    {
        [SerializeField] private List<Sprite> candySprites;
        [field: SerializeField] public CandyType CandyType { get; private set; }
        
        public bool IsMatching { get;  set; }

        public Candy Initialize()
        {
            var candyNumber = Random.Range(0, candySprites.Count);
            var candySprite = GetComponentInChildren<SpriteRenderer>();
            candySprite.sprite = candySprites[candyNumber];
            CandyType = (CandyType)candyNumber;
            return this;
        }
    }
}