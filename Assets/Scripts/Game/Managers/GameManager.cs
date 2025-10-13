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

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            SetState(GameState.Start);
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState);
        }

        public void StartGame()
        {
            SetState(GameState.Playing);
            AudioManager.Instance.PlayBGM(true);
        }

        public void GameOver()
        {
            SetState(GameState.Lose);
            AudioManager.Instance.PlayBGM(false);
        }

        public void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}