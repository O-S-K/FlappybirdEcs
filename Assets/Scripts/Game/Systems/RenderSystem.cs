using System.Collections.Generic;
using UnityEngine;
using BlitzEcs;

namespace FlappyECS
{
    public class RenderSystem : ISystem , IQueryDebugInfo
    {
        public void OnUpdate(World world, float deltaTime)
        {
            var query = new Query<RenderObject, Position, Rotation>(world);
            query.ForEach(( ref RenderObject render, ref Position pos, ref Rotation rot) =>
            {
                if (render.gameObject)
                {
                    render.gameObject.transform.position = pos.value;
                    
                    // Cập nhật vị trí
                    render.gameObject.transform.position = pos.value;

                    // Lerp mượt rotation thực tế (Z)
                    Quaternion targetRot = Quaternion.Euler(0, 0, rot.value);
                    render.gameObject.transform.rotation = Quaternion.Lerp(render.gameObject.transform.rotation, targetRot, deltaTime * 5f);
                }
            });
        }
        
        public IEnumerable<System.Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(RenderObject), typeof(Position), typeof(Rotation) };
        }
    }
}