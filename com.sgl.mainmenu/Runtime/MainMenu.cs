using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace SGL.MainMenu
{
    // ─────────────────────────────────────────────
    // Данные одной кнопки — настраивается в Inspector
    // ─────────────────────────────────────────────
    [Serializable]
    public class MenuButtonData
    {
        [Tooltip("Название кнопки (для отладки)")]
        public string label = "Button";

        [Tooltip("Спрайт иконки на кнопке")]
        public Sprite icon;

        [Tooltip("Звук при нажатии")]
        public AudioClip clickSound;

        [Tooltip("Скрывать кнопку при переходе в игру")]
        public bool hideOnPlay = false;

        [Tooltip("Что делает кнопка")]
        public UnityEvent onClick;
    }

    // ─────────────────────────────────────────────
    // Главный компонент
    // Перетащить на Canvas → настроить → готово
    // ─────────────────────────────────────────────
    [AddComponentMenu("SGL/Main Menu")]
    public class MainMenu : MonoBehaviour
    {
        // ── Layout ──────────────────────────────

        [Header("Banner")]
        [Tooltip("Высота рекламного баннера (px). 0 = нет баннера.")]
        [SerializeField] private float _bannerHeight = 90f;

        [Tooltip("Цвет placeholder баннера")]
        [SerializeField] private Color _bannerColor = new Color(0.2f, 0.4f, 0.9f, 0.5f);

        [Header("Buttons Panel")]
        [Tooltip("Размер одной кнопки (px)")]
        [SerializeField] private float _buttonSize = 64f;

        [Tooltip("Отступ между кнопками (px)")]
        [SerializeField] private float _buttonSpacing = 12f;

        [Tooltip("Отступ от правого края (px)")]
        [SerializeField] private float _rightPadding = 16f;

        [Header("Menu Content")]
        [Tooltip("Название игры")]
        [SerializeField] private string _gameTitle = "TUBE RUNNER";

        [Tooltip("Текст кнопки Play")]
        [SerializeField] private string _playLabel = "PLAY";

        [Header("Fade")]
        [SerializeField] private float _fadeInDuration  = 0.4f;
        [SerializeField] private float _fadeOutDuration = 0.25f;

        [Header("Boot")]
        [Tooltip("Длительность показа boot-экрана (сек)")]
        [SerializeField] private float _bootDuration = 2f;

        [Tooltip("Логотип на boot-экране")]
        [SerializeField] private Sprite _bootLogo;

        [Header("Buttons")]
        [SerializeField] private MenuButtonData[] _buttons = Array.Empty<MenuButtonData>();

        // ── Runtime references ───────────────────
        private CanvasGroup _bootGroup;
        private CanvasGroup _menuGroup;
        private CanvasGroup _buttonsGroup;     // только hideOnPlay=false кнопки
        private AudioSource _audioSource;

        private Transform  _bannerRoot;
        private Transform  _buttonsRoot;
        private Transform  _menuRoot;

        // Runtime кнопки: индекс → объект
        private GameObject[] _buttonObjects;

        // ── Events — подключить снаружи если нужно ─
        public event Action OnPlayPressed;
        public event Action OnBootComplete;

        // ─────────────────────────────────────────
        // Unity lifecycle
        // ─────────────────────────────────────────

        void Awake()
        {
            BuildUI();
        }

        void Start()
        {
            StartCoroutine(BootRoutine());
        }

        // ─────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────

        /// <summary>
        /// Скрыть меню-контент и hideOnPlay кнопки при старте игры.
        /// Вызывать из своего GameManager.
        /// </summary>
        public void ShowGameplay()
        {
            StartCoroutine(FadeOut(_menuGroup));
            HidePlayOnlyButtons();
        }

        /// <summary>
        /// Вернуть полное меню (после Game Over).
        /// </summary>
        public void ShowMenu()
        {
            StartCoroutine(FadeIn(_menuGroup));
            ShowPlayOnlyButtons();
            UpdateBestScore();
        }

        // ─────────────────────────────────────────
        // UI Construction
        // ─────────────────────────────────────────

        private void BuildUI()
        {
            // AudioSource для кнопок
            _audioSource = gameObject.GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;

            // Canvas должен быть на этом же GO или родителе
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[MainMenu] Нет Canvas в родителях. Добавь MainMenu на Canvas.");
                return;
            }

            BuildBoot();
            BuildBanner();
            BuildButtonsPanel();
            BuildMenuContent();

            // Стартовое состояние: boot видим, меню скрыто
            SetAlpha(_bootGroup,    1f);
            SetAlpha(_menuGroup,    0f);
            SetAlpha(_buttonsGroup, 0f);
        }

        // ── Boot ────────────────────────────────

        private void BuildBoot()
        {
            var bootGO = CreateGO("__Boot", transform);
            StretchFull(bootGO);

            var bg = bootGO.AddComponent<Image>();
            bg.color = Color.black;

            _bootGroup = bootGO.AddComponent<CanvasGroup>();

            if (_bootLogo != null)
            {
                var logoGO = CreateGO("Logo", bootGO.transform);
                Center(logoGO, new Vector2(400, 200));
                logoGO.AddComponent<Image>().sprite = _bootLogo;
            }
            else
            {
                // Текстовый placeholder
                var logoGO = CreateGO("LogoText", bootGO.transform);
                Center(logoGO, new Vector2(500, 120));
                var tmp = logoGO.AddComponent<TextMeshProUGUI>();
                tmp.text = _gameTitle;
                tmp.fontSize = 72;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.white;
            }
        }

        // ── Banner ──────────────────────────────

        private void BuildBanner()
        {
            if (_bannerHeight <= 0f) return;

            var go = CreateGO("__Banner", transform);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot     = new Vector2(0.5f, 1f);

            float safeTop = Screen.height - Screen.safeArea.yMax;
            rt.sizeDelta        = new Vector2(0f, _bannerHeight + safeTop);
            rt.anchoredPosition = Vector2.zero;

            var img = go.AddComponent<Image>();
            img.color = _bannerColor;

            _bannerRoot = go.transform;
        }

        // ── Buttons Panel ────────────────────────

        private void BuildButtonsPanel()
        {
            var panelGO = CreateGO("__Buttons", transform);
            var rt = panelGO.GetComponent<RectTransform>();

            float topOffset = _bannerHeight + 16f;
            rt.anchorMin        = new Vector2(1f, 1f);
            rt.anchorMax        = new Vector2(1f, 1f);
            rt.pivot            = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-_rightPadding, -topOffset);

            float totalH = (_buttonSize + _buttonSpacing) * _buttons.Length;
            rt.sizeDelta = new Vector2(_buttonSize, totalH);

            _buttonsGroup = panelGO.AddComponent<CanvasGroup>();
            _buttonsRoot  = panelGO.transform;

            // VLG
            var vlg = panelGO.AddComponent<VerticalLayoutGroup>();
            vlg.spacing              = _buttonSpacing;
            vlg.childAlignment       = TextAnchor.UpperRight;
            vlg.childControlWidth    = true;
            vlg.childControlHeight   = true;
            vlg.childForceExpandWidth  = false;
            vlg.childForceExpandHeight = false;

            _buttonObjects = new GameObject[_buttons.Length];

            for (int i = 0; i < _buttons.Length; i++)
            {
                var data  = _buttons[i];
                var btnGO = CreateGO(data.label, panelGO.transform);

                var le = btnGO.AddComponent<LayoutElement>();
                le.preferredWidth  = _buttonSize;
                le.preferredHeight = _buttonSize;

                var img = btnGO.AddComponent<Image>();
                img.color = new Color(1f, 1f, 1f, 0.15f);

                if (data.icon != null)
                    img.sprite = data.icon;

                var btn = btnGO.AddComponent<Button>();

                // Захватываем для closure
                int idx = i;
                btn.onClick.AddListener(() => OnButtonClick(idx));

                _buttonObjects[i] = btnGO;
            }
        }

        // ── Menu Content ─────────────────────────

        private void BuildMenuContent()
        {
            var menuGO = CreateGO("__Menu", transform);
            StretchFull(menuGO);
            _menuGroup = menuGO.AddComponent<CanvasGroup>();
            _menuRoot  = menuGO.transform;

            // Title
            var titleGO = CreateGO("Title", menuGO.transform);
            Center(titleGO, new Vector2(640, 90));
            titleGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 250f);
            var titleTmp = titleGO.AddComponent<TextMeshProUGUI>();
            titleTmp.text = _gameTitle;
            titleTmp.fontSize = 64;
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.color = Color.white;

            // Best score — обновляется через UpdateBestScore()
            var bestGO = CreateGO("BestScore", menuGO.transform);
            Center(bestGO, new Vector2(400, 50));
            bestGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 150f);
            var bestTmp = bestGO.AddComponent<TextMeshProUGUI>();
            bestTmp.text = string.Empty;
            bestTmp.fontSize = 36;
            bestTmp.alignment = TextAlignmentOptions.Center;
            bestTmp.color = new Color(1f, 1f, 1f, 0.6f);
            bestTmp.name = "BestScoreText"; // для UpdateBestScore

            // Play button
            var playGO = CreateGO("PlayButton", menuGO.transform);
            Center(playGO, new Vector2(320, 90));
            playGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -180f);
            playGO.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.2f);
            var playBtn = playGO.AddComponent<Button>();
            playBtn.onClick.AddListener(HandlePlayPressed);

            var playLabelGO = CreateGO("Label", playGO.transform);
            StretchFull(playLabelGO);
            var playTmp = playLabelGO.AddComponent<TextMeshProUGUI>();
            playTmp.text = _playLabel;
            playTmp.fontSize = 48;
            playTmp.alignment = TextAlignmentOptions.Center;
            playTmp.color = Color.white;
        }

        // ─────────────────────────────────────────
        // Boot sequence
        // ─────────────────────────────────────────

        private IEnumerator BootRoutine()
        {
            yield return new WaitForSeconds(_bootDuration);
            yield return FadeOut(_bootGroup);

            // Показать меню
            yield return FadeIn(_buttonsGroup);
            yield return FadeIn(_menuGroup);

            UpdateBestScore();
            OnBootComplete?.Invoke();
        }

        // ─────────────────────────────────────────
        // Button handling
        // ─────────────────────────────────────────

        private void OnButtonClick(int index)
        {
            var data = _buttons[index];

            if (data.clickSound != null)
                _audioSource.PlayOneShot(data.clickSound);

            data.onClick?.Invoke();
        }

        private void HandlePlayPressed()
        {
            OnPlayPressed?.Invoke();
            ShowGameplay();
        }

        private void HidePlayOnlyButtons()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i].hideOnPlay)
                    _buttonObjects[i].SetActive(false);
            }
        }

        private void ShowPlayOnlyButtons()
        {
            for (int i = 0; i < _buttons.Length; i++)
                _buttonObjects[i].SetActive(true);
        }

        // ─────────────────────────────────────────
        // Score
        // ─────────────────────────────────────────

        private void UpdateBestScore()
        {
            if (_menuRoot == null) return;
            var txt = _menuRoot.Find("BestScore/BestScoreText")?.GetComponent<TextMeshProUGUI>()
                   ?? _menuRoot.GetComponentInChildren<TextMeshProUGUI>();

            int best = PlayerPrefs.GetInt("BestScore", 0);
            if (txt != null)
                txt.text = best > 0 ? $"Best: {best}" : string.Empty;
        }

        // ─────────────────────────────────────────
        // Fade helpers
        // ─────────────────────────────────────────

        private IEnumerator FadeIn(CanvasGroup cg)
        {
            if (cg == null) yield break;
            float t = 0f;
            cg.blocksRaycasts = true;
            while (t < _fadeInDuration)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Clamp01(t / _fadeInDuration);
                yield return null;
            }
            cg.alpha = 1f;
            cg.interactable = true;
        }

        private IEnumerator FadeOut(CanvasGroup cg)
        {
            if (cg == null) yield break;
            cg.interactable  = false;
            cg.blocksRaycasts = false;
            float t = 0f;
            float start = cg.alpha;
            while (t < _fadeOutDuration)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Lerp(start, 0f, t / _fadeOutDuration);
                yield return null;
            }
            cg.alpha = 0f;
        }

        private static void SetAlpha(CanvasGroup cg, float a)
        {
            if (cg == null) return;
            cg.alpha         = a;
            cg.interactable  = a > 0f;
            cg.blocksRaycasts = a > 0f;
        }

        // ─────────────────────────────────────────
        // GameObject helpers
        // ─────────────────────────────────────────

        private static GameObject CreateGO(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void StretchFull(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void Center(GameObject go, Vector2 size)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot     = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
        }
    }
}