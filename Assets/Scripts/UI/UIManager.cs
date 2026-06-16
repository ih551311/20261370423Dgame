using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer3D
{
    public class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private string scoreFormat = "Score: {0}";
        [SerializeField] private string timerFormat = "Time: {0:00}:{1:00}";

        [Header("Result Panel")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TMP_Text resultTitleText;
        [SerializeField] private TMP_Text resultMessageText;
        [SerializeField] private Button restartButton;

        [Header("Messages")]
        [SerializeField] private string victoryTitle = "GOOOOOOOOOOOOOOOOD";
        [SerializeField] private string victoryMessage = "Goal reached!";
        [SerializeField] private string timeOutTitle = "Game Over";
        [SerializeField] private string timeOutMessage = "Time is up.";
        [SerializeField] private string fallTitle = "YOU DIE";
        [SerializeField] private string fallMessage = "You fell off the map.";

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                Subscribe(GameManager.Instance);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                Subscribe(GameManager.Instance);
                RefreshAll(GameManager.Instance);
            }

            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                Unsubscribe(GameManager.Instance);
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(OnRestartClicked);
            }
        }

        private void Subscribe(GameManager manager)
        {
            manager.OnScoreChanged -= HandleScoreChanged;
            manager.OnTimeChanged -= HandleTimeChanged;
            manager.OnGameStateChanged -= HandleGameStateChanged;
            manager.OnGameOver -= HandleGameOver;

            manager.OnScoreChanged += HandleScoreChanged;
            manager.OnTimeChanged += HandleTimeChanged;
            manager.OnGameStateChanged += HandleGameStateChanged;
            manager.OnGameOver += HandleGameOver;
        }

        private void Unsubscribe(GameManager manager)
        {
            manager.OnScoreChanged -= HandleScoreChanged;
            manager.OnTimeChanged -= HandleTimeChanged;
            manager.OnGameStateChanged -= HandleGameStateChanged;
            manager.OnGameOver -= HandleGameOver;
        }

        private void RefreshAll(GameManager manager)
        {
            HandleScoreChanged(manager.Score);
            HandleTimeChanged(manager.RemainingTime);
            HandleGameStateChanged(manager.CurrentState);
        }

        private void HandleScoreChanged(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = string.Format(scoreFormat, score);
            }
        }

        private void HandleTimeChanged(float remainingTime)
        {
            if (timerText == null)
            {
                return;
            }

            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = string.Format(timerFormat, minutes, seconds);
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.Victory)
            {
                ShowResult(victoryTitle, victoryMessage);
            }
        }

        private void HandleGameOver(GameOverReason reason)
        {
            switch (reason)
            {
                case GameOverReason.TimeOut:
                    ShowResult(timeOutTitle, timeOutMessage);
                    break;
                case GameOverReason.Fall:
                    ShowResult(fallTitle, fallMessage);
                    break;
            }
        }

        private void ShowResult(string title, string message)
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(true);
            }

            if (resultTitleText != null)
            {
                resultTitleText.text = title;
            }

            if (resultMessageText != null)
            {
                resultMessageText.text = message;
            }
        }

        private void OnRestartClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }
    }
}