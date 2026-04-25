using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlappyECS
{
    public class ItemIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private Query<Position, ColliderRect, ItemTag> itemQuery;
        private Query<Position, ColliderRect, BirdTag> birdQuery;
        private Query<GameStateComponent> stateQuery;
        public void OnCreate(World world)
        {
            itemQuery = new Query<Position, ColliderRect, ItemTag>(world);
            birdQuery = new Query<Position, ColliderRect, BirdTag>(world);
            stateQuery = new Query<GameStateComponent>(world);
        }

        public void OnUpdate(World world, float deltaTime)
        {
            GameState currentState = GameState.Start;
            stateQuery.ForEach((ref GameStateComponent state) => currentState = state.value);

            if (currentState != GameState.Playing)
                return;

            // Kiểm tra va chạm với Chim (Logic Spawn đã chuyển sang PipeIecsSystem)
            CheckCollision(world);
        }

        private void CheckCollision(World world)
        {
            birdQuery.ForEach((Entity birdEntity, ref Position bPos, ref ColliderRect bCol, ref BirdTag birdTag) =>
            {
                var bMin = bPos.value - bCol.size / 2;
                var bMax = bPos.value + bCol.size / 2;

                itemQuery.ForEach((Entity itemEntity, ref Position iPos, ref ColliderRect iCol, ref ItemTag iTag) =>
                {
                    var iMin = iPos.value - iCol.size / 2;
                    var iMax = iPos.value + iCol.size / 2;

                    if (bMin.x < iMax.x && bMax.x > iMin.x &&
                        bMin.y < iMax.y && bMax.y > iMin.y)
                    {
                        // Thay vì xử lý logic buff trực tiếp, ta chỉ gửi "Yêu cầu" (Request)
                        // BuffIecsSystem sẽ chịu trách nhiệm xử lý các Request này.
                        birdEntity.Add(new ApplyBuffRequest { type = iTag.type });
                        
                        // Xóa item
                        if(!itemEntity.Has<DestroyTag>())
                            itemEntity.Add(new DestroyTag());
                            
                        Debug.Log($"Item collected! Requesting buff: {iTag.type}");
                    }
                });
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(ColliderRect), typeof(ItemTag) };
            yield return new[] { typeof(Position), typeof(ColliderRect), typeof(BirdTag) };
        }
    }
}
