using UnityEngine;

namespace FlappyECS
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject startUI;
        [SerializeField] private GameObject loseUI;
        [SerializeField] private UIScore scoreUI;

        private void Start()
        {
            GameManager.Instance.OnStateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnStateChanged -= OnStateChanged;
        }
        
        private void Initialize()
        {
            startUI.SetActive(true);
            loseUI.SetActive(false);
            scoreUI.gameObject.SetActive(false);
        }

        private void OnStateChanged(GameState state)
        {
            startUI.SetActive(state == GameState.Start);
            loseUI.SetActive(state == GameState.Lose);
            scoreUI.gameObject.SetActive(state == GameState.Playing);
        }

        public void OnStartEasyModeButton()
        {
            GameData.GameSpeed = 1f;
            AudioManager.Instance.PlayButton();
            GameManager.Instance.StartGame();
        }
        
        public void OnStartHardModeButton()
        {
            GameData.GameSpeed = 2f;
            AudioManager.Instance.PlayButton();
            GameManager.Instance.StartGame();
        }

        public void OnRestartButton()
        {
            AudioManager.Instance.PlayButton();
            GameManager.Instance.Restart();
        }
    }
}