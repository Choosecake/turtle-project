using System;
using Code;
using UnityEngine;
using static UI.ActionExtensions;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject gameplayCanvas;
        [SerializeField] private GameObject pauseMenu;

        private void OnEnable()
        {
            SetPauseActionListeners(true);
            SetUnpauseActionListeners(true);
        }
        
        private void OnDisable()
        {
            SetPauseActionListeners(false);
            SetUnpauseActionListeners(false);
        }

        private void Start()
        {
            ToggleMousePointerOff();
        }

        private void SetPauseActionListeners(bool registerListener)
        {
            SetListeners(ref GameplayManager.Instance.OnGamePause, registerListener, 
                TogglePauseMenuOn, ToggleGameplayCanvasOff, ToggleMousePointerOn);
        }

        private void SetUnpauseActionListeners(bool registerListener)
        {
            SetListeners(ref GameplayManager.Instance.OnGameResume, registerListener, 
                TogglePauseMenuOff, ToggleGameplayCanvasOn, ToggleMousePointerOff);
        }

        private void ToggleGameplayCanvasOff()
        {
            gameplayCanvas.SetActive(false);
        }
        
        private void ToggleGameplayCanvasOn()
        {
            gameplayCanvas.SetActive(true);
        }
        
        private void TogglePauseMenuOn()
        {
            pauseMenu.SetActive(true);
        }

        private void TogglePauseMenuOff()
        {
            pauseMenu.SetActive(false);
        }

        public static void ToggleMousePointerOff()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void ToggleMousePointerOn()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
    }
}