using System;
using System.Collections;
using System.Collections.Generic;
using BlitzEcs;
using Object = UnityEngine.Object;

namespace FlappyECS
{
    public class CleanupIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private Query<Position, RenderObject> genericQuery;
        
        public void OnCreate(World world)
        {
            // Query tất cả thực thể có Position và RenderObject để kiểm tra biên
            genericQuery = new Query<Position, RenderObject>(world);
        }
        
        public void OnUpdate(World world, float deltaTime)
        {
            genericQuery.ForEach((Entity entity, ref Position pos, ref RenderObject render) =>
            {
                // Chỉ dọn dẹp nếu là Pipe hoặc Item (không dọn dẹp Bird hay Ground)
                if (entity.Has<ObstacleTag>() || entity.Has<ItemTag>())
                {
                    if (pos.value.x < Const.LEFT_BOUNDARY)
                    {
                        if (!entity.Has<DestroyTag>())
                            entity.Add(new DestroyTag());
                    }
                }
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(RenderObject) };
        }
    }
}
