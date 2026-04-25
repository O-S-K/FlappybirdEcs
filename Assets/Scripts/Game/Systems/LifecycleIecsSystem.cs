using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    /// <summary>
    /// System chuyên trách dọn dẹp các thực thể bị đánh dấu Destroy.
    /// Nên được đặt ở cuối danh sách System.
    /// </summary>
    public class LifecycleIecsSystem : IECSSystem
    {
        private Query<DestroyTag> query;

        public void OnCreate(World world)
        {
            query = new Query<DestroyTag>(world);
        }

        public void OnUpdate(World world, float deltaTime)
        {
            query.ForEach((Entity entity, ref DestroyTag tag) =>
            {
                // Nếu có RenderObject, hãy destroy GameObject trước
                if (entity.Has<RenderObject>())
                {
                    var render = entity.Get<RenderObject>();
                    if (render.gameObject != null)
                    {
                        Object.Destroy(render.gameObject);
                    }
                }
                
                // Hủy thực thể trong ECS World (Sử dụng Despawn theo API của BlitzEcs)
                entity.Despawn();
            });
        }
    }
}
