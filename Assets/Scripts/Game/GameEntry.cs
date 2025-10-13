using System.Linq;
using UnityEngine;
using BlitzEcs;

namespace FlappyECS
{
    public class GameEntry : MonoBehaviour
    {
        // Prefabs for entities
        public GameObject birdPrefab;
        public GameObject pipePrefab;
        public GameObject groundPrefab;
        
        // Background parallax
        public GameObject cloudPrefab;
        public GameObject hillPrefab;
        public GameObject bgPrefab;

        private World world;

        private void Start()
        {
            // Tạo world ECS
            world = new World();
            
            // create and add systems
            world.Systems = new EcsSystem(world);
            
            ISystem[] entitySystems = {
                new BirdSystem(),
                new PipeSystem(this, world),
            };
            
            ISystem[] renderSystems = {
                new RenderSystem(),
                new ParallaxSystem()
            };
            
            ISystem[] logicSystems = {
                new ScoreSystem(world),
                new CollisionSystem(),
                new PipeMovementSystem(),
                new PipeDestroySystem(),
                new CameraShakeSystem(),
            };
            
            world.Systems.AddRange(new []
            {
                entitySystems, renderSystems, logicSystems
            }.SelectMany(x => x)).OnCreate();
            
            Debug.Log($"Total Systems: {world.Systems.Systems.Count}");
            
            // create initial entities
            CreateBird(new Vector2(-2, 0));
            CreateGround(new Vector2(0, -10));
            CreateBGParallax();
            
#if UNITY_EDITOR
            BlitzEcs.Editor.EcsDebuggerWindow.Open();
            BlitzEcs.Editor.EcsDebuggerWindow.Instance.Attach(world, world.Systems);
#endif
            Debug.Log("Game Started");
        }
        
        private Entity CreateBird(Vector2 position)
        {
            Entity bird = world.Spawn();
            bird.Add(new Position { value = position });
            bird.Add(new Rotation { value = 0f });
            bird.Add(new Velocity { value = Vector2.zero });
            bird.Add(new Gravity { value = -9.8f });
            bird.Add(new RenderObject { gameObject = Object.Instantiate(birdPrefab) });
            bird.Add(new ColliderRect { size = new Vector2(1f, 1f) });
            bird.Add(new BirdTag());
            bird.Add(new ScoreTag { value = 0 });
            return bird;
        }
        private Entity CreateGround(Vector2 position)
        {
            Entity ground = world.Spawn();
            ground.Add(new Position { value = position });
            ground.Add(new RenderObject { gameObject = Object.Instantiate(groundPrefab) });
            ground.Add(new ColliderRect { size = new Vector2(20f, 1f) });
            return ground;
        }
        public Entity CreatePipe(Vector2 position, int nextPairId)
        {
            Entity pipe = world.Spawn();
            pipe.Add(new Position { value = position });
            pipe.Add(new Rotation { value = 0f });
            pipe.Add(new Velocity { value = new Vector2(-2f, 0) });
            pipe.Add(new RenderObject { gameObject = Object.Instantiate(pipePrefab) });
            pipe.Add(new ColliderRect { size = new Vector2(1f, 10f) });
            pipe.Add(new PipeTag { passed = false, pairId = nextPairId });
            return pipe;
        }

        private void CreateBGParallax()
        {
            CreateParallax(bgPrefab, speed: 1f, startX: 30f, resetX: -30f);
            CreateParallax(cloudPrefab, speed: 1.5f, startX: 15f, resetX: -15f);
            CreateParallax(hillPrefab, speed: 3f, startX: 20f, resetX: -20f);

            void CreateParallax(GameObject prefab, float speed, float startX, float resetX)
            {
                Entity bg = world.Spawn();

                var go = Instantiate(prefab);
                var pos = go.transform.position;

                bg.Add(new Position { value = new Vector2(pos.x, pos.y) })
                    .Add(new RenderObject { gameObject = go })
                    .Add(new ParallaxTag { speed = speed, startX = startX, resetX = resetX });
                Debug.Log($"Created Parallax Entity with speed {speed}");
            }
        }
        

        private void Update()
        {
            float deltaTime = Time.deltaTime * GameData.GameSpeed; 
            world.Systems.OnUpdate(deltaTime);
        }
        
        private void OnDestroy()
        {
            world.Systems.OnDestroy();
        }
    }
}