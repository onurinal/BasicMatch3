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

        // public bool isMoving = false;
        // private Vector3 targetPosition;
        private IEnumerator candyMovementCoroutine;

        public Candy Initialize(int width, int height)
        {
            var candyNumber = Random.Range(0, candySpriteList.Count);
            candySprite.sprite = candySpriteList[candyNumber];
            CandyType = (CandyType)candyNumber;
            GridX = width;
            GridY = height;
            return this;
        }

        // private void Update()
        // {
        //     Move();
        // }

        // ------------------------------- CANDY MOVEMENT ------------------------
        private IEnumerator StartCandyMovementCoroutine(Vector3 targetPosition)
        {
            var elapsedTime = 0f;
            while (elapsedTime < candyProperties.FallDuration)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, elapsedTime / candyProperties.FallDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
        }

        public void StartCandyMovement(Vector3 targetPosition)
        {
            candyMovementCoroutine = StartCandyMovementCoroutine(targetPosition);
            CoroutineHandler.Instance.StartCoroutine(candyMovementCoroutine);
        }

        public void StopCandyMovement()
        {
            if (candyMovementCoroutine != null)
            {
                CoroutineHandler.Instance.StopCoroutine(candyMovementCoroutine);
                candyMovementCoroutine = null;
            }
        }

        // TESTING

        // public void StartMoving()
        // {
        //     isMoving = true;
        // }
        //
        // private void Move()
        // {
        //     if (!isMoving)
        //     {
        //         return;
        //     }
        //
        //     var elapsedTime = 0f;
        //     while (elapsedTime < candyProperties.FallDuration)
        //     {
        //         transform.position = Vector3.Lerp(transform.position, targetPosition, elapsedTime / candyProperties.FallDuration);
        //         elapsedTime += Time.deltaTime;
        //     }
        //
        //     transform.position = targetPosition;
        //     isMoving = false;
        // }
        //
        // public void SetTargetPosition(Vector3 targetPosition)
        // {
        //     this.targetPosition = targetPosition;
        // }
    }
}