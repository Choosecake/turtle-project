using System;
using UI;
using UnityEngine;
using Utilities;

namespace Code
{
    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        public GameObject[] gameEnders;
        public bool CanPause = true;

        private float defaultTime = 1f;
        private bool isPaused;

        public Action OnGamePause;
        public Action OnGameResume;

        private void OnEnable()
        {
            Array.ForEach(gameEnders, g =>
                g.GetComponent<GameEnder>().OnCriticalPointReached += DisablePause);
        }
        
        private void OnDisable()
        {
            Array.ForEach(gameEnders, g =>
                g.GetComponent<GameEnder>().OnCriticalPointReached -= DisablePause);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && CanPause)
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

        public void DisablePause()
        {
            CanPause = false;
        }
    }
}