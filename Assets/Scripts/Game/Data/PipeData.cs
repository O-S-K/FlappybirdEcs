using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyECS
{
    public class PipeData 
    {
        public const float spawnInterval = 2f; // khoảng thời gian giữa các lần spawn ống

        public const float gapHeight = 10f; // chiều cao khoảng trống giữa hai ống
        public const float leftBoundary = -10f; // vị trí bên trái để despawn ống
        public const float spaceBetweenPipes = 5f; // khoảng cách giữa hai ống (trên và dưới)

    }
}
