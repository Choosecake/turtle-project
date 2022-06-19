using System;
using System.Collections.Generic;
using Code.Utilities;
using TMPro;
using UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Code
{
    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        [SerializeField] private GameObject playerGameObject;
        [SerializeField] private TransitionScreen victoryTransitionScreen;
        [SerializeField] private GameObject musicPlayerGameObject;
        [SerializeField] private string musicPlayerTag = "MusicPlayer";
        [SerializeField] private GameObject sharkBoundaryGameObject;
        [SerializeField] private string sharkBoundaryTag = "Boundary";

        private List<GameEnder> gameEnders;
        private bool CanPause = true;

        private float defaultTime = 1f;
        private bool isPaused;

        public Action OnGamePause;
        public Action OnGameResume;

        public TransitionScreen VictoryTransitionScreen => victoryTransitionScreen;
        public GameObject PlayerGameObject => playerGameObject;
        public GameObject MusicPlayerGameObject 
        {
            get
            {
                MonoUtilities.TryFindWithTag(out musicPlayerGameObject, musicPlayerTag);
                return musicPlayerGameObject;
            }
        }
        public GameObject SharkBoundaryGameObject 
        {
            get
            {
                MonoUtilities.TryFindWithTag(out sharkBoundaryGameObject, sharkBoundaryTag);
                return sharkBoundaryGameObject;
            }
        }

        private void Awake()
        {
            gameEnders = MonoUtilities.FindGameObjects<GameEnder>();
            
            MonoUtilities.TryFindWithTag(out musicPlayerGameObject, musicPlayerTag);
            MonoUtilities.TryFindWithTag(out sharkBoundaryGameObject, sharkBoundaryTag);
        }
        
        private void OnEnable()
        {
            gameEnders.ForEach(gE => gE.OnCriticalPointReached += _ => DisablePause());
            
            Time.timeScale = defaultTime;

        }
        
        private void OnDisable()
        {
            gameEnders.ForEach(gE => gE.OnCriticalPointReached -= _ => DisablePause());
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
            // AppHelper.Quit();
            SceneManager.LoadScene(0);
        }

        public void DisablePause()
        {
            CanPause = false;
        }

        public void GoToNextScene()
        {
            var currenSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log(currenSceneIndex.ToString());
            Debug.Log(SceneManager.sceneCountInBuildSettings.ToString());
            var nextSceneIndex = (currenSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
            Debug.Log(nextSceneIndex.ToString());
            SceneManager.LoadScene(nextSceneIndex);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(0);
        }

        public bool IsTheLastScene()
        {
            return SceneManager.sceneCountInBuildSettings == SceneManager.GetActiveScene().buildIndex + 1;
        }
    }
}