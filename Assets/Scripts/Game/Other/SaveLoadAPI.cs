using System;
using System.Collections.Generic;
using BlitzEcs;
using UnityEngine;
using System.IO;

namespace FlappyECS
{
    [Serializable]
    public class GameDataSnapshot
    {
        public Vector2 birdPosition;
        public Vector2 birdVelocity;
        public int score;
        public List<ObstacleSnapshot> obstacles = new List<ObstacleSnapshot>();
    }

    [Serializable]
    public class ObstacleSnapshot
    {
        public Vector2 position;
        public int pairId;
        public ObstacleType type;
    }

    public static class SaveLoadAPI
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

        public static void SaveGame(World world)
        {
            var snapshot = new GameDataSnapshot();

            // Lấy dữ liệu Bird
            var birdQuery = new Query<Position, Velocity, BirdTag>(world);
            birdQuery.ForEach((ref Position pos, ref Velocity vel, ref BirdTag tag) =>
            {
                snapshot.birdPosition = pos.value;
                snapshot.birdVelocity = vel.value;
            });

            // Lấy Score
            var scoreQuery = new Query<ScoreTag>(world);
            scoreQuery.ForEach((ref ScoreTag score) =>
            {
                snapshot.score = score.value;
            });

            // Lấy danh sách Vật cản
            var obstacleQuery = new Query<Position, ObstacleTag>(world);
            obstacleQuery.ForEach((ref Position pos, ref ObstacleTag tag) =>
            {
                snapshot.obstacles.Add(new ObstacleSnapshot { 
                    position = pos.value, 
                    pairId = tag.pairId,
                    type = tag.type
                });
            });

            string json = JsonUtility.ToJson(snapshot, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"Game Saved to {SavePath}");
        }

        public static void LoadGame(World world, GameObject birdPrefab, GameObject pipePrefab)
        {
            if (!File.Exists(SavePath)) return;

            string json = File.ReadAllText(SavePath);
            var snapshot = JsonUtility.FromJson<GameDataSnapshot>(json);

            // 1. Dọn dẹp các thực thể hiện tại (Bird, Obstacles)
            var cleanQuery = new Query<BirdTag>(world);
            cleanQuery.ForEach((Entity e, ref BirdTag t) => e.Add(new DestroyTag()));
            
            var cleanObstacleQuery = new Query<ObstacleTag>(world);
            cleanObstacleQuery.ForEach((Entity e, ref ObstacleTag t) => e.Add(new DestroyTag()));

            // 2. Re-spawn Bird
            EntityFactory.CreateBird(world, birdPrefab, snapshot.birdPosition);
            
            // Cập nhật lại Velocity cho Bird
            var birdQuery = new Query<Velocity, BirdTag>(world);
            birdQuery.ForEach((ref Velocity vel, ref BirdTag tag) => vel.value = snapshot.birdVelocity);

            // 3. Re-spawn Obstacles
            foreach (var obs in snapshot.obstacles)
            {
                EntityFactory.CreateObstacle(world, pipePrefab, obs.position, obs.pairId, obs.type);
            }

            // 4. Cập nhật Score
            var scoreQuery = new Query<ScoreTag>(world);
            scoreQuery.ForEach((ref ScoreTag score) => score.value = snapshot.score);

            Debug.Log("Game Loaded");
        }
    }
}
