using System;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Code
{
    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        private float defaultTime = 1f;
        private bool isPaused;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public bool IsPaused
        {
            get => isPaused;
            set
            {
                isPaused = value;
                if (value)  OnGamePause?.Invoke();
            }
        } 
        public Action OnGamePause;

        private void TogglePause()
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
    }
}