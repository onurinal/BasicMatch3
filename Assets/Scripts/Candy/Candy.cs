﻿using System;
using System.Collections.Generic;
using BasicMatch3.Manager;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

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

        private Vector3 targetPosition;
        private Vector3 startPosition;

        private Tween moveTween, destroyTween, colorTween;

        public Candy Initialize(int width, int height, LevelManager levelManager)
        {
            if (CandyType != CandyType.Bomb && CandyType != CandyType.Rainbow)
            {
                var candyNumber = Random.Range(0, candySpriteList.Count);
                candySprite.sprite = candySpriteList[candyNumber];
                CandyType = (CandyType)candyNumber;
            }
            else
            {
                StartRandomColorAnimation();
            }

            GridX = width;
            GridY = height;

            if (levelManager.IsGridInitializing)
            {
                candySprite.enabled = false;
            }

            return this;
        }

        // REMOVE OR DISABLE AFTER TESTING FEATURES
        public Candy InitializeForTest(int width, int height, LevelManager levelManager, CandyType candyType)
        {
            if (CandyType != CandyType.Bomb && CandyType != CandyType.Rainbow)
            {
                switch (candyType)
                {
                    case CandyType.Pasta:
                        candySprite.sprite = candySpriteList[0];
                        CandyType = candyType;
                        break;
                    case CandyType.IceCream:
                        candySprite.sprite = candySpriteList[1];
                        CandyType = candyType;
                        break;
                    case CandyType.Biscuit:
                        candySprite.sprite = candySpriteList[2];
                        CandyType = candyType;
                        break;
                    case CandyType.Donut:
                        candySprite.sprite = candySpriteList[3];
                        CandyType = candyType;
                        break;
                    case CandyType.Chocolate:
                        candySprite.sprite = candySpriteList[4];
                        CandyType = candyType;
                        break;
                }
            }

            CandyType = candyType;
            GridX = width;
            GridY = height;

            if (levelManager.IsGridInitializing)
            {
                candySprite.enabled = true;
            }

            return this;
        }

        private void OnDestroy()
        {
            moveTween.Kill();
            destroyTween.Kill();
            colorTween.Kill();
        }

        public void Move(Vector3 targetPosition)
        {
            moveTween = transform.DOMove(targetPosition, candyProperties.MoveDuration).SetEase(Ease.InSine);
        }

        public void MoveToTop(Vector3 targetPosition)
        {
            transform.position = targetPosition;
        }

        public void Destroy()
        {
            destroyTween = transform.DOScale(Vector2.zero, candyProperties.DestroyDuration).SetEase(Ease.InBounce).OnComplete(() => Destroy(gameObject));
        }

        private void StartRandomColorAnimation()
        {
            colorTween = candySprite.DOColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)), candyProperties.ColorChangeDuration).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
        }
    }
}