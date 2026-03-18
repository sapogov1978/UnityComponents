using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SGL.Boot
{
    /// <summary>
    /// Boot sequence for all SGL games.
    ///
    /// Usage: drop the BootCanvas prefab into the scene, assign Splash Sprite. Done.
    ///
    /// Sequence:
    ///   1. SGL logo:  fade from black → hold → fade to black
    ///   2. Splash:    fade from black → hold → dissolve into main menu
    /// </summary>
    public class BootSequenceController : MonoBehaviour
    {
        [SerializeField] private Sprite _splashSprite;

        private Image       _blackOverlay;
        private CanvasGroup _logoLayer;
        private CanvasGroup _splashLayer;
        private Image       _splashImage;

        private const float LogoFadeIn     = 0.6f;
        private const float LogoHold       = 1.8f;
        private const float LogoFadeOut    = 0.6f;
        private const float SplashFadeIn   = 0.8f;
        private const float SplashHold     = 1.5f;
        private const float SplashDissolve = 1.2f;

        private System.Action _onComplete;
        private bool _running;

        void Awake()
        {
            _blackOverlay = transform.Find("BlackOverlay")?.GetComponent<Image>();
            _logoLayer    = transform.Find("LogoLayer")?.GetComponent<CanvasGroup>();
            _splashLayer  = transform.Find("SplashLayer")?.GetComponent<CanvasGroup>();
            _splashImage  = transform.Find("SplashLayer/SplashImage")?.GetComponent<Image>();

            if (_blackOverlay == null) Debug.LogError("[SGL.Boot] BlackOverlay not found in prefab hierarchy.");
            if (_logoLayer    == null) Debug.LogError("[SGL.Boot] LogoLayer not found in prefab hierarchy.");
            if (_splashLayer  == null) Debug.LogError("[SGL.Boot] SplashLayer not found in prefab hierarchy.");
            if (_splashImage  == null) Debug.LogError("[SGL.Boot] SplashLayer/SplashImage not found in prefab hierarchy.");
        }

        public void StartBoot(System.Action onComplete)
        {
            if (_running)
            {
                Debug.LogWarning("[SGL.Boot] StartBoot called while already running. Ignored.");
                return;
            }

            if (_splashSprite == null)
                Debug.LogWarning("[SGL.Boot] Splash Sprite is not assigned. Splash screen will be empty.");

            _onComplete = onComplete;
            _running    = true;

            if (_splashSprite != null)
                _splashImage.sprite = _splashSprite;

            StartCoroutine(BootRoutine());
        }

        private IEnumerator BootRoutine()
        {
            Debug.Log("[SGL.Boot] Boot sequence started.");

            SetOverlay(1f);
            SetLayer(_logoLayer,   0f);
            SetLayer(_splashLayer, 0f);

            // 1. SGL logo
            Debug.Log("[SGL.Boot] Showing SGL logo.");
            _logoLayer.gameObject.SetActive(true);
            SetLayer(_logoLayer, 1f);
            yield return FadeOverlay(1f, 0f, LogoFadeIn);
            yield return new WaitForSecondsRealtime(LogoHold);
            yield return FadeOverlay(0f, 1f, LogoFadeOut);
            _logoLayer.gameObject.SetActive(false);

            // 2. Splash
            Debug.Log("[SGL.Boot] Showing splash screen.");
            _splashLayer.gameObject.SetActive(true);
            SetLayer(_splashLayer, 1f);
            yield return FadeOverlay(1f, 0f, SplashFadeIn);
            yield return new WaitForSecondsRealtime(SplashHold);
            yield return FadeLayer(_splashLayer, 1f, 0f, SplashDissolve);

            Debug.Log("[SGL.Boot] Boot sequence complete.");
            gameObject.SetActive(false);
            _running = false;
            _onComplete?.Invoke();
        }

        private IEnumerator FadeOverlay(float from, float to, float duration)
        {
            float elapsed = 0f;
            Color c = _blackOverlay.color;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(from, to, elapsed / duration);
                _blackOverlay.color = c;
                yield return null;
            }
            c.a = to;
            _blackOverlay.color = c;
        }

        private IEnumerator FadeLayer(CanvasGroup group, float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            group.alpha = to;
        }

        private void SetOverlay(float alpha)
        {
            Color c = _blackOverlay.color;
            c.a = alpha;
            _blackOverlay.color = c;
        }

        private static void SetLayer(CanvasGroup group, float alpha)
        {
            group.alpha = alpha;
            group.interactable   = false;
            group.blocksRaycasts = false;
        }
    }
}