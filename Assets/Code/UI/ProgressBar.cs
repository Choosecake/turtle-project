using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private Image bgImage;
        [SerializeField] private Image outlineImage;
        [SerializeField] private float transitionSpeed = 0.25f;

        private float initialBgAlpha;

        private void Awake()
        {
            initialBgAlpha = bgImage.color.a;
        }

        public void SetValue(float value)
        {
            fillImage.DOFillAmount(value, transitionSpeed);
            bgImage.DOFade(value * initialBgAlpha, transitionSpeed);
            outlineImage.DOFade(value * initialBgAlpha, transitionSpeed);
        }

        public float Value { set => SetValue(value); }
    }
}