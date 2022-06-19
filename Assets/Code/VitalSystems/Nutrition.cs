using System;
using System.Collections;
using Code.DeathMessages;
using Ez;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code
{
    public class Nutrition : MonoBehaviour
    {
        [Range(0, 1)][SerializeField] private float initialNutrition = 1.0f;
        [SerializeField] private float nutritionDecayPeriod = 1.0f; 
        [Range(0, 1)] [SerializeField] private float nutritionDecayFactor = 0.1f;

        [Header("User Interface")] 
        [SerializeField] ProgressBar nutritionMeter;
        
        
        private float _currentNutrition;
        private CauseOfDeath _causeOfDeath = CauseOfDeath.Indigestion;

        private float CurrentNutrition
        {
            set => nutritionMeter.Value = _currentNutrition = Mathf.Clamp01(value);
            get => _currentNutrition;
        }
        
        private const float MAXNutrition = 1.0f;
        
        public event Action OnNutritionEnd;


        private void Awake()
        {
            CurrentNutrition = initialNutrition;
        }

        private void Start()
        {
            StartCoroutine(Hunger());
        }

        private void Update()
        {
            if (CurrentNutrition <= 0)
            {
                gameObject.Send<IVitalSystems>(_=>_.Die(_causeOfDeath));
            }
        }

        /// <summary>
        /// Loses *nutritionDecayFactor* each *nutritionDecayPeriod* seconds
        /// </summary>
        /// <returns> it's a COROUTINE </returns>
        private IEnumerator Hunger()
        {
            var period = new WaitForSeconds(nutritionDecayPeriod);
            while (true)
            {
                yield return period;
                CurrentNutrition -= nutritionDecayFactor;
            }
        }

        //Deve ficar numa classe separada
        void Die()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void RecoverNutrition(float value)
        {
            CurrentNutrition += value;
        }

        public float NutritionDecayFactor
        {
            get => nutritionDecayFactor;
            set => nutritionDecayFactor = value;
        }
    }
}