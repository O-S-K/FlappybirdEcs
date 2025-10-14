using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class CollisionIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private HashSet<int> scoredPairs = new HashSet<int>(); // ✅ lưu các cặp đã tính điểm, tránh tính điểm nhiều lần
        private Query<Position, ColliderRect, BirdTag> birdQuery;
        private Query<Position, ColliderRect, PipeTag> pipeQuery;

        public void OnCreate(World world)
        {
            birdQuery = new Query<Position, ColliderRect, BirdTag>(world);
            pipeQuery = new Query<Position, ColliderRect, PipeTag>(world);
        }

        public void OnUpdate(World world, float deltaTime)
        {
            if (GameManager.Instance.CurrentState != GameState.Playing)
                return;
            CheckCollisions(world);
        }

        private void CheckCollisions(World world)
        {
            birdQuery.ForEach((ref Position bpos, ref ColliderRect bcol, ref BirdTag birdTag) =>
            {
                var bMin = bpos.value - bcol.size / 2;
                var bMax = bpos.value + bcol.size / 2;
                var birdPos = bpos.value;

                // Kiểm tra va chạm với tất cả các pipe
                pipeQuery.ForEach((ref Position ppos, ref ColliderRect pcol, ref PipeTag pipeTag) =>
                {
                    var pMin = ppos.value - pcol.size / 2;
                    var pMax = ppos.value + pcol.size / 2;
                    var pipePos = ppos.value;

                    bool isPassed = !scoredPairs.Contains(pipeTag.pairId) && birdPos.x > pipePos.x;
                    if (isPassed)
                    {
                        AudioManager.Instance.PlayScore();
                        AddScore(ref pipeTag, world);
                    }

                    // Kiểm tra va chạm AABB
                    if (bMin.x < pMax.x && bMax.x > pMin.x &&
                        bMin.y < pMax.y && bMax.y > pMin.y)
                    {
                        AudioManager.Instance.PlayHit();
                        world.Systems.Get<CameraShakeIecsSystem>().Shake();
                        GameOver();
                    }
                });

                // Kiểm tra va chạm với mặt đất và trần
                if (bMin.y <= -Const.GROUND_Y || bMax.y > Const.GROUND_Y)
                {
                    AudioManager.Instance.PlayDie();
                    GameOver();
                }
            });
        }


        private void AddScore(ref PipeTag pipeTag, World world)
        {
            pipeTag.passed = true;
            scoredPairs.Add(pipeTag.pairId);
            world.Systems.Get<ScoreIecsSystem>().AddScore(Const.SCORE_PER_PASS);
        }

        private void GameOver()
        {
            if (GameManager.Instance.CurrentState == GameState.Lose)
                return;

            Debug.Log("Game Over!");
            AudioManager.Instance.PlayHit();
            GameManager.Instance.GameOver();
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(ColliderRect), typeof(BirdTag) };
            yield return new[] { typeof(Position), typeof(ColliderRect), typeof(PipeTag) };
        }
    }
}