using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class CameraShakeSystem : ISystem
    {
        private float duration = 0f;
        private float intensity = 0.1f;
        private Vector3 origin;
        private bool shaking;

        public void OnUpdate(World world, float deltaTime)
        {
            var cam = Camera.main;
            if (cam == null) return;

            // nếu không shake thì bỏ qua
            if (!shaking) return;

            if (duration > 0f)
            {
                cam.transform.localPosition = origin + Random.insideUnitSphere * intensity;
                duration -= deltaTime;
            }
            else
            {
                // trả camera về vị trí gốc
                cam.transform.localPosition = origin;
                shaking = false;
            }
        }

        public void Shake(float time = 0.2f, float strength = 0.15f)
        {
            var cam = Camera.main;
            if (cam == null) return;

            origin = cam.transform.localPosition;
            duration = time;
            intensity = strength;
            shaking = true;
        }
    }
}