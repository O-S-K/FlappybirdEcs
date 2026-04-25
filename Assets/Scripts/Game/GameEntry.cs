using System;
using System.Linq;
using UnityEngine;
using BlitzEcs;
using Object = UnityEngine.Object;

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
        public GameObject itemPrefab; // Prefab cho vật phẩm (Ngôi sao)

        private World world;

        private void Awake()
        {
            Application.targetFrameRate = 9999;
        }

        private void Start()
        {
            // Tạo world ECS
            world = new World();
            
            // create and add systems
            world.Systems = new EcsSystem(world);
            
            IECSSystem[] entitySystems = {
                new InputIecsSystem(), // Thêm Input System vào đầu
                new BirdIecsSystem(),
                new PipeIecsSystem(this, world),
                new ItemIecsSystem(), // Hệ thống vật phẩm
            };
            
            IECSSystem[] renderSystems = {
                new RenderIecsSystem(),
                new FloatingTextIecsSystem(), // Hệ thống text bay
                new ParallaxIecsSystem()
            };
            
            IECSSystem[] logicSystems = {
                new ScoreIecsSystem(world),
                new CollisionIecsSystem(),
                new ScrollingIecsSystem(),
                new CleanupIecsSystem(),
                new CameraShakeIecsSystem(),
                new ObstacleIecsSystem(), // Xử lý Cưa, Laze...
                new BuffIecsSystem(), // Quản lý Buff/Debuff
                new MagnetIecsSystem(), // Hệ thống nam châm hút vật phẩm
                new SoundIecsSystem(), // Xử lý âm thanh qua Component
            };

            IECSSystem[] infraSystems = {
                new LifecycleIecsSystem(), // Luôn nằm cuối để dọn dẹp thực thể
            };
            
            world.Systems.AddRange(new []
            {
                entitySystems, renderSystems, logicSystems, infraSystems
            }.SelectMany(x => x)).OnCreate();
 
            Debug.Log($"Total Systems: {world.Systems.Systems.Count}");
            
            // create initial entities using Factory API
            EntityFactory.CreateBird(world, birdPrefab, new Vector2(-2, 0));
            
            // Create Ground (Manual or add to factory)
            Entity ground = world.Spawn();
            ground.Add(new Position { value = new Vector2(0, -10) })
                  .Add(new Rotation { value = 0f })
                  .Add(new RenderObject { gameObject = Instantiate(groundPrefab) })
                  .Add(new ColliderRect { size = new Vector2(20f, 1f) })
                  .Add(new GameStateComponent { value = GameState.Start })
                  .Add(new GameConfigComponent { gameSpeed = GameData.GameSpeed });

            EntityFactory.CreateParallax(world, bgPrefab, 1f, 30f, -30f);
            EntityFactory.CreateParallax(world, cloudPrefab, 1.5f, 15f, -15f);
            EntityFactory.CreateParallax(world, hillPrefab, 3f, 20f, -20f);

            // Link GameManager with ECS World
            GameManager.Instance.Init(world);
            
#if UNITY_EDITOR
            BlitzEcs.Editor.EcsDebuggerWindow.Open();
            BlitzEcs.Editor.EcsDebuggerWindow.Instance.Attach(world, world.Systems);
#endif
            Debug.Log("Game Started");
            gameObject.AddComponent<UIAutoSetup>();
        }

        private void Update()
        {
            world.Systems.OnUpdate(Time.deltaTime);
        }
        
        private void OnDestroy()
        {
            world.Systems.OnDestroy();
        }
    }
}