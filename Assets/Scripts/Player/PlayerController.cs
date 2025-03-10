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
        private int gridWidth, gridHeight;

        private Candy firstCandy;
        private bool isFirstCandySelected = false;
        private bool hasSwapped = false;

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
            HandleCandySelection();
        }

        private void HandleCandySelection()
        {
            var inputPosition = GetInputPosition();
            if (inputPosition != Vector2.zero)
            {
                if (!isFirstCandySelected)
                {
                    firstCandy = GetCandyAtInputPosition(inputPosition);
                    if (firstCandy != null)
                    {
                        isFirstCandySelected = true;
                        hasSwapped = false;
                    }
                }
                else if (!hasSwapped)
                {
                    SwapCandies(inputPosition);
                }
            }
        }

        private Vector2 GetInputPosition()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                playerInput.SetFirstMousePosition(playerInput.MousePosition);
                return playerInput.FirstMousePosition;
            }

            if (Input.GetMouseButton(0) && isFirstCandySelected)
            {
                return playerInput.MousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                ResetSelection();
                return Vector2.zero;
            }

#elif UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    playerInput.SetFirstTouchPosition(playerInput.TouchPosition);
                    return playerInput.FirstTouchPosition;
                }

                if (touch.phase == TouchPhase.Moved && isFirstCandySelected)
                {
                    return playerInput.TouchPosition;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    ResetSelection();
                    return Vector2.zero;
                }
            }
#endif
            return Vector2.zero;
        }

        private void SwapCandies(Vector2 inputPosition)
        {
#if UNITY_EDITOR

            var delta = inputPosition - playerInput.FirstMousePosition;

#elif UNITY_ANDROID
            var delta = inputPosition - playerInput.FirstTouchPosition;

#endif

            if (delta.magnitude > 0.1f)
            {
                var secondCandyDirection = GetSecondCandyDirection(delta);
                var secondCandyX = firstCandy.GridX + (int)secondCandyDirection.x;
                var secondCandyY = firstCandy.GridY + (int)secondCandyDirection.y;
                if (secondCandyX >= gridWidth || secondCandyX < 0 || secondCandyY >= gridHeight || secondCandyY < 0)
                {
                    ResetSelection();
                    return;
                }

                var secondCandy = gridSpawner.GetCandyPrefabAtIndex(secondCandyX, secondCandyY);
                gridMovement.StartSwapCandies(firstCandy, secondCandy);

                hasSwapped = true;
                ResetSelection();
            }
        }

        private void ResetSelection()
        {
            isFirstCandySelected = false;
            firstCandy = null;
        }

        private Candy GetCandyAtInputPosition(Vector2 position)
        {
            var hit = Physics2D.Raycast(position, Vector2.down, 0.1f, candyLayer);
            if (hit.collider == null) return null;

            return hit.collider.gameObject.GetComponentInParent<Candy>();
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