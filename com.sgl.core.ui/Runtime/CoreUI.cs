using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SGL.CoreUI
{
    // ─────────────────────────────────────────────────────────────
    // UIElement — корень иерархии
    // ─────────────────────────────────────────────────────────────
    public abstract class UIElement : MonoBehaviour
    {
        [Header("Base Settings")]
        [SerializeField] protected bool _activeOnStart = true;

        protected virtual void Awake()  => Initialize();

        protected virtual void Start()
        {
            if (_activeOnStart) Show(); else Hide();
        }

        protected virtual void Initialize() { }

        public abstract void Show();
        public abstract void Hide();
        public abstract bool IsVisible { get; }
    }

    // ─────────────────────────────────────────────────────────────
    // UIFadeable — fade через CanvasGroup
    // ─────────────────────────────────────────────────────────────
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIFadeable : UIElement
    {
        [Header("Fade Settings")]
        [SerializeField] protected float _fadeInDuration  = 0.3f;
        [SerializeField] protected float _fadeOutDuration = 0.3f;

        protected CanvasGroup _canvasGroup;
        protected Coroutine   _fadeCoroutine;

        public override bool IsVisible => _canvasGroup != null && _canvasGroup.alpha > 0f;

        protected override void Initialize()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            FadeIn();
        }

        public override void Hide() => FadeOut();

        public virtual void FadeIn()
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeInRoutine());
        }

        public virtual void FadeOut()
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeOutRoutine());
        }

        protected virtual IEnumerator FadeInRoutine()
        {
            yield return UIFader.FadeIn(_canvasGroup, _fadeInDuration);
            OnFadeInComplete();
        }

        protected virtual IEnumerator FadeOutRoutine()
        {
            yield return UIFader.FadeOut(_canvasGroup, _fadeOutDuration);
            OnFadeOutComplete();
        }

        protected virtual void OnFadeInComplete()  { }
        protected virtual void OnFadeOutComplete() => gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────
    // UIPanel — панель с опциональным SetActive после fade-out
    // ─────────────────────────────────────────────────────────────
    public abstract class UIPanel : UIFadeable
    {
        [Header("Panel Settings")]
        [SerializeField] protected bool _hideOnFadeOutComplete = true;

        protected override void OnFadeOutComplete()
        {
            if (_hideOnFadeOutComplete) gameObject.SetActive(false);
        }
    }

    // ─────────────────────────────────────────────────────────────
    // UIButton — кнопка с scale feedback
    // Fade унаследован от UIFadeable → кнопка сама не фейдит,
    // фейдит панель-родитель через CanvasGroup
    // ─────────────────────────────────────────────────────────────
    [RequireComponent(typeof(Button))]
    public abstract class UIButton : UIFadeable, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Button")]
        [SerializeField] protected Button _button;
        [SerializeField] protected Image  _icon;

        [Header("Press Feedback")]
        [SerializeField] protected bool  _useScaleEffect = true;
        [SerializeField] protected float _pressedScale   = 0.95f;
        [SerializeField] protected float _scaleDuration  = 0.1f;

        protected Vector3  _originalScale;
        protected Coroutine _scaleCoroutine;

        protected override void Initialize()
        {
            base.Initialize();

            if (_button == null) _button = GetComponent<Button>();
            _originalScale = transform.localScale;
            _button?.onClick.AddListener(OnClick);
        }

        protected virtual void OnDestroy() => _button?.onClick.RemoveListener(OnClick);

        protected abstract void OnClick();

        public virtual void SetInteractable(bool v)
        {
            if (_button != null) _button.interactable = v;
        }

        public virtual void OnPointerDown(PointerEventData e)
        {
            if (!_useScaleEffect || !(_button?.interactable ?? false)) return;
            if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = StartCoroutine(ScaleTo(_originalScale * _pressedScale));
            OnPressVisual();
        }

        public virtual void OnPointerUp(PointerEventData e)
        {
            if (!_useScaleEffect || !(_button?.interactable ?? false)) return;
            if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = StartCoroutine(ScaleTo(_originalScale));
            OnReleaseVisual();
        }

        protected virtual void OnPressVisual()   { }
        protected virtual void OnReleaseVisual() { }

        private IEnumerator ScaleTo(Vector3 target)
        {
            Vector3 start   = transform.localScale;
            float   elapsed = 0f;
            while (elapsed < _scaleDuration)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(start, target, elapsed / _scaleDuration);
                yield return null;
            }
            transform.localScale = target;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // UIAlwaysVisible — элемент который нельзя скрыть
    // ─────────────────────────────────────────────────────────────
    public class UIAlwaysVisible : UIElement
    {
        public override bool IsVisible => true;

        public override void Show() => gameObject.SetActive(true);

        public override void Hide()
        {
            Debug.LogWarning($"[UIAlwaysVisible] {gameObject.name} cannot be hidden");
        }
    }

    // ─────────────────────────────────────────────────────────────
    // UIFader — static утилита
    // ─────────────────────────────────────────────────────────────
    public static class UIFader
    {
        public static IEnumerator FadeIn(CanvasGroup cg, float duration)
        {
            if (cg == null) yield break;
            cg.alpha = 0f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Clamp01(t / duration);
                yield return null;
            }
            cg.alpha          = 1f;
            cg.interactable   = true;
            cg.blocksRaycasts = true;
        }

        public static IEnumerator FadeOut(CanvasGroup cg, float duration)
        {
            if (cg == null) yield break;
            cg.interactable   = false;
            cg.blocksRaycasts = false;
            float start = cg.alpha;
            float t     = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Lerp(start, 0f, t / duration);
                yield return null;
            }
            cg.alpha = 0f;
        }
    }
}