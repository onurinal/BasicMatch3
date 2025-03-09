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

        private IPlayerGridMovement gridMovement;
        private GridSpawner gridSpawner;
        private Candy firstCandy;
        private int gridWidth, gridHeight;

        public void Initialize(IPlayerGridMovement gridMovement, GridSpawner gridSpawner, LevelProperties levelProperties)
        {
            this.gridMovement = gridMovement;
            this.gridSpawner = gridSpawner;
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
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                playerInput.FirstMousePosition = playerInput.MousePosition;
                firstCandy = GetCandyAtInputPosition(playerInput.FirstMousePosition);
            }
            else if (Input.GetMouseButton(0) && firstCandy)
            {
                var currentMousePosition = playerInput.MousePosition;
                var delta = currentMousePosition - playerInput.FirstMousePosition;
                if (delta.magnitude > 0.1f)
                {
                    var secondCandyDirection = GetSecondCandyDirection(delta);
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

#elif UNITY_ANDROID
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                playerInput.FirstTouchPosition = playerInput.TouchPosition;
                firstCandy = GetCandyAtInputPosition(playerInput.FirstTouchPosition);
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && firstCandy)
            {
                var currentTouchPosition = playerInput.TouchPosition;
                var delta = currentTouchPosition - playerInput.FirstTouchPosition;
                if (delta.magnitude > 0.1f)
                {
                    var secondCandyDirection = GetSecondCandyDirection(delta);
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
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && firstCandy)
            {
                firstCandy = null;
            }
#endif
        }

        private Candy GetCandyAtInputPosition(Vector2 position)
        {
            var hit = Physics2D.Raycast(position, Vector2.down, 0.1f, candyLayer);
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