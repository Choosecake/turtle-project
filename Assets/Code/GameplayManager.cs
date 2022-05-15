using System;
using UnityEngine;
using Utilities;

namespace Code
{
    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        private float defaultTime = 1f;
        private bool isPaused;

        public Action OnGamePause;
        public Action OnGameResume;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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