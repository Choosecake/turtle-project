using System;
using UnityEngine;
using Utilities;

namespace Code
{
    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        [SerializeField] private TurtleVitalSystems playerVitalSystems;

        private float defaultTime = 1f;
        private bool isPaused;

        public Action OnGamePause;
        public Action OnGameResume;

        void Start()
        {
            if (playerVitalSystems == null)
            {
                playerVitalSystems = FindObjectOfType<TurtleVitalSystems>();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !playerVitalSystems.IsDead)
            {
                TogglePause();
            }
        }

        private bool IsPaused
        {
            get => isPaused;
            set
            {
                isPaused = value;
                if (isPaused)  OnGamePause?.Invoke();
                else OnGameResume?.Invoke();
            }
        } 

        public void TogglePause()
        {
            if (!IsPaused)
            {
                defaultTime = Time.timeScale;
                Time.timeScale = 0f;
                IsPaused = true;
            }
            else
            {
                Time.timeScale = defaultTime;
                IsPaused = false;
            }
        }

        public void QuitGame()
        {
            AppHelper.Quit();
        }
    }
}