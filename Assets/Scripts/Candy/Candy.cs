using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

        private bool isFalling = false;
        private bool isSwapping = false;
        private Vector3 targetPosition;
        private Vector3 startPosition;
        private float elapsedTime = 0f;

        public Candy Initialize(int width, int height)
        {
            var candyNumber = Random.Range(0, candySpriteList.Count);
            candySprite.sprite = candySpriteList[candyNumber];
            CandyType = (CandyType)candyNumber;
            GridX = width;
            GridY = height;
            return this;
        }

        private void Update()
        {
            Move();
        }

        public void StartMoving(Vector3 startPosition, Vector3 targetPosition, bool isFalling)
        {
            this.startPosition = startPosition;
            this.targetPosition = targetPosition;
            elapsedTime = 0f;
            if (isFalling)
            {
                this.isFalling = true;
                isSwapping = false;
            }
            else
            {
                this.isFalling = false;
                isSwapping = true;
            }
        }

        private void Move()
        {
            if (!isFalling && !isSwapping)
            {
                return;
            }

            var duration = isFalling ? candyProperties.FallDuration : candyProperties.SwapDuration;

            if (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
            }
            else
            {
                transform.position = targetPosition;
                isFalling = false;
                isSwapping = false;
            }
        }
    }
}