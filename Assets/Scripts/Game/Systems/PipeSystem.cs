using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlappyECS
{
    public class PipeSystem : ISystem, IQueryDebugInfo
    {
        private int nextPairId = 0;
        public float spawnTimer = 0f; // bộ đếm thời gian spawn

        private GameEntry gameEntry;
        private World world;

        public PipeSystem(GameEntry gameEntry, World world)
        {
            this.world = world;
            this.gameEntry = gameEntry;
        }

        public void OnUpdate(World world, float deltaTime)
        {
            if (GameManager.Instance.CurrentState != GameState.Playing)
                return;

#if UNITY_EDITOR
            BlitzEcs.EcsProfiler.Begin(this);
            CheckAndSpawn(deltaTime);
            BlitzEcs.EcsProfiler.End(this);
#else
            CheckAndSpawn(deltaTime);
#endif
        }

        private void CheckAndSpawn(float deltaTime)
        {
            spawnTimer += deltaTime;
            if (spawnTimer >= PipeData.spawnInterval)
            {
                spawnTimer = 0f;
                SpawnPipes();
            }
        }

        private void SpawnPipes()
        {
            float topPipeY = Random.Range(PipeData.gapHeight / 2 + 1f, 10f);
            float bottomPipeY = topPipeY - PipeData.gapHeight - PipeData.spaceBetweenPipes;
            Entity pipeTop = gameEntry.CreatePipe(new Vector2(10f, topPipeY), nextPairId);
            Entity pipeBottom = gameEntry.CreatePipe(new Vector2(10f, bottomPipeY), nextPairId);
            nextPairId++;
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return Array.Empty<Type>();
        }
    }
}