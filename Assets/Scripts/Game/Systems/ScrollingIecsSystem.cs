using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;

namespace FlappyECS
{
    public class ScrollingIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private Query<Position, Velocity, ObstacleTag> obstacleQuery;
        private Query<Position, Velocity, ItemTag> itemQuery;
        private Query<GameStateComponent> stateQuery;
        private Query<GameConfigComponent> configQuery;
        
        public void OnCreate(World world)
        {
            obstacleQuery = new Query<Position, Velocity, ObstacleTag>(world);
            itemQuery = new Query<Position, Velocity, ItemTag>(world);
            stateQuery = new Query<GameStateComponent>(world);
            configQuery = new Query<GameConfigComponent>(world);
        }
        
        public void OnUpdate(World world, float deltaTime)
        {
            GameState currentState = GameState.Start;
            stateQuery.ForEach((ref GameStateComponent state) => currentState = state.value);

            if (currentState != GameState.Playing)
                return;

            float speedMultiplier = 1f;
            configQuery.ForEach((ref GameConfigComponent config) => speedMultiplier = config.gameSpeed);

            // Kiểm tra hiệu ứng Làm chậm thời gian (Time Warp)
            var warpQuery = new Query<TimeWarpComponent>(world);
            warpQuery.ForEach((ref TimeWarpComponent warp) => speedMultiplier *= warp.speedMultiplier);

            // Di chuyển Vật cản (Ống, Cưa, Gai...)
            obstacleQuery.ForEach((ref Position position, ref Velocity velocity, ref ObstacleTag obstacle) =>
            {
                position.value += velocity.value * (deltaTime * speedMultiplier);  
            });

            // Di chuyển Vật phẩm (Buff Items)
            itemQuery.ForEach((ref Position position, ref Velocity velocity, ref ItemTag itemTag) =>
            {
                position.value += velocity.value * (deltaTime * speedMultiplier);  
            });
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return new[] { typeof(Position), typeof(Velocity), typeof(ObstacleTag) };
            yield return new[] { typeof(Position), typeof(Velocity), typeof(ItemTag) };
            yield return new[] { typeof(GameStateComponent) };
            yield return new[] { typeof(GameConfigComponent) };
        }
    }
}