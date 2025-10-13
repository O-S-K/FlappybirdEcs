using System;
using System.Collections.Generic;
using BlitzEcs;
 
namespace FlappyECS
{
    public class ParallaxSystem : ISystem, IQueryDebugInfo
    {
        public void OnUpdate(World world, float deltaTime)
        {
            var query = new Query<Position, RenderObject, ParallaxTag>(world);

            query.ForEach((ref Position pos, ref RenderObject render, ref ParallaxTag para) =>
            {
                if (render.gameObject == null)
                    return;

                pos.value.x -= para.speed * deltaTime;
                if (pos.value.x <= para.resetX)
                    pos.value.x = para.startX;
                render.gameObject.transform.position = pos.value;
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(RenderObject), typeof(ParallaxTag) };
        }
    }
}