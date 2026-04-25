using UnityEngine;
using BlitzEcs;
using System.Collections.Generic;

namespace FlappyECS
{
    public class EntityDebugger : MonoBehaviour
    {
        public Entity entity;
        public World world;
        public bool showDebug = true;

        [Header("ECS Data")]
        public string entityId;
        public bool isAlive;
        public List<string> allComponents = new List<string>();

        private void LateUpdate()
        {
            if (world == null) return;
            isAlive = world.IsEntityAlive(entity);
            if (!isAlive) return;

            entityId = entity.Id.ToString();
            allComponents.Clear();

            if (entity.Has<Position>()) allComponents.Add($"Position: {entity.Get<Position>().value}");
            if (entity.Has<Rotation>()) allComponents.Add($"Rotation: {entity.Get<Rotation>().value:F1}°");
            if (entity.Has<Scale>()) allComponents.Add($"Scale: {entity.Get<Scale>().value:F2}");
            if (entity.Has<Velocity>()) allComponents.Add($"Velocity: {entity.Get<Velocity>().value}");
            if (entity.Has<ColliderRect>()) allComponents.Add($"Collider: {entity.Get<ColliderRect>().size}");
            
            if (entity.Has<BirdTag>()) {
                allComponents.Add("Tag: [BIRD]");
                if (entity.Has<Velocity>()) {
                    float vY = entity.Get<Velocity>().value.y;
                    allComponents.Add($"Bird Jump Power: {vY:F2} ({(vY > 0 ? "Rising" : "Falling")})");
                }
            }
            if (entity.Has<ObstacleTag>()) allComponents.Add($"Obstacle: {entity.Get<ObstacleTag>().type}");
            if (entity.Has<ItemTag>()) allComponents.Add($"Item: {entity.Get<ItemTag>().type}");
            
            if (entity.Has<OscillatingMovementComponent>()) {
                var osc = entity.Get<OscillatingMovementComponent>();
                allComponents.Add($"Oscillating: {osc.type} (Freq:{osc.frequency})");
            }
            
            if (entity.Has<TrollComponent>()) {
                var troll = entity.Get<TrollComponent>();
                allComponents.Add($"Troll: Triggered={troll.triggered}");
            }

            if (entity.Has<BuffComponent>()) allComponents.Add($"Buff Timer: {entity.Get<BuffComponent>().remainingTime:F2}s");
            if (entity.Has<ShieldComponent>()) allComponents.Add($"Shield Charges: {entity.Get<ShieldComponent>().charges}");
            if (entity.Has<MagnetComponent>()) allComponents.Add($"Magnet Range: {entity.Get<MagnetComponent>().range}");
            if (entity.Has<InvincibleTag>()) allComponents.Add("State: <INVINCIBLE>");
            if (entity.Has<GhostComponent>()) allComponents.Add("State: <GHOST>");
        }

        private void OnDrawGizmos()
        {
            if (!showDebug || world == null || !world.IsEntityAlive(entity)) return;

            // 1. Vẽ Collider nếu có
            if (entity.Has<ColliderRect>())
            {
                var col = entity.Get<ColliderRect>();
                var pos = entity.Has<Position>() ? entity.Get<Position>().value : (Vector2)transform.position;
                
                Gizmos.color = Color.green;
                // Nếu là Laser đang tắt (size = 0) thì vẽ màu xám mờ
                if (col.size.magnitude <= 0) Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);

                Gizmos.DrawWireCube(new Vector3(pos.x, pos.y, 0), new Vector3(col.size.x, col.size.y, 0.1f));
            }

            // 2. Vẽ danh sách Component (Chỉ chạy trong Editor để hiển thị Label)
#if UNITY_EDITOR
            string info = $"[Entity ID: {entity.Id}] (Alive: {world.IsEntityAlive(entity)})\n";
            
            // Lấy danh sách tất cả các component (Dùng Reflection hoặc thủ công)
            if (entity.Has<BirdTag>()) info += "• BirdTag\n";
            if (entity.Has<ObstacleTag>()) info += $"• ObstacleTag ({entity.Get<ObstacleTag>().type})\n";
            if (entity.Has<ItemTag>()) info += "• ItemTag\n";
            
            // Các Component đặc biệt
            if (entity.Has<OscillatingMovementComponent>()) info += "• [Special: Moving]\n";
            if (entity.Has<SineScaleComponent>()) info += "• [Special: Pulsing]\n";
            if (entity.Has<TrollComponent>()) info += "• [Special: Troll]\n";

            if (entity.Has<BuffComponent>()) info += $"• Buff Time: {entity.Get<BuffComponent>().remainingTime:F1}s\n";
            if (entity.Has<ShieldComponent>()) info += $"• Shield: {entity.Get<ShieldComponent>().charges}\n";
            if (entity.Has<InvincibleTag>()) info += "• <INVINCIBLE>\n";
            if (entity.Has<GhostComponent>()) info += "• <GHOST>\n";
            if (entity.Has<MagnetComponent>()) info += "• Magnet\n";

            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, info);
#endif
        }
    }
}
