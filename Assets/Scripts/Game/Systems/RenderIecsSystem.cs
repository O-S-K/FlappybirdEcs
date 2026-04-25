using System.Collections.Generic;
using UnityEngine;
using BlitzEcs;

namespace FlappyECS
{
    public class RenderIecsSystem : IECSSystem , IQueryDebugInfo
    {
        private Query<RenderObject, Position> query;
        public void OnCreate(World world)
        {
            query = new Query<RenderObject, Position>(world);
        }
        public void OnUpdate(World world, float deltaTime)
        {
            query.ForEach((Entity entity, ref RenderObject render, ref Position pos) =>
            {
                if (render.gameObject)
                {
                    // Tự động gắn script Debug nếu chưa có
                    var debugger = render.gameObject.GetComponent<EntityDebugger>();
                    if (debugger == null)
                    {
                        debugger = render.gameObject.AddComponent<EntityDebugger>();
                        debugger.entity = entity;
                        debugger.world = world;
                    }

                    // Đồng bộ vị trí từ ECS sang Transform của GameObject
                    render.gameObject.transform.position = pos.value;

                    // Đồng bộ tỉ lệ (Scale) nếu có
                    if (entity.Has<Scale>())
                    {
                        float s = entity.Get<Scale>().value;
                        float sy = s;
                        if (entity.Has<GravityFlipComponent>())
                        {
                            sy *= entity.Get<GravityFlipComponent>().multiplier;
                        }
                        render.gameObject.transform.localScale = new Vector3(s, sy, s);
                    }

                    // Đồng bộ Rotation nếu có
                    if (entity.Has<Rotation>())
                    {
                        render.gameObject.transform.localRotation = Quaternion.Euler(0, 0, entity.Get<Rotation>().value);
                    }

                    // Hiệu ứng nhấp nháy cho vật thể đang tàng hình/bất tử
                    if (entity.Has<InvincibleTag>())
                    {
                        // Nhấp nháy theo thời gian thực (Unity Time)
                        float alpha = (Mathf.Sin(Time.time * 20f) + 1f) / 2f;
                        var sr = render.gameObject.GetComponentInChildren<SpriteRenderer>();
                        if (sr != null)
                        {
                            var color = sr.color;
                            color.a = alpha > 0.5f ? 1f : 0.3f; // Nhấp nháy nhanh
                            sr.color = color;
                        }
                    }
                    else
                    {
                        // Reset độ trong suốt nếu hết bất tử
                        var sr = render.gameObject.GetComponentInChildren<SpriteRenderer>();
                        if (sr != null && sr.color.a < 1f)
                        {
                            var color = sr.color;
                            color.a = 1f;
                            sr.color = color;
                        }
                    }

                    // Nếu thực thể có Rotation thì mới đồng bộ xoay
                    if (entity.Has<Rotation>())
                    {
                        ref var rot = ref entity.Get<Rotation>();
                        Quaternion targetRot = Quaternion.Euler(0, 0, rot.value);
                        render.gameObject.transform.rotation = Quaternion.Lerp(render.gameObject.transform.rotation, targetRot, deltaTime * 5f);
                    }
                }
            });
        }
        
        public IEnumerable<System.Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(RenderObject), typeof(Position) };
        }
    }
}