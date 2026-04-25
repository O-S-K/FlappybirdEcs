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
            float rand = Random.value;
            float topPipeY = Random.Range(PipeData.gapHeight / 2 + 1f, 5f);
            float bottomPipeY = topPipeY - PipeData.gapHeight - PipeData.spaceBetweenPipes;
            
            bool isSpecial = Random.value < 0.2f; // 20% cơ hội ra vật cản đặc biệt
            string specialType = "";
            if (isSpecial)
            {
                float r = Random.value;
                if (r < 0.4f) specialType = "Moving";
                else if (r < 0.7f) specialType = "Pulse";
                else specialType = "Troll";
            }

            if (rand < 0.6f) // 60% Pipe pair
            {
                if (isSpecial && specialType == "Moving") {
                    EntityFactory.CreateSpecialObstacle(world, gameEntry.pipePrefab, new Vector2(10f, topPipeY), nextPairId, ObstacleType.Pipe, "Moving");
                    EntityFactory.CreateSpecialObstacle(world, gameEntry.pipePrefab, new Vector2(10f, bottomPipeY), nextPairId, ObstacleType.Pipe, "Moving");
                } else {
                    EntityFactory.CreateObstacle(world, gameEntry.pipePrefab, new Vector2(10f, topPipeY), nextPairId, ObstacleType.Pipe);
                    EntityFactory.CreateObstacle(world, gameEntry.pipePrefab, new Vector2(10f, bottomPipeY), nextPairId, ObstacleType.Pipe);
                }
            }
            else if (rand < 0.75f) // 15% Saw
            {
                if (isSpecial)
                    EntityFactory.CreateSpecialObstacle(world, gameEntry.pipePrefab, new Vector2(10f, Random.Range(-3f, 3f)), nextPairId, ObstacleType.Saw, specialType);
                else
                    EntityFactory.CreateObstacle(world, gameEntry.pipePrefab, new Vector2(10f, Random.Range(-3f, 3f)), nextPairId, ObstacleType.Saw);
            }
            else 
            {
                EntityFactory.CreateObstacle(world, gameEntry.pipePrefab, new Vector2(10f, Random.Range(-4f, 0f)), nextPairId, ObstacleType.Spike);
                EntityFactory.CreateObstacle(world, gameEntry.pipePrefab, new Vector2(10f, Random.Range(0f, 4f)), nextPairId, ObstacleType.Spike);
            }
         
            // Spawn Item ở giữa 2 ống (hoặc vị trí ngẫu nhiên nếu không phải ống)
            if (Random.value < 0.35f)
            {
                float gapCenterY = topPipeY - (PipeData.spaceBetweenPipes / 2f) - (PipeData.gapHeight / 2f);
                ItemType randomType = (ItemType)Random.Range(0, 7);
                EntityFactory.CreateItem(world, gameEntry.itemPrefab, new Vector2(12f, gapCenterY), randomType);
            }

            nextPairId++;
        }

        public IEnumerable<Type[]> GetQuerySignatures()
        {
            yield return Array.Empty<Type>();
        }
    }
}