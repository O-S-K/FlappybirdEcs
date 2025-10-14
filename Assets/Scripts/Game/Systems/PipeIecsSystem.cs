using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlappyECS
{
    public class PipeIecsSystem : IECSSystem, IQueryDebugInfo
    {
        private int nextPairId = 0;
        public float spawnTimer = 0f; // bộ đếm thời gian spawn

        private GameEntry gameEntry;
        private World world;

        public PipeIecsSystem(GameEntry gameEntry, World world)
        {
            this.world = world;
            this.gameEntry = gameEntry;
        }

        public void OnUpdate(World world, float deltaTime)
        {
            if (GameManager.Instance.CurrentState != GameState.Playing)
                return;

            CheckAndSpawn(deltaTime);
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