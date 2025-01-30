using System.Collections.Generic;
using UnityEngine;

namespace BasicMatch3.Candies
{
    public class Candy : MonoBehaviour
    {
        [SerializeField] private List<Sprite> candySprites;
        [field: SerializeField] public CandyType CandyType { get; private set; }

        [field: SerializeField] public bool IsMatching { get; set; } = false;
        [field: SerializeField] public bool IsAlreadyMatched { get; set; } = false;

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