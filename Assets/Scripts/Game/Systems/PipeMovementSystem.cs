using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class PipeMovementSystem : ISystem, IQueryDebugInfo
    {
        public void OnUpdate(World world, float deltaTime)
        {
            var pipe = new Query<Position, Velocity, PipeTag>(world);
            pipe.ForEach((ref Position position, ref Velocity velocity, ref PipeTag pipeTag) =>
            {
                if(GameManager.Instance.CurrentState != GameState.Playing) 
                    return;
                position.value += velocity.value * deltaTime;  
            });
            
            var bird = new Query<Position, Velocity, BirdTag>(world);
            bird.ForEach((ref Position position, ref Velocity velocity, ref BirdTag birdTag) =>
            {
                if(GameManager.Instance.CurrentState == GameState.Start)
                {
                    position.value.y = 0.5f + Mathf.Sin(Time.time * 2f) * 0.5f;
                }
                else if (GameManager.Instance.CurrentState == GameState.Playing)
                {
                    position.value += velocity.value * deltaTime;
                }  
                else if(GameManager.Instance.CurrentState == GameState.Lose)
                {
                    position.value += new UnityEngine.Vector2(0, -9.8f) * deltaTime;
                }
            });

        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(Velocity), typeof(PipeTag) };
            yield return new[] { typeof(Position), typeof(Velocity), typeof(BirdTag) };
        }
    }
}