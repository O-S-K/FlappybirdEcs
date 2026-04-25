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

    // --- Global / Singleton Components ---
    
    public struct GameStateComponent {
        public GameState value;
    }

    public struct GameConfigComponent {
        public float gameSpeed;
    }

    public struct InputComponent {
        public bool jumpPressed;
    }

    // --- Infrastructure / Event Components ---

    public struct PlaySoundRequest {
        public string soundName; // hoặc enum SoundType
    }

    public struct CameraShakeRequest {
        public float intensity;
        public float duration;
    }

    public struct DestroyTag { } // Đánh dấu thực thể cần bị xóa cuối frame

    // --- Buff / Debuff System ---

    public struct BuffComponent {
        public float remainingTime;
    }

    public struct InvincibleTag { } // Bất tử
    
    public struct SpeedBoostComponent {
        public float multiplier;
    }

    public enum ObstacleType { Pipe, Saw, Spike }
    public struct ObstacleTag {
        public ObstacleType type;
        public int pairId;
        public bool passed;
    }

    public struct RotationSpeedComponent {
        public float degreesPerSecond;
    }

    public struct LaserComponent {
        public float interval; // Thời gian nghỉ
        public float duration; // Thời gian bắn
        public float timer;
        public bool isActive;
    }

    public struct ScoreMultiplierComponent {
        public int multiplier;
    }

    public struct TimeWarpComponent {
        public float speedMultiplier;
    }

    public struct ShrinkComponent {
        public float scaleFactor;
    }

    public struct ShieldComponent {
        public int charges;
    }

    public struct GravityFlipComponent {
        public float multiplier; // -1 for flipped, 1 for normal
    }

    public struct MagnetComponent {
        public float range;
        public float pullSpeed;
    }

    public struct Scale {
        public float value;
    }

    public struct FloatingTextComponent {
        public float remainingTime;
        public float speed;
    }

    // --- Requests (Dùng để giao tiếp giữa các System một cách sạch sẽ) ---
    public enum ItemType { Invincibility, ScoreMultiplier, TimeWarp, Shield, Shrink, GravityFlip, Magnet, Ghost }

    public struct ItemTag {
        public ItemType type;
    }

    public struct GhostComponent { }

    public struct ApplyBuffRequest {
        public ItemType type;
    }

    public struct AddScoreRequest {
        public int amount;
    }

    // --- Obstacle Behaviors ---
    
    public enum MoveType { Vertical, Horizontal, Circular }
    
    public struct OscillatingMovementComponent {
        public MoveType type;
        public float amplitude;
        public float frequency;
        public float phase;
        public Vector2 center;
    }

    public struct SineScaleComponent {
        public float minScale;
        public float maxScale;
        public float frequency;
    }

    public struct TrollComponent {
        public float detectRange;
        public bool triggered;
        public Vector2 dodgeOffset;
    }
}