using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class MagnetIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private Query<Position, BirdTag, MagnetComponent> birdQuery;
        private Query<Position, ItemTag> itemQuery;

        public void OnCreate(World world)
        {
            birdQuery = new Query<Position, BirdTag, MagnetComponent>(world);
            itemQuery = new Query<Position, ItemTag>(world);
        }

        public void OnUpdate(World world, float deltaTime)
        {
            birdQuery.ForEach((ref Position birdPos, ref BirdTag birdTag, ref MagnetComponent magnet) =>
            {
                Vector2 bp = birdPos.value;
                float currentRange = magnet.range;
                float currentPullSpeed = magnet.pullSpeed;
                float radiusSq = currentRange * currentRange;

                itemQuery.ForEach((ref Position itemPos, ref ItemTag itemTag) =>
                {
                    Vector2 ip = itemPos.value;
                    float distSq = (ip - bp).sqrMagnitude;

                    if (distSq < radiusSq)
                    {
                        // Hút vật phẩm về phía chim
                        Vector2 direction = (bp - ip).normalized;
                        itemPos.value += direction * currentPullSpeed * deltaTime;
                    }
                });
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(BirdTag), typeof(MagnetComponent) };
            yield return new[] { typeof(Position), typeof(ItemTag) };
        }
    }
}
