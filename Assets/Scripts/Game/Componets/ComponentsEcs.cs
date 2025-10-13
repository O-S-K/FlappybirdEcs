using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyECS
{
    public struct Position
    {
        public Vector2 value;
    }

    public struct Rotation
    {
        public float value;
    }

    public struct Velocity
    {
        public Vector2 value;
    }

    public struct BirdTag
    {
    } // đánh dấu player
    
    public struct PipeTag
    {
        public bool passed; // đánh dấu chướng ngại vật
        public int pairId; // ID nhóm (trên + dưới)
    } 

    public struct ScoreTag
    {
        public int value;
    } // dùng cho hệ thống điểm

    public struct Gravity
    {
        public float value;
    }

    public struct RenderObject
    {
        public GameObject gameObject;
    }

    public struct ColliderRect
    {
        public Vector2 size;
    }
    
    public struct ParallaxTag {
        public float speed;      
        public float resetX;     
        public float startX;     
    }
}