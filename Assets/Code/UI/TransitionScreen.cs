using System;
using System.Collections;
using System.Threading.Tasks;
using Code;
using DG.Tweening;
using Ez;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class TransitionScreen : MonoBehaviour
    {
        [SerializeField] private GameObject gameEnder;
        [SerializeField] private float fadeInTime = 1.0f;
        [SerializeField] private float delayTime = 0.5f;
        [SerializeField] private float fadeOutTime = 1.0f;
        [Header("Message")] 
        [Multiline][SerializeField] private string[] messageContent;
        [SerializeField] private float[] readingDelayTime;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float textFadeInTime;
        [Space]
        [SerializeField] private AudioClip deathTrack;
        [SerializeField] private AudioSource musicPlayer;

        private Image _blackoutScreen;

        public GameObject GameEnder
        {
            get => gameEnder;
            set => gameEnder = value;
        }

        private void Awake()
        {
            _blackoutScreen = GetComponent<Image>();
        }

        void Start()
        {
            if (gameEnder != null)
            {
                gameEnder.GetComponent<GameEnder>().OnCriticalPointReached += FadeInCoroutine;
            }

            if (musicPlayer == null)
            {
                 musicPlayer = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
                if (musicPlayer == null)
                {
                    Debug.LogWarning("No valid audioSource has been found!");
                }
            }
                
            _blackoutScreen.color = new Color(_blackoutScreen.color.r, _blackoutScreen.color.g, _blackoutScreen.color.b, 0);
            messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 0);
            StartCoroutine(BackgroundFadeOut());
        }

        private void OnDisable()
        {
            if (gameEnder != null)
            {
                gameEnder.GetComponent<GameEnder>().OnCriticalPointReached -= FadeInCoroutine;
            }
        }
        
        private IEnumerator BackgroundFadeIn()
        {
            musicPlayer.loop = false;
            musicPlayer.clip = deathTrack;
            musicPlayer.Play();
            
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
        }

        private IEnumerator MessageFadeCycle()
        {
            var fadeCompletionYield = new WaitForSeconds(textFadeInTime);
            for (int i = 0; i < messageContent.Length; i++)
            {
                messageText.text = messageContent[i];
                Tween fadeTween = messageText.DOFade(1, textFadeInTime);
                yield return new WaitForSeconds(textFadeInTime + readingDelayTime[i]);
                fadeTween = messageText.DOFade(0, textFadeInTime);
                yield return fadeCompletionYield;
            }
            SceneManager.LoadScene(0);
        }
    }
}