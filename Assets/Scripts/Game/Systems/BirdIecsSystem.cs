using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class BirdIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private Query<Position, Rotation, Velocity, Gravity, BirdTag> query;
        private Query<GameStateComponent> stateQuery;
        private Query<InputComponent> inputQuery;

        public void OnCreate(World world)
        {
            query = new Query<Position, Rotation, Velocity, Gravity, BirdTag>(world);
            stateQuery = new Query<GameStateComponent>(world);
            inputQuery = new Query<InputComponent>(world);
        }

        public void OnUpdate(World world, float deltaTime)
        {
            GameState currentState = GameState.Start;
            stateQuery.ForEach((ref GameStateComponent state) => currentState = state.value);

            bool jumpPressed = false;
            inputQuery.ForEach((ref InputComponent input) => jumpPressed = input.jumpPressed);

            query.ForEach((Entity entity, ref Position position, ref Rotation rotation, ref Velocity velocity, ref Gravity gravity, ref BirdTag birdTag) =>
            {
                float gravMult = 1f;
                if (entity.Has<GravityFlipComponent>()) gravMult = entity.Get<GravityFlipComponent>().multiplier;

                if (currentState == GameState.Start)
                {
                    // Hiệu ứng bay dập dìu lúc bắt đầu
                    position.value.y = 0.5f + Mathf.Sin(Time.time * 3f) * 0.2f;
                }
                else if (currentState == GameState.Playing)
                {
                    if (jumpPressed)
                    {
                        velocity.value.y = BirdData.jumpForce * gravMult;
                        AudioManager.Instance.PlayJump();
                    }
                    velocity.value += new Vector2(0, gravity.value * gravMult) * deltaTime;
                    position.value += velocity.value * deltaTime;

                    // Xoay chim: Nếu trọng lực ngược thì quay ngược lại
                    float t = Mathf.InverseLerp(gravity.value * gravMult, BirdData.jumpForce * gravMult, velocity.value.y * 2f);
                    float targetAngle = Mathf.Lerp(-80f * gravMult, BirdData.rotationFace * gravMult, t);
                    rotation.value = Mathf.Lerp(rotation.value, targetAngle, deltaTime * 5f);
                }
                else if (currentState == GameState.Lose)
                {
                    // Rơi tự do khi thua
                    velocity.value += new Vector2(0, -20f) * deltaTime; 
                    position.value += velocity.value * deltaTime;
                    rotation.value = Mathf.Lerp(rotation.value, -90f, deltaTime * 10f);
                }
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(Rotation), typeof(Velocity), typeof(Gravity), typeof(BirdTag) };
            yield return new[] { typeof(GameStateComponent) };
            yield return new[] { typeof(InputComponent) };
        }
    }
}