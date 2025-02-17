using System;
using System.Collections;
using System.Collections.Generic;
using BasicMatch3.Manager;
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
        [field: SerializeField] public int GridX { get; set; }
        [field: SerializeField] public int GridY { get; set; }
        public SpriteRenderer CandySprite => candySprite;

        private bool isFalling = false;
        private bool isSwapping = false;
        private Vector3 targetPosition;
        private Vector3 startPosition;
        private float elapsedTime = 0f;

        public Candy Initialize(int width, int height, LevelManager levelManager)
        {
            var candyNumber = Random.Range(0, candySpriteList.Count - 2);
            candySprite.sprite = candySpriteList[candyNumber];
            CandyType = (CandyType)candyNumber;
            GridX = width;
            GridY = height;

            if (levelManager.IsGridInitializing)
            {
                candySprite.enabled = false;
            }

            return this;
        }

        private void Update()
        {
            Move();
        }

        public void StartMoving(Vector3 startPosition, Vector3 targetPosition)
        {
            this.startPosition = startPosition;
            this.targetPosition = targetPosition;
            elapsedTime = 0f;
            isSwapping = false;
            isFalling = true;
        }

        public void StartSwapping(Vector3 startPosition, Vector3 targetPosition)
        {
            this.startPosition = startPosition;
            this.targetPosition = targetPosition;
            elapsedTime = 0f;
            isFalling = false;
            isSwapping = true;
        }

        private void Move()
        {
            if (!isFalling && !isSwapping)
            {
                return;
            }

            if (elapsedTime < candyProperties.MoveDuration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / candyProperties.MoveDuration);
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