using System;
using System.Collections;
using Code;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class TransitionScreen : MonoBehaviour
    {
        [SerializeField] private TurtleVitalSystems _vitalSystems;
        [SerializeField] private float fadeTime = 1.0f;
        [SerializeField] private float delayTime = 0.5f;

        private Image _blackoutScreen;

        private void Awake()
        {
            _blackoutScreen = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _vitalSystems.OnTurtleDeath += FadeInCoroutine;
        }

        private void OnDisable()
        {
            _vitalSystems.OnTurtleDeath -= FadeInCoroutine;
        }
        
        private IEnumerator FadeIn()
        {
            Tween fadeTween = _blackoutScreen.DOFade(1, fadeTime);
            yield return new WaitForSeconds(fadeTime+delayTime);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void FadeInCoroutine() => StartCoroutine(FadeIn());
    }
}