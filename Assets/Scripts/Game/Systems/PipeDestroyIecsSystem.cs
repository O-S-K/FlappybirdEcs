using System;
using System.Collections;
using System.Collections.Generic;
using BlitzEcs;
using Object = UnityEngine.Object;

namespace FlappyECS
{
    public class PipeDestroyIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private Query<Position, PipeTag, RenderObject> query;
        private List<Entity> toRemove = new List<Entity>();
        
        public void OnCreate(World world)
        {
            query = new Query<Position, PipeTag, RenderObject>(world);
        }
        
        public void OnUpdate(World world, float deltaTime)
        {
             toRemove = new List<Entity>();
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
