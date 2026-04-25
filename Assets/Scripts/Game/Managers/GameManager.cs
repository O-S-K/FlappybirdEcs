using UnityEngine;

namespace FlappyECS
{
    public enum GameState
    {
        Start,
        Playing,
        Lose
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GameState CurrentState { get; private set; } = GameState.Start;
        public System.Action<GameState> OnStateChanged;

        private BlitzEcs.World ecsWorld;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            SetState(GameState.Start);
        }

        public void Init(BlitzEcs.World world)
        {
            this.ecsWorld = world;
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;
            
            // Cập nhật State vào ECS World nếu có
            if (ecsWorld != null)
            {
                var stateQuery = new BlitzEcs.Query<GameStateComponent>(ecsWorld);
                stateQuery.ForEach((ref GameStateComponent state) => state.value = newState);
            }

            OnStateChanged?.Invoke(CurrentState);
        }

        public void StartGame()
        {
            SetState(GameState.Playing);

            // Cập nhật cả GameSpeed vào ECS World
            if (ecsWorld != null)
            {
                var configQuery = new BlitzEcs.Query<GameConfigComponent>(ecsWorld);
                configQuery.ForEach((ref GameConfigComponent config) => config.gameSpeed = GameData.GameSpeed);
            }

            AudioManager.Instance.PlayBGM(true);
        }

        public void GameOver()
        {
            // Chỉ cập nhật nếu chưa ở trạng thái Lose
            if (CurrentState == GameState.Lose) return;

            CurrentState = GameState.Lose;
            OnStateChanged?.Invoke(CurrentState);
            AudioManager.Instance.PlayBGM(false);
        }

        public void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}