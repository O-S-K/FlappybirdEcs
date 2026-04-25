using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class FloatingTextIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private Query<Position, FloatingTextComponent, RenderObject> query;

        public void OnCreate(World world)
        {
            query = new Query<Position, FloatingTextComponent, RenderObject>(world);
        }

        public void OnUpdate(World world, float deltaTime)
        {
            query.ForEach((Entity entity, ref Position pos, ref FloatingTextComponent text, ref RenderObject render) =>
            {
                text.remainingTime -= deltaTime;

                // Bay lên trên
                pos.value.y += text.speed * deltaTime;

                if (text.remainingTime <= 0)
                {
                    if (!entity.Has<DestroyTag>())
                        entity.Add(new DestroyTag());
                }
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(FloatingTextComponent), typeof(RenderObject) };
        }
    }
}
