using System;
using System.Collections;
using Ez;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Code
{
    public class Breath : MonoBehaviour
    {
        [Range(0, 1)][SerializeField] private float initialBreath = 1.0f;
        [SerializeField] private float breathDecayPeriod = 1.0f; 
        [Range(0, 1)] [SerializeField] private float breathDecayFactor = 0.1f;

        [Header("References")] 
        //Might have to change to something without ProgressBarPro
        [SerializeField] private ProgressBarPro breathMeter;
        
        private float _currentBreath;

        private float CurrentBreath
        {
            set => breathMeter.Value = _currentBreath = Mathf.Clamp01(value);
            get => _currentBreath;
        }
        
        // private const float MAXNutrition = 1.0f;
        
        public Action OnBreathEnd;

        private void Awake()
        {
            CurrentBreath = initialBreath;
        }

        private void Start()
        {
            StartCoroutine(LoseBreath());
        }

        private void Update()
        {
            if (CurrentBreath <= 0)
            {
                gameObject.Send<IVitalSystems>(_=>_.Die());
            }
        }

        /// <summary>
        /// Loses *breathDecayFactor* each *breathDecayPeriod* seconds
        /// </summary>
        /// <returns> it's a COROUTINE </returns>
        private IEnumerator LoseBreath()
        {
            var period = new WaitForSeconds(breathDecayPeriod);
            while (true)
            {
                yield return period;
                CurrentBreath -= breathDecayFactor;
            }
        }

        // //Deve ficar numa classe separada
        // void Die()
        // {
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // }

        public void RecoverBreath(float value)
        {
            CurrentBreath += value;
        }

        public float BreathDecayFactor
        {
            get => breathDecayFactor;
            set => breathDecayFactor = value;
        }
    }
}