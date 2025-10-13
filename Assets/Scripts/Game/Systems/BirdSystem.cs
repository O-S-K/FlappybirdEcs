using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class BirdSystem : ISystem, IQueryDebugInfo
    {
        public void OnUpdate(World world, float deltaTime)
        {
            if(GameManager.Instance.CurrentState != GameState.Playing) 
                return;
            
            var query = new Query<Position, Rotation, Velocity, Gravity, BirdTag>(world);
            query.ForEach((ref Position position,ref Rotation rotation, ref Velocity velocity, ref Gravity gravity, ref BirdTag birdTag) =>
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    velocity.value.y = BirdData.jumpForce;
                    AudioManager.Instance.PlayJump();
                }
                velocity.value += new Vector2(0, gravity.value) * deltaTime;
                position.value += velocity.value * deltaTime;

                float t = Mathf.InverseLerp(gravity.value, BirdData.jumpForce, velocity.value.y * 2f);
                float targetAngle = Mathf.Lerp(-80f, BirdData.rotationFace, t);
                rotation.value = Mathf.Lerp(rotation.value, targetAngle, deltaTime * 5f);
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(Rotation), typeof(Velocity), typeof(Gravity), typeof(BirdTag) };
        }
    }
}