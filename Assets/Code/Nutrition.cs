using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code
{
    public class Nutrition : MonoBehaviour
    {
        [SerializeField] private float nutritionDecayPeriod = 1.0f; 
        [Range(0, 1)] [SerializeField] private float nutritionDecayFactor = 0.1f;

        private float _currentNutrition;
        
        private const float MAXNutrition = 1.0f;

        private void Awake()
        {
            _currentNutrition = MAXNutrition;
        }

        private void Start()
        {
            StartCoroutine(Hunger());
        }

        private void Update()
        {
            if (_currentNutrition <= 0)
            {
                Die();
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
                _currentNutrition -= nutritionDecayFactor;
            }
        }

        //Deve ficar numa classe separada
        void Die()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}