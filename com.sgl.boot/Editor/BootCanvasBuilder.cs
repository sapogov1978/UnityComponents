using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

namespace SGL.Boot.Editor
{
    /// <summary>
    /// Creates the BootCanvas prefab in a folder of your choice.
    /// Run via: Assets > SGL > Create Boot Prefab
    /// </summary>
    public static class BootCanvasBuilder
    {
        private const string SpritesPath = "Packages/com.sgl.boot/Sprites/SGL_logo.png";

        [MenuItem("Assets/SGL/Create Boot Prefab")]
        public static void CreateBootPrefab()
        {
            if (!Validate()) return;

            string folder = EditorUtility.OpenFolderPanel("Save BootCanvas prefab to...", "Assets", "");
            if (string.IsNullOrEmpty(folder)) return;

            if (!folder.StartsWith(Application.dataPath))
            {
                Debug.LogError("[SGL.Boot] Please choose a folder inside the project's Assets directory.");
                return;
            }

            string relativePath = "Assets" + folder.Substring(Application.dataPath.Length);
            string prefabPath   = Path.Combine(relativePath, "BootCanvas.prefab").Replace("\\", "/");

            GameObject root = BuildHierarchy();

            bool success;
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath, out success);
            GameObject.DestroyImmediate(root);

            if (success)
            {
                AssetDatabase.Refresh();
                Debug.Log($"[SGL.Boot] BootCanvas prefab created at {prefabPath}");
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
            }
            else
            {
                Debug.LogError("[SGL.Boot] Failed to save BootCanvas prefab.");
            }
        }

        private static bool Validate()
        {
            if (AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath) == null)
            {
                Debug.LogError($"[SGL.Boot] SGL_logo.png not found at {SpritesPath}. Add the sprite before building the prefab.");
                return false;
            }
            return true;
        }

        private static GameObject BuildHierarchy()
        {
            // ── Root: BootCanvas ──────────────────────────────────────────────────
            GameObject root   = new GameObject("BootCanvas");
            Canvas canvas     = root.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 99;
            CanvasScaler scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            root.AddComponent<GraphicRaycaster>();

            // ── BootSequenceController ────────────────────────────────────────────
            GameObject controllerGO = MakeRT("BootSequenceController", root.transform);
            controllerGO.AddComponent<BootSequenceController>();
            SetStretch(controllerGO);

            // ── BlackOverlay ──────────────────────────────────────────────────────
            GameObject overlay = MakeRT("BlackOverlay", controllerGO.transform);
            overlay.AddComponent<Image>().color = Color.black;
            SetStretch(overlay);

            // ── LogoLayer ─────────────────────────────────────────────────────────
            GameObject logoLayer     = MakeRT("LogoLayer", controllerGO.transform);
            CanvasGroup logoGroup    = logoLayer.AddComponent<CanvasGroup>();
            logoGroup.alpha          = 0f;
            logoGroup.interactable   = false;
            logoGroup.blocksRaycasts = false;
            logoLayer.SetActive(false);
            SetStretch(logoLayer);

            GameObject logoImageGO   = MakeRT("LogoImage", logoLayer.transform);
            Image logoImage          = logoImageGO.AddComponent<Image>();
            logoImage.preserveAspect = true;
            logoImage.sprite         = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath);
            SetCenter(logoImageGO, new Vector2(600f, 300f));

            // ── SplashLayer ───────────────────────────────────────────────────────
            GameObject splashLayer     = MakeRT("SplashLayer", controllerGO.transform);
            CanvasGroup splashGroup    = splashLayer.AddComponent<CanvasGroup>();
            splashGroup.alpha          = 0f;
            splashGroup.interactable   = false;
            splashGroup.blocksRaycasts = false;
            splashLayer.SetActive(false);
            SetStretch(splashLayer);

            GameObject splashImageGO = MakeRT("SplashImage", splashLayer.transform);
            splashImageGO.AddComponent<Image>().preserveAspect = true;
            SetStretch(splashImageGO);

            return root;
        }

        // Creates a GameObject with RectTransform already attached
        private static GameObject MakeRT(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void SetStretch(GameObject go)
        {
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void SetCenter(GameObject go, Vector2 size)
        {
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0.5f, 0.5f);
            rt.anchorMax        = new Vector2(0.5f, 0.5f);
            rt.sizeDelta        = size;
            rt.anchoredPosition = Vector2.zero;
        }
    }
}