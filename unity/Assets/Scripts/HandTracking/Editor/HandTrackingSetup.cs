using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Menu: Tools > Hand Tracking > Setup Prefab
// Merapikan prefab HandTrackingRig dalam sekali klik:
// - memindahkan prefab ke Scripts/HandTracking/Resources (supaya dibuat otomatis saat game mulai)
// - memasang & mengisi semua komponen (HandVirtualMouse, HandDwellClick, HandTrackingRig)
// - cursor jadi bulat, ring jadi lingkaran progress (Filled Radial 360)
// - Hold Seconds = 0.8, Raycast Target dimatikan, tampilan kamera MediaPipe disembunyikan
namespace DEMSIFT.HandTracking.EditorTools
{
  public static class HandTrackingSetup
  {
    private const string Root = "Assets/Scripts/HandTracking";
    private const string ResourcesDir = Root + "/Resources";
    private const string PrefabPath = ResourcesDir + "/HandTrackingRig.prefab";
    private const string ArtDir = Root + "/Art";

    private const float HoldSeconds = 0.8f;
    private const float CursorSize = 56f;
    private const float RingSize = 84f;
    private static readonly Color RingColor = new Color(0.49f, 0.36f, 0.85f, 1f);

    [MenuItem("Tools/Hand Tracking/Setup Prefab")]
    public static void Setup()
    {
      if (EditorApplication.isPlaying)
      {
        EditorUtility.DisplayDialog("Hand Tracking", "Stop Play dulu, lalu jalankan menu ini lagi.", "OK");
        return;
      }

      var log = new StringBuilder();

      var path = FindPrefabPath();
      if (path == null)
      {
        EditorUtility.DisplayDialog("Hand Tracking",
          "Prefab HandTrackingRig tidak ditemukan.\nBuat dulu prefab-nya dari scene CursorTest.", "OK");
        return;
      }

      if (path != PrefabPath)
      {
        EnsureFolder(ResourcesDir);
        var error = AssetDatabase.MoveAsset(path, PrefabPath);
        if (!string.IsNullOrEmpty(error))
        {
          Debug.LogError("[HandTracking] Gagal memindahkan prefab: " + error);
          return;
        }
        log.AppendLine("- Prefab dipindah ke " + PrefabPath);
        path = PrefabPath;
      }

      var ringSprite = GetOrCreateSprite("CursorRing.png", true);
      var dotSprite = GetOrCreateSprite("CursorDisc.png", false);

      var root = PrefabUtility.LoadPrefabContents(path);
      try
      {
        var handCursor = root.GetComponentInChildren<HandCursor>(true);
        if (handCursor == null)
        {
          EditorUtility.DisplayDialog("Hand Tracking",
            "Tidak ada objek dengan komponen HandCursor di dalam prefab.\nPastikan CursorCanvas > Cursor ada di dalam HandTrackingRig.", "OK");
          return;
        }

        var cursorGo = handCursor.gameObject;
        var cursorCanvas = cursorGo.GetComponentInParent<Canvas>(true);
        if (cursorCanvas == null || cursorCanvas.transform == root.transform)
        {
          EditorUtility.DisplayDialog("Hand Tracking", "Cursor harus berada di dalam CursorCanvas.", "OK");
          return;
        }

        // CursorCanvas: selalu di atas UI lain, tidak menghalangi klik.
        cursorCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        cursorCanvas.sortingOrder = 100;
        var cursorRaycaster = cursorCanvas.GetComponent<GraphicRaycaster>();
        if (cursorRaycaster != null) cursorRaycaster.enabled = false;
        log.AppendLine("- CursorCanvas: Sort Order 100");

        // Cursor: titik bulat, tidak menghalangi klik.
        var cursorImage = cursorGo.GetComponent<Image>();
        if (cursorImage != null)
        {
          cursorImage.sprite = dotSprite;
          cursorImage.type = Image.Type.Simple;
          cursorImage.preserveAspect = true;
          cursorImage.raycastTarget = false;
        }
        var cursorRect = cursorGo.GetComponent<RectTransform>();
        if (cursorRect != null) cursorRect.sizeDelta = new Vector2(CursorSize, CursorSize);
        var cursorVisual = cursorGo.GetComponent<HandCursorVisual>();
        if (cursorVisual == null) cursorVisual = cursorGo.AddComponent<HandCursorVisual>();
        SetRef(cursorVisual, "handCursor", handCursor);
        log.AppendLine("- Cursor: bulat " + CursorSize + "px, samar, mengecil saat pinch, Raycast Target mati");

        // Ring: anak dari Cursor, lingkaran progress.
        var ringImage = root.GetComponentsInChildren<Image>(true).FirstOrDefault(i => i.name == "Ring");
        if (ringImage == null)
        {
          var ringGo = new GameObject("Ring", typeof(RectTransform), typeof(Image));
          ringImage = ringGo.GetComponent<Image>();
          log.AppendLine("- Ring baru dibuat");
        }
        ringImage.transform.SetParent(cursorGo.transform, false);
        var ringRect = (RectTransform)ringImage.transform;
        ringRect.anchorMin = ringRect.anchorMax = ringRect.pivot = new Vector2(0.5f, 0.5f);
        ringRect.anchoredPosition = Vector2.zero;
        ringRect.sizeDelta = new Vector2(RingSize, RingSize);
        ringRect.localScale = Vector3.one;
        ringImage.sprite = ringSprite;
        ringImage.type = Image.Type.Filled;
        ringImage.fillMethod = Image.FillMethod.Radial360;
        ringImage.fillOrigin = (int)Image.Origin360.Top;
        ringImage.fillClockwise = true;
        ringImage.fillAmount = 0f;
        ringImage.color = RingColor;
        ringImage.raycastTarget = false;
        ringImage.gameObject.SetActive(true);
        log.AppendLine("- Ring: Filled Radial 360, anak dari Cursor");

        // Komponen di Cursor.
        var virtualMouse = cursorGo.GetComponent<HandVirtualMouse>();
        if (virtualMouse == null) virtualMouse = cursorGo.AddComponent<HandVirtualMouse>();
        var dwellClick = cursorGo.GetComponent<HandDwellClick>();
        if (dwellClick == null) dwellClick = cursorGo.AddComponent<HandDwellClick>();

        SetRef(virtualMouse, "handCursor", handCursor);
        SetRef(dwellClick, "handCursor", handCursor);
        SetRef(dwellClick, "progressRing", ringImage);
        SetFloat(dwellClick, "holdSeconds", HoldSeconds);
        SetRef(handCursor, "progressRing", null);
        log.AppendLine("- Hand Virtual Mouse & Hand Dwell Click terisi, Hold Seconds = " + HoldSeconds);

        // Komponen di root.
        var rig = root.GetComponent<HandTrackingRig>();
        if (rig == null) rig = root.AddComponent<HandTrackingRig>();
        SetRef(rig, "cursorCanvas", cursorCanvas.gameObject);
        SetRef(rig, "virtualMouse", virtualMouse);
        SetRef(rig, "dwellClick", dwellClick);
        log.AppendLine("- Hand Tracking Rig terisi");

        // Tampilan kamera MediaPipe disembunyikan (deteksi tetap jalan).
        foreach (var canvas in root.GetComponentsInChildren<Canvas>(true))
        {
          if (canvas == cursorCanvas || !canvas.isRootCanvas) continue;
          canvas.enabled = false;
          var raycaster = canvas.GetComponent<GraphicRaycaster>();
          if (raycaster != null) raycaster.enabled = false;
          log.AppendLine("- Tampilan kamera disembunyikan: " + canvas.name);
        }

        root.transform.position = Vector3.zero;

        PrefabUtility.SaveAsPrefabAsset(root, path);
      }
      finally
      {
        PrefabUtility.UnloadPrefabContents(root);
      }

      AssetDatabase.SaveAssets();

      var scenesWithRig = FindScenesUsing(AssetDatabase.AssetPathToGUID(path));
      if (scenesWithRig.Count > 0)
      {
        log.AppendLine();
        log.AppendLine("PERHATIAN: hapus HandTrackingRig dari scene ini (rig sekarang dibuat otomatis):");
        foreach (var scene in scenesWithRig) log.AppendLine("  " + scene);
        log.AppendLine("(CursorTest boleh tetap punya rig untuk tes.)");
      }

      Debug.Log("[HandTracking] Setup selesai:\n" + log);
      EditorUtility.DisplayDialog("Hand Tracking: Setup selesai", log.ToString(), "OK");
      Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
    }

    private static string FindPrefabPath()
    {
      if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null) return PrefabPath;

      foreach (var guid in AssetDatabase.FindAssets("t:Prefab HandTrackingRig"))
      {
        var candidate = AssetDatabase.GUIDToAssetPath(guid);
        var go = AssetDatabase.LoadAssetAtPath<GameObject>(candidate);
        if (go != null && go.GetComponent<HandTrackingRig>() != null) return candidate;
      }
      return null;
    }

    private static List<string> FindScenesUsing(string prefabGuid)
    {
      var result = new List<string>();
      foreach (var guid in AssetDatabase.FindAssets("t:Scene", new[] { "Assets" }))
      {
        var scenePath = AssetDatabase.GUIDToAssetPath(guid);
        if (scenePath.Contains("/_Recovery/")) continue;
        try
        {
          if (File.ReadAllText(scenePath).Contains(prefabGuid)) result.Add(scenePath);
        }
        catch (IOException)
        {
        }
      }
      return result;
    }

    private static void SetRef(Object target, string field, Object value)
    {
      var so = new SerializedObject(target);
      var prop = so.FindProperty(field);
      if (prop == null)
      {
        Debug.LogWarning($"[HandTracking] Field '{field}' tidak ada di {target.GetType().Name}. Pastikan script-nya versi terbaru.");
        return;
      }
      prop.objectReferenceValue = value;
      so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFloat(Object target, string field, float value)
    {
      var so = new SerializedObject(target);
      var prop = so.FindProperty(field);
      if (prop == null)
      {
        Debug.LogWarning($"[HandTracking] Field '{field}' tidak ada di {target.GetType().Name}. Pastikan script-nya versi terbaru.");
        return;
      }
      prop.floatValue = value;
      so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Sprite GetOrCreateSprite(string fileName, bool ring)
    {
      EnsureFolder(ArtDir);
      var assetPath = ArtDir + "/" + fileName;

      if (!File.Exists(assetPath))
      {
        const int size = 256;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var center = (size - 1) / 2f;
        var outer = size / 2f - 2f;
        var inner = ring ? outer - 26f : -1f;
        for (var y = 0; y < size; y++)
        {
          for (var x = 0; x < size; x++)
          {
            var d = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
            var a = Mathf.Clamp01(outer - d + 0.5f);
            if (ring)
            {
              a *= Mathf.Clamp01(d - inner + 0.5f);
            }
            else
            {
              var rim = Mathf.Clamp01(d - (outer - 14f) + 0.5f);
              a *= Mathf.Lerp(0.5f, 1f, rim);
            }
            tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
          }
        }
        tex.Apply();
        File.WriteAllBytes(assetPath, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(assetPath);
      }

      var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
      if (importer != null && importer.textureType != TextureImporterType.Sprite)
      {
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
      }

      return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }

    private static void EnsureFolder(string folder)
    {
      if (AssetDatabase.IsValidFolder(folder)) return;
      var parent = Path.GetDirectoryName(folder)?.Replace('\\', '/');
      if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
      AssetDatabase.CreateFolder(parent, Path.GetFileName(folder));
    }
  }
}
