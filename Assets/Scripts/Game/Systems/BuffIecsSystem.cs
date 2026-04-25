using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public struct BuffDisplayData {
        public ItemType type;
        public float remainingTime;
        public float totalTime;
    }

    public class BuffIecsSystem : IECSSystem
    {
        public static System.Action<List<BuffDisplayData>> OnBuffsUpdated;
        
        private Query<BuffComponent> buffQuery;
        private Query<ApplyBuffRequest, Position> requestQuery;
        private Query<BirdTag> birdQuery;

        public void OnCreate(World world)
        {
            buffQuery = new Query<BuffComponent>(world);
            requestQuery = new Query<ApplyBuffRequest, Position>(world);
            birdQuery = new Query<BirdTag>(world);
        }

        private List<BuffDisplayData> displayCache = new List<BuffDisplayData>();

        public void OnUpdate(World world, float deltaTime)
        {
            // 1. Xử lý yêu cầu Buff mới
            requestQuery.ForEach((Entity entity, ref ApplyBuffRequest request, ref Position pos) =>
            {
                ApplyBuff(entity, request.type, pos.value, world);
                entity.Remove<ApplyBuffRequest>();
            });

            // 2. Cập nhật thời gian
            buffQuery.ForEach((Entity entity, ref BuffComponent buff) =>
            {
                buff.remainingTime -= deltaTime;
                if (buff.remainingTime <= 0)
                {
                    entity.Remove<BuffComponent>();
                    if (entity.Has<InvincibleTag>()) entity.Remove<InvincibleTag>();
                    if (entity.Has<SpeedBoostComponent>()) entity.Remove<SpeedBoostComponent>();
                    if (entity.Has<ScoreMultiplierComponent>()) entity.Remove<ScoreMultiplierComponent>();
                    if (entity.Has<TimeWarpComponent>()) entity.Remove<TimeWarpComponent>();
                    if (entity.Has<GravityFlipComponent>()) entity.Remove<GravityFlipComponent>();
                    if (entity.Has<MagnetComponent>()) entity.Remove<MagnetComponent>();
                    if (entity.Has<ShieldComponent>()) entity.Remove<ShieldComponent>();
                    
                    if (entity.Has<GhostComponent>())
                    {
                        entity.Remove<GhostComponent>();
                        var sr = entity.Get<RenderObject>().gameObject.GetComponentInChildren<SpriteRenderer>();
                        if (sr != null) sr.color = Color.white;
                    }

                    if (entity.Has<ShrinkComponent>())
                    {
                        entity.Remove<ShrinkComponent>();
                        if (entity.Has<Scale>()) entity.Get<Scale>().value = 1.0f;
                        if (entity.Has<ColliderRect>()) entity.Get<ColliderRect>().size = new Vector2(0.8f, 0.8f);
                    }
                }
            });

            // 3. Cập nhật UI (Chỉ cho Bird)
            displayCache.Clear();
            birdQuery.ForEach((Entity bird, ref BirdTag tag) => {
                if (bird.Has<BuffComponent>()) {
                    var buff = bird.Get<BuffComponent>();
                    ItemType? currentType = null;
                    if (bird.Has<InvincibleTag>()) currentType = ItemType.Invincibility;
                    else if (bird.Has<ScoreMultiplierComponent>()) currentType = ItemType.ScoreMultiplier;
                    else if (bird.Has<TimeWarpComponent>()) currentType = ItemType.TimeWarp;
                    else if (bird.Has<GravityFlipComponent>()) currentType = ItemType.GravityFlip;
                    else if (bird.Has<MagnetComponent>()) currentType = ItemType.Magnet;
                    else if (bird.Has<ShrinkComponent>()) currentType = ItemType.Shrink;
                    else if (bird.Has<ShieldComponent>()) currentType = ItemType.Shield;
                    else if (bird.Has<GhostComponent>()) currentType = ItemType.Ghost;

                    if (currentType.HasValue) {
                        displayCache.Add(new BuffDisplayData {
                            type = currentType.Value,
                            remainingTime = buff.remainingTime,
                            totalTime = 8.0f 
                        });
                    }
                }
            });
            OnBuffsUpdated?.Invoke(displayCache);
        }

        private void ApplyBuff(Entity bird, ItemType type, Vector2 birdPos, World world)
        {
            // Reset existing specific buff components
            if (bird.Has<InvincibleTag>()) bird.Remove<InvincibleTag>();
            if (bird.Has<ScoreMultiplierComponent>()) bird.Remove<ScoreMultiplierComponent>();
            if (bird.Has<TimeWarpComponent>()) bird.Remove<TimeWarpComponent>();
            if (bird.Has<GravityFlipComponent>()) bird.Remove<GravityFlipComponent>();
            if (bird.Has<MagnetComponent>()) bird.Remove<MagnetComponent>();
            if (bird.Has<ShrinkComponent>()) bird.Remove<ShrinkComponent>();
            if (bird.Has<GhostComponent>()) bird.Remove<GhostComponent>();
            if (bird.Has<ShieldComponent>()) bird.Remove<ShieldComponent>();

            bird.Add(new BuffComponent { remainingTime = 8.0f });

            switch (type)
            {
                case ItemType.Invincibility:
                    bird.Add(new InvincibleTag());
                    EntityFactory.CreateFloatingText(world, "INVINCIBLE!", birdPos + Vector2.up * 1.5f, Color.yellow);
                    break;
                case ItemType.ScoreMultiplier:
                    bird.Add(new ScoreMultiplierComponent { multiplier = 2 });
                    EntityFactory.CreateFloatingText(world, "X2 SCORE!", birdPos + Vector2.up * 1.5f, Color.cyan);
                    break;
                case ItemType.TimeWarp:
                    bird.Add(new TimeWarpComponent { speedMultiplier = 0.5f });
                    EntityFactory.CreateFloatingText(world, "SLOW MOTION!", birdPos + Vector2.up * 1.5f, Color.white);
                    break;
                case ItemType.Shield:
                    bird.Add(new ShieldComponent { charges = 1 });
                    EntityFactory.CreateFloatingText(world, "SHIELD READY!", birdPos + Vector2.up * 1.5f, Color.blue);
                    break;
                case ItemType.Shrink:
                    bird.Add(new ShrinkComponent { scaleFactor = 0.5f });
                    if (bird.Has<Scale>()) bird.Get<Scale>().value = 0.5f;
                    if (bird.Has<ColliderRect>()) bird.Get<ColliderRect>().size = new Vector2(0.4f, 0.4f);
                    EntityFactory.CreateFloatingText(world, "TINY BIRD!", birdPos + Vector2.up * 1.5f, Color.green);
                    break;
                case ItemType.GravityFlip:
                    bird.Add(new GravityFlipComponent { multiplier = -1f });
                    EntityFactory.CreateFloatingText(world, "GRAVITY FLIP!", birdPos + Vector2.up * 1.5f, Color.gray);
                    break;
                case ItemType.Magnet:
                    bird.Add(new MagnetComponent { range = 5.0f, pullSpeed = 10.0f });
                    EntityFactory.CreateFloatingText(world, "MAGNET!", birdPos + Vector2.up * 1.5f, Color.red);
                    break;
                case ItemType.Ghost:
                    bird.Add(new GhostComponent());
                    var sr = bird.Get<RenderObject>().gameObject.GetComponentInChildren<SpriteRenderer>();
                    if (sr != null) sr.color = new Color(1, 1, 1, 0.5f);
                    EntityFactory.CreateFloatingText(world, "GHOST!", birdPos + Vector2.up * 1.5f, new Color(1, 1, 1, 0.8f));
                    break;
            }

            AudioManager.Instance.PlayPowerUp();
        }
    }
}
