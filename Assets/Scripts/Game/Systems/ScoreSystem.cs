using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class ScoreSystem : ISystem, IQueryDebugInfo
    {
        private World world;
        private Query<ScoreTag> query;
        public static System.Action<int> OnScoreChanged;

        public ScoreSystem(World world)
        {
            this.world = world;
            query = new Query<ScoreTag>(this.world);
        }

        public void AddScore(int amount)
        {
            query.ForEach((ref ScoreTag score) =>
            {
                score.value += amount;
                ScoreData.AddScore(amount);
                OnScoreChanged?.Invoke(score.value);
            });
        }

        public void OnUpdate(World world, float deltaTime)
        {
            
        }
        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(ScoreTag) };
        }
    }
}