using UnityEngine;
using UnityEditor;
using SGL.MainMenu;

namespace SGL.MainMenu.Editor
{
    /// <summary>
    /// Кастомный Inspector для MainMenu.
    /// Добавляет кнопку «Создать на сцене» и валидацию.
    /// </summary>
    [CustomEditor(typeof(MainMenu))]
    public class MainMenuEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

            // Валидация
            var menu = (MainMenu)target;
            bool isOnCanvas = menu.GetComponentInParent<Canvas>() != null;

            if (!isOnCanvas)
            {
                EditorGUILayout.HelpBox(
                    "MainMenu должен быть дочерним объектом Canvas.",
                    MessageType.Error);
            }

            // Быстрое создание Canvas + компонента
            using (new EditorGUI.DisabledScope(isOnCanvas))
            {
                if (GUILayout.Button("Создать Canvas и разместить"))
                {
                    CreateCanvasWithMenu();
                }
            }
        }

        // ── MenuItem — самый быстрый способ добавить в сцену ──

        [MenuItem("GameObject/SGL/Main Menu", false, 10)]
        static void CreateFromMenu(MenuCommand cmd)
        {
            // Найти или создать Canvas
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
                canvas = CreateCanvas();

            // Создать GO с компонентом
            var go = new GameObject("MainMenu", typeof(RectTransform));
            go.transform.SetParent(canvas.transform, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            go.AddComponent<MainMenu>();

            GameObjectUtility.SetParentAndAlign(go, canvas.gameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create MainMenu");
            Selection.activeObject = go;

            Debug.Log("[SGL] MainMenu создан. Настрой параметры в Inspector.");
        }

        private static void CreateCanvasWithMenu()
        {
            var canvas = CreateCanvas();
            var go = new GameObject("MainMenu", typeof(RectTransform));
            go.transform.SetParent(canvas.transform, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            go.AddComponent<MainMenu>();
            Undo.RegisterCreatedObjectUndo(canvas.gameObject, "Create Canvas + MainMenu");
            Selection.activeObject = go;
        }

        private static Canvas CreateCanvas()
        {
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode         = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight  = 0.5f;

            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            return canvas;
        }
    }
}