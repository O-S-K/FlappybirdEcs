using System;
using System.Collections;
using System.Collections.Generic;
using BlitzEcs;
using Object = UnityEngine.Object;

namespace FlappyECS
{
    public class PipeDestroySystem : ISystem, IQueryDebugInfo
    {
        
        public void OnUpdate(World world, float deltaTime)
        {
            var query = new Query<Position, PipeTag, RenderObject>(world);
            var toRemove = new List<Entity>();

            query.ForEach((Entity entity, ref Position pos, ref PipeTag tag, ref RenderObject render) =>
            {
                if (pos.value.x < Const.LEFT_BOUNDARY)
                {
                    if (render.gameObject) Object.Destroy(render.gameObject);
                    toRemove.Add(entity);
                }
                
                foreach (var e in toRemove)
                    world.Despawn(e);
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(PipeTag), typeof(RenderObject) };
        }
    }
}
