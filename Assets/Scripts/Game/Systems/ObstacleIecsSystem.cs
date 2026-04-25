using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlappyECS
{
    public class ObstacleIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private Query<Rotation, RotationSpeedComponent> rotationQuery;
        private Query<LaserComponent, RenderObject, ColliderRect> laserQuery;
        private Query<Position, OscillatingMovementComponent> oscillationQuery;
        private Query<Scale, SineScaleComponent> scalingQuery;
        private Query<Position, TrollComponent> trollQuery;
        private Query<Position, BirdTag> birdQuery;

        public void OnCreate(World world)
        {
            rotationQuery = new Query<Rotation, RotationSpeedComponent>(world);
            laserQuery = new Query<LaserComponent, RenderObject, ColliderRect>(world);
            oscillationQuery = new Query<Position, OscillatingMovementComponent>(world);
            scalingQuery = new Query<Scale, SineScaleComponent>(world);
            trollQuery = new Query<Position, TrollComponent>(world);
            birdQuery = new Query<Position, BirdTag>(world);
        }

        public void OnUpdate(World world, float deltaTime)
        {
            // 1. Xử lý quay cho Cưa (Saw)
            rotationQuery.ForEach((ref Rotation rot, ref RotationSpeedComponent speed) =>
            {
                rot.value += speed.degreesPerSecond * deltaTime;
            });

            // 2. Xử lý Laze (Laser)
            laserQuery.ForEach((ref LaserComponent laser, ref RenderObject render, ref ColliderRect collider) =>
            {
                laser.timer -= deltaTime;
                if (laser.timer <= 0)
                {
                    laser.isActive = !laser.isActive;
                    laser.timer = laser.isActive ? laser.duration : laser.interval;
                }

                Vector2 size = new Vector3(5, 1, 1f);
                if (render.gameObject != null)
                {
                    var sr = render.gameObject.GetComponentInChildren<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = laser.isActive ? new Color(1, 0, 0, 1f) : new Color(1, 0, 0, 0.5f);
                        if (laser.isActive)
                        {
                            sr.transform.localScale = size;
                        }
                    }
                }
                collider.size = laser.isActive ? size : Vector2.zero;
            });

            // 3. Di chuyển dập dìu (Oscillation)
            oscillationQuery.ForEach((ref Position pos, ref OscillatingMovementComponent osc) =>
            {
                float offset = Mathf.Sin(Time.time * osc.frequency + osc.phase) * osc.amplitude;
                if (osc.type == MoveType.Vertical)
                    pos.value.y = osc.center.y + offset;
                else if (osc.type == MoveType.Horizontal)
                    pos.value.x = osc.center.x + offset;
            });

            // 4. Phình to thu nhỏ (Sine Scale)
            scalingQuery.ForEach((ref Scale scale, ref SineScaleComponent sine) =>
            {
                scale.value = Mathf.Lerp(sine.minScale, sine.maxScale, (Mathf.Sin(Time.time * sine.frequency) + 1f) / 2f);
            });

            // 5. Troll (Né tránh khi chim lại gần)
            Vector2 bPos = Vector2.zero;
            birdQuery.ForEach((ref Position p, ref BirdTag t) => bPos = p.value);

            trollQuery.ForEach((ref Position p, ref TrollComponent troll) =>
            {
                if (!troll.triggered && Vector2.Distance(p.value, bPos) < troll.detectRange)
                {
                    troll.triggered = true;
                    p.value += troll.dodgeOffset; // Né tránh tức thì!
                    AudioManager.Instance.PlayJump(); // Giả âm thanh để troll
                }
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Rotation), typeof(RotationSpeedComponent) };
            yield return new[] { typeof(LaserComponent), typeof(RenderObject), typeof(ColliderRect) };
            yield return new[] { typeof(Position), typeof(OscillatingMovementComponent) };
            yield return new[] { typeof(Scale), typeof(SineScaleComponent) };
            yield return new[] { typeof(Position), typeof(TrollComponent) };
        }
    }
}
