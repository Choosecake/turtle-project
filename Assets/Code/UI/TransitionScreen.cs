using System;
using System.Collections;
using System.Threading.Tasks;
using Code;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class TransitionScreen : MonoBehaviour
    {
        [SerializeField] private TurtleVitalSystems _vitalSystems;
        [SerializeField] private float fadeInTime = 1.0f;
        [SerializeField] private float delayTime = 0.5f;
        [SerializeField] private float fadeOutTime = 1.0f;
        [Header("Message")] 
        [Multiline][SerializeField] private string[] messageContent;
        [SerializeField] private float[] readingDelayTime;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float textFadeInTime;


        private Image _blackoutScreen;

        private void Awake()
        {
            _blackoutScreen = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _vitalSystems.OnTurtleDeath += FadeInCoroutine;
        }

        void Start()
        {
            _blackoutScreen.color = Color.black;
            messageText.color = new Color(1,1,1,0);
            StartCoroutine(BackgroundFadeOut());
        }

        private void OnDisable()
        {
            _vitalSystems.OnTurtleDeath -= FadeInCoroutine;
        }
        
        private IEnumerator BackgroundFadeIn()
        {
            Tween fadeTween = _blackoutScreen.DOFade(1, fadeInTime);
            yield return new WaitForSeconds(fadeInTime+delayTime);
            StartCoroutine(MessageFadeCycle());
        }
        private void FadeInCoroutine() => StartCoroutine(BackgroundFadeIn());

        private IEnumerator BackgroundFadeOut()
        {
            _blackoutScreen.DOFade(0, fadeOutTime);
            yield return null;
        }
        
        private async void BackgroundFadeIn_Async()
        {
            Tween fadeTween = _blackoutScreen.DOFade(1, fadeInTime);
            await fadeTween.AsyncWaitForCompletion();
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator MessageFadeCycle()
        {
            // var readingYield = new WaitForSeconds(textFadeInTime + readingDelayTime);
            var fadeCompletionYield = new WaitForSeconds(textFadeInTime);
            for (int i = 0; i < messageContent.Length; i++)
            {
                messageText.text = messageContent[i];
                Tween fadeTween = messageText.DOFade(1, textFadeInTime);
                yield return new WaitForSeconds(textFadeInTime + readingDelayTime[i]);
                fadeTween = messageText.DOFade(0, textFadeInTime);
                yield return fadeCompletionYield;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}