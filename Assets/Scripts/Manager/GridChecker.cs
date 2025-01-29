using System;
using BasicMatch3.Candies;
using UnityEngine;

namespace BasicMatch3.Manager
{
    public class GridChecker : MonoBehaviour
    {
        private Candy[,] candyGrid;

        private void Start()
        {
            
        }

        public void Initialize(Candy[,] candyGrid)
        {
            this.candyGrid = candyGrid;
            // IsMatch3();
        }

        // private void IsMatch3()
        // {
        //     for (int height = 0; height < candyGrid.GetLength(0); height++)
        //     {
        //         for (int width = 0; width < candyGrid.GetLength(1); width++)
        //         {
        //             if (candyGrid[height, width + 1] != null)
        //             {
        //                 if (candyGrid[height, width].CandyType == candyGrid[height, width + 1].CandyType)
        //                 {
        //                     candyGrid[height,width].IsMatching = true;
        //                     candyGrid[height,width + 1].IsMatching = true;
        //                 }
        //             }
        //         }
        //     }
        // }
    }
}