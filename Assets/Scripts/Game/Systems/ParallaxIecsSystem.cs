using System;
using System.Collections.Generic;
using BlitzEcs;
 
namespace FlappyECS
{
    public class ParallaxIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private Query<Position, RenderObject, ParallaxTag> query;

        public void OnCreate(World world)
        {
            query = new Query<Position, RenderObject, ParallaxTag>(world);
        }

        public void OnUpdate(World world, float deltaTime)
        {
            if(GameManager.Instance.CurrentState != GameState.Playing)
                return;
            
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