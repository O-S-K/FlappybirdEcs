using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class CollisionIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private HashSet<int> scoredPairs = new HashSet<int>(); 
        private Query<Position, ColliderRect, BirdTag> birdQuery;
        private Query<Position, ColliderRect, ObstacleTag> obstacleQuery;
        private Query<GameStateComponent> stateQuery;

        public void OnCreate(World world)
        {
            birdQuery = new Query<Position, ColliderRect, BirdTag>(world);
            obstacleQuery = new Query<Position, ColliderRect, ObstacleTag>(world);
            stateQuery = new Query<GameStateComponent>(world);
        }

        public void OnUpdate(World world, float deltaTime)
        {
            GameState currentState = GameState.Start;
            stateQuery.ForEach((ref GameStateComponent state) => currentState = state.value);

            if (currentState != GameState.Playing)
                return;

            CheckCollisions(world);
        }

        private void CheckCollisions(World world)
        {
            birdQuery.ForEach((Entity birdEntity, ref Position bpos, ref ColliderRect bcol, ref BirdTag birdTag) =>
            {
                var bMin = bpos.value - bcol.size / 2;
                var bMax = bpos.value + bcol.size / 2;
                var birdPos = bpos.value;

                // Kiểm tra va chạm với Vật cản (Pipe, Saw, Spike, Laser...)
                obstacleQuery.ForEach((ref Position ppos, ref ColliderRect pcol, ref ObstacleTag obstacle) =>
                {
                    var pMin = ppos.value - pcol.size / 2;
                    var pMax = ppos.value + pcol.size / 2;
                    var obstaclePos = ppos.value;

                    // Chỉ cộng điểm khi vượt qua Cặp Ống (Pipe)
                    bool isPassed = obstacle.type == ObstacleType.Pipe && !scoredPairs.Contains(obstacle.pairId) && birdPos.x > obstaclePos.x;
                    if (isPassed)
                    {
                        birdEntity.Add(new AddScoreRequest { amount = Const.SCORE_PER_PASS });
                        scoredPairs.Add(obstacle.pairId);
                        obstacle.passed = true;
                        AudioManager.Instance.PlayScore();
                    }

                    // Chỉ kiểm tra va chạm nếu vật cản có kích thước (tránh laser đang tắt)
                    if (pcol.size.x > 0 && pcol.size.y > 0 &&
                        bMin.x < pMax.x && bMax.x > pMin.x &&
                        bMin.y < pMax.y && bMax.y > pMin.y)
                    {
                        // Nếu có GhostComponent thì xuyên qua, không chết
                        if (birdEntity.Has<GhostComponent>())
                            return;

                        // Kiểm tra tàng hình/bất tử
                        if (birdEntity.Has<InvincibleTag>())
                        {
                            // Đi xuyên qua vật cản, không chết
                            return; 
                        }

                        // Kiểm tra Giáp (Shield)
                        if (birdEntity.Has<ShieldComponent>())
                        {
                            ref var shield = ref birdEntity.Get<ShieldComponent>();
                            shield.charges--;
                            if (shield.charges <= 0) birdEntity.Remove<ShieldComponent>();

                            // Cho bất tử tạm thời (2s) để vượt qua ống
                            if (!birdEntity.Has<InvincibleTag>()) birdEntity.Add(new InvincibleTag());
                            if (!birdEntity.Has<BuffComponent>())
                                birdEntity.Add(new BuffComponent { remainingTime = 2.0f });
                            else
                                birdEntity.Get<BuffComponent>().remainingTime = Mathf.Max(birdEntity.Get<BuffComponent>().remainingTime, 2.0f);

                            EntityFactory.CreateFloatingText(world, "SHIELD USED!", birdPos + Vector2.up * 1.5f, Color.red);
                            AudioManager.Instance.PlayHit();
                            return;
                        }

                        // Va chạm thật sự -> Chết
                        TriggerGameOver(world);
                    }
                });

                // Kiểm tra va chạm với mặt đất/trần
                if (bMin.y <= -Const.GROUND_Y || bMax.y > Const.GROUND_Y)
                {
                    if (!birdEntity.Has<InvincibleTag>())
                    {
                        AudioManager.Instance.PlayDie();
                        TriggerGameOver(world);
                    }
                }
            });
        }

        private void TriggerGameOver(World world)
        {
            stateQuery.ForEach((ref GameStateComponent state) => {
                if(state.value != GameState.Lose) {
                    state.value = GameState.Lose;
                    Debug.Log("Game Over detected by ECS!");
                    AudioManager.Instance.PlayHit();
                    world.Systems.Get<CameraShakeIecsSystem>().Shake();
                    
                    // Thông báo cho GameManager Mono để cập nhật UI
                    GameManager.Instance.GameOver(); 
                }
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(ColliderRect), typeof(BirdTag) };
            yield return new[] { typeof(Position), typeof(ColliderRect), typeof(ObstacleTag) };
            yield return new[] { typeof(GameStateComponent) };
        }
    }
}