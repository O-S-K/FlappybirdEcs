using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyECS
{
    public class Const  
    {
        public const float GROUND_Y = 10f; // vị trí y của mặt đất
        public const float LEFT_BOUNDARY = -10f; // vị trí bên trái để despawn ống
        public const int SCORE_PER_PASS = 1; // điểm mỗi khi vượt qua một cặp ống
    }
}
