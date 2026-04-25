using UnityEngine;
using BlitzEcs;

namespace FlappyECS
{
    /// <summary>
    /// API tập trung để khởi tạo các Blueprint thực thể.
    /// Giúp code Base sạch hơn và dễ quản lý logic khởi tạo.
    /// </summary>
    public static class EntityFactory
    {
        public static Entity CreateBird(World world, GameObject prefab, Vector2 position)
        {
            Entity bird = world.Spawn();
            bird.Add(new Position { value = position })
                .Add(new Rotation { value = 0f })
                .Add(new Velocity { value = Vector2.zero })
                .Add(new Gravity { value = -9.8f })
                .Add(new RenderObject { gameObject = Object.Instantiate(prefab) })
                .Add(new ColliderRect { size = new Vector2(0.8f, 0.8f) })
                .Add(new Scale { value = 1f })
                .Add(new BirdTag())
                .Add(new ScoreTag { value = 0 });
            return bird;
        }

        public static Entity CreateObstacle(World world, GameObject prefab, Vector2 position, int pairId, ObstacleType type)
        {
            var go = Object.Instantiate(prefab);
            var sr = go.GetComponentInChildren<SpriteRenderer>();
            Vector2 colliderSize = new Vector2(1f, 10f); // Default pipe size

            Entity obstacle = world.Spawn()
                .Add(new Position { value = position })
                .Add(new Velocity { value = new Vector2(-2f, 0) })
                .Add(new RenderObject { gameObject = go })
                .Add(new ObstacleTag { type = type, pairId = pairId, passed = false });

            switch (type)
            {
                case ObstacleType.Pipe:
                    if (sr != null) sr.color = Color.green;
                    break;
                case ObstacleType.Saw:
                    if (sr != null) {
                        sr.color = Color.grey;
                        go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    }
                    colliderSize = new Vector2(1.5f, 1.5f);
                    obstacle.Add(new Rotation { value = 0 })
                            .Add(new RotationSpeedComponent { degreesPerSecond = 360f });
                    break;
                case ObstacleType.Spike:
                    if (sr != null) {
                        sr.color = Color.red;
                        go.transform.localScale = new Vector3(1f, 2f, 1f);
                    }
                    colliderSize = new Vector2(1f, 2f);
                    break;
            }

            obstacle.Add(new ColliderRect { size = colliderSize });
            return obstacle;
        }

        public static Entity CreateSpecialObstacle(World world, GameObject prefab, Vector2 position, int pairId, ObstacleType type, string special)
        {
            Entity obstacle = CreateObstacle(world, prefab, position, pairId, type);
            
            if (special == "Moving")
            {
                obstacle.Add(new OscillatingMovementComponent {
                    type = MoveType.Vertical,
                    amplitude = 2f,
                    frequency = 2f,
                    center = position
                });
            }
            else if (special == "Troll")
            {
                obstacle.Add(new TrollComponent {
                    detectRange = 3f,
                    dodgeOffset = new Vector2(0, 5f) // Bay vọt lên trên
                });
                var sr = obstacle.Get<RenderObject>().gameObject.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.color = Color.magenta; // Cảnh báo troll
            }
            else if (special == "Pulse")
            {
                obstacle.Add(new Scale { value = 1f })
                        .Add(new SineScaleComponent { minScale = 0.5f, maxScale = 2f, frequency = 3f });
            }

            return obstacle;
        }

        // Alias để không break code cũ
        public static Entity CreatePipe(World world, GameObject prefab, Vector2 position, int pairId)
        {
            return CreateObstacle(world, prefab, position, pairId, ObstacleType.Pipe);
        }

        public static Entity CreateParallax(World world, GameObject prefab, float speed, float startX, float resetX)
        {
            var go = Object.Instantiate(prefab);
            Entity bg = world.Spawn();
            bg.Add(new Position { value = (Vector2)go.transform.position })
              .Add(new RenderObject { gameObject = go })
              .Add(new ParallaxTag { speed = speed, startX = startX, resetX = resetX });
            return bg;
        }

        public static Entity CreateItem(World world, GameObject prefab, Vector2 position, ItemType type)
        {
            var go = Object.Instantiate(prefab);
            
            // Đổi màu để phân biệt loại item (Nếu bạn chưa có prefab riêng)
            var sr = go.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                switch (type)
                {
                    case ItemType.Invincibility: sr.color = Color.white; break;
                    case ItemType.ScoreMultiplier: sr.color = Color.yellow; break;
                    case ItemType.TimeWarp: sr.color = Color.cyan; break;
                    case ItemType.Shrink: sr.color = Color.green; break;
                    case ItemType.Shield: sr.color = Color.blue; break;
                    case ItemType.GravityFlip: sr.color = Color.magenta; break;
                    case ItemType.Magnet: sr.color = new Color(1f, 0.5f, 0f); break;
                }
            }

            Entity item = world.Spawn();
            item.Add(new Position { value = position })
                .Add(new Rotation { value = 0f })
                .Add(new RenderObject { gameObject = go })
                .Add(new ColliderRect { size = new Vector2(0.8f, 0.8f) })
                .Add(new Velocity { value = new Vector2(-2f, 0) })
                .Add(new ItemTag { type = type });
            return item;
        }
        public static Entity CreateFloatingText(World world, string text, Vector2 position, Color color)
        {
            GameObject go = new GameObject("FloatingText");
            var textMesh = go.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.color = color;
            textMesh.fontSize = 48;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.characterSize = 0.1f;
            
            go.transform.position = position;

            Entity entity = world.Spawn();
            entity.Add(new Position { value = position })
                  .Add(new RenderObject { gameObject = go })
                  .Add(new FloatingTextComponent { remainingTime = 1.5f, speed = 2f });
            return entity;
        }
    }
}
