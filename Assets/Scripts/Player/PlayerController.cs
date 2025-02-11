using System;
using System.Collections;
using BasicMatch3.Candies;
using BasicMatch3.Grid;
using BasicMatch3.Level;
using UnityEngine;

namespace BasicMatch3.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private LayerMask candyLayer;

        private LevelProperties levelProperties;
        private GridMovement gridMovement;
        private GridSpawner gridSpawner;
        private Candy firstCandy;
        private Vector2 initialMousePosition;
        private int gridWidth, gridHeight;

        public void Initialize(GridMovement gridMovement, GridSpawner gridSpawner, LevelProperties levelProperties)
        {
            this.gridMovement = gridMovement;
            this.gridSpawner = gridSpawner;
            this.levelProperties = levelProperties;
            playerInput.Initialize();

            gridWidth = levelProperties.GridWidth;
            gridHeight = levelProperties.GridHeight;
        }

        private void Update()
        {
            playerInput.UpdateInput();
            SelectCandyToSwap();
        }

        private void SelectCandyToSwap()
        {
            if (Input.GetMouseButtonDown(0))
            {
                initialMousePosition = playerInput.MousePosition;
                firstCandy = GetCandyAtMousePosition();
            }
            else if (Input.GetMouseButton(0) && firstCandy)
            {
                Vector2 currentMousePosition = playerInput.MousePosition;
                Vector2 delta = currentMousePosition - initialMousePosition;
                if (delta.magnitude > 0.1f)
                {
                    Vector2 secondCandyDirection = GetSecondCandyDirection(delta);
                    var secondCandyX = firstCandy.GridX + (int)secondCandyDirection.x;
                    var secondCandyY = firstCandy.GridY + (int)secondCandyDirection.y;
                    if (secondCandyX >= gridWidth || secondCandyX < 0 || secondCandyY >= gridHeight || secondCandyY < 0)
                    {
                        firstCandy = null;
                        return;
                    }

                    var secondCandy = gridSpawner.GetCandyPrefabAtIndex(secondCandyX, secondCandyY);
                    gridMovement.StartSwapCandies(firstCandy, secondCandy);
                    firstCandy = null;
                }
            }
            else if (Input.GetMouseButtonUp(0) && firstCandy)
            {
                firstCandy = null;
            }
        }

        private Candy GetCandyAtMousePosition()
        {
            var hit = Physics2D.Raycast(playerInput.MousePosition, Vector2.down, 0.1f, candyLayer);
            if (hit.collider)
            {
                return hit.collider.gameObject.GetComponentInParent<Candy>();
            }

            return null;
        }

        private Vector2 GetSecondCandyDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0 ? Vector2.right : Vector2.left;
            }

            return delta.y > 0 ? Vector2.up : Vector2.down;
        }
    }
}