using System;
using UnityEngine;

namespace Code
{
    public class CinematicManager : MonoBehaviour
    {
        [SerializeField] private GameObject turtle;
        [SerializeField] private GameObject turtleMesh;
        [SerializeField] private GameObject gamePlayHUD;
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private GameObject cinematicCamera;
        [SerializeField] private KeyCode holdingKeyCode = KeyCode.LeftShift;

        private PlayerMovement _playerMovement;
        private Nutrition _nutrition;
        private Breath _breath;

        private float _originalNutritionDecayFactor;
        private float _originalBreathDecayFactor;
        private bool _isTurtleImmortal = false;
        private bool _isHudEnabled = true;
        private bool _isCinematicModeActive = false;

        private void Awake()
        {
            _playerMovement = turtle.GetComponent<PlayerMovement>();
            _nutrition = turtle.GetComponent<Nutrition>();
            _breath = turtle.GetComponent<Breath>();

            if (playerCamera == null)
            {
                turtle.GetComponentInChildren<Camera>();
            }
        }

        private void Start()
        {
            _originalBreathDecayFactor = _breath.BreathDecayFactor;
            _originalNutritionDecayFactor = _nutrition.NutritionDecayFactor;
        }

        private void Update()
        {
            if (Input.GetKey(holdingKeyCode))
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    SwitchToGamePlayMode();
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                   SwitchToCinematicPlayMode();
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    SwitchToCinematicFreeMode();
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    ToggleTurtleMesh();
                }
            }
        }

        private void ToggleNutritionDecay()
        {
            _nutrition.NutritionDecayFactor = _nutrition.NutritionDecayFactor != 0 ? 0 : _originalNutritionDecayFactor;
        }

        private void ToggleBreathDecay()
        {
            _breath.BreathDecayFactor = _breath.BreathDecayFactor != 0 ? 0 : _originalBreathDecayFactor;
        }

        private void ToggleTurtleMortality(bool turtleShouldBeImmortal)
        {
            if (turtleShouldBeImmortal)
            {
                _nutrition.NutritionDecayFactor = 0;
                _breath.BreathDecayFactor = 0;
            }
            else
            {
                _nutrition.NutritionDecayFactor = _originalNutritionDecayFactor;
                _breath.BreathDecayFactor = _originalBreathDecayFactor;
            }
        }

        private void ToggleGameplayHUD(bool enable)
        {
            gamePlayHUD.SetActive(enable);
        }

        private void ToggleTurtleMesh()
        {
            turtleMesh.SetActive(!turtleMesh.activeSelf);
        }

        private void SwitchToCinematicPlayMode()
        {
            cinematicCamera.SetActive(false);
            playerCamera.SetActive(true);
            _playerMovement.enabled = true;
            ToggleTurtleMortality(true);
            ToggleGameplayHUD(false);
        }

        private void SwitchToCinematicFreeMode()
        {
            cinematicCamera.SetActive(true);
            playerCamera.SetActive(false);
            _playerMovement.enabled = false;
            ToggleTurtleMortality(true);
            ToggleGameplayHUD(false);
        }

        private void SwitchToGamePlayMode()
        {
            cinematicCamera.SetActive(false);
            playerCamera.SetActive(true);
            _playerMovement.enabled = true;
            ToggleTurtleMortality(false);
            ToggleGameplayHUD(true);
        }
    }
}