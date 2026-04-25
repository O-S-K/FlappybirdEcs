using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class ScoreIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private World world;
        private Query<ScoreTag> query;
        public static System.Action<int> OnScoreChanged;

        public ScoreIecsSystem(World world)
        {
            this.world = world;
            query = new Query<ScoreTag>(this.world);
        }

        public void OnUpdate(World world, float deltaTime)
        {
            // Xử lý các yêu cầu cộng điểm
            var requestQuery = new Query<AddScoreRequest>(world);
            requestQuery.ForEach((Entity entity, ref AddScoreRequest request) =>
            {
                ApplyScore(request.amount);
                entity.Remove<AddScoreRequest>();
            });
        }

        private void ApplyScore(int amount)
        {
            // Kiểm tra xem chim có đang được x2 điểm không
            int finalAmount = amount;
            var birdQuery = new Query<ScoreMultiplierComponent>(world);
            birdQuery.ForEach((ref ScoreMultiplierComponent mult) => {
                finalAmount *= mult.multiplier;
            });

            query.ForEach((ref ScoreTag score) =>
            {
                score.value += finalAmount;
                ScoreData.AddScore(finalAmount);
                OnScoreChanged?.Invoke(score.value);
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(ScoreTag) };
            yield return new[] { typeof(AddScoreRequest) };
        }
    }
}