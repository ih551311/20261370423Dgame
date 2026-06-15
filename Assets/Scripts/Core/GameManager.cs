using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer3D
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Rules")]
        [SerializeField] private float gameDuration = 60f;
        [SerializeField] private int startingScore = 0;

        public GameState CurrentState { get; private set; } = GameState.Playing;
        public GameOverReason LastGameOverReason { get; private set; } = GameOverReason.None;
        public int Score { get; private set; }
        public float RemainingTime { get; private set; }

        public event Action<int> OnScoreChanged;
        public event Action<float> OnTimeChanged;
        public event Action<GameState> OnGameStateChanged;
        public event Action<GameOverReason> OnGameOver;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Score = startingScore;
            RemainingTime = gameDuration;
        }

        private void Start()
        {
            NotifyScoreChanged();
            NotifyTimeChanged();
            NotifyStateChanged();
        }

        private void Update()
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }

            RemainingTime -= Time.deltaTime;
            NotifyTimeChanged();

            if (RemainingTime <= 0f)
            {
                RemainingTime = 0f;
                TriggerGameOver(GameOverReason.TimeOut);
            }
        }

        public void AddScore(int amount)
        {
            if (CurrentState != GameState.Playing || amount <= 0)
            {
                return;
            }

            Score += amount;
            NotifyScoreChanged();
        }

        public void TriggerGameOver(GameOverReason reason)
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }

            CurrentState = GameState.GameOver;
            LastGameOverReason = reason;
            NotifyStateChanged();
            OnGameOver?.Invoke(reason);
        }

        public void TriggerVictory()
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }

            CurrentState = GameState.Victory;
            LastGameOverReason = GameOverReason.None;
            NotifyStateChanged();
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public bool IsPlaying => CurrentState == GameState.Playing;

        private void NotifyScoreChanged()
        {
            OnScoreChanged?.Invoke(Score);
        }

        private void NotifyTimeChanged()
        {
            OnTimeChanged?.Invoke(RemainingTime);
        }

        private void NotifyStateChanged()
        {
            OnGameStateChanged?.Invoke(CurrentState);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}