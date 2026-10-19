using UnityEngine;

// Membuat HandTrackingRig otomatis satu kali saat game mulai, dari scene mana pun.
// Prefab harus ada di folder Resources dengan nama "HandTrackingRig"
// (menu Tools > Hand Tracking > Setup Prefab memindahkannya otomatis).
namespace DEMSIFT.HandTracking
{
  public static class HandTrackingSpawner
  {
    private const string PrefabName = "HandTrackingRig";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Spawn()
    {
      // Scene tes (misalnya CursorTest) yang sudah punya rig sendiri tidak perlu dibuatkan lagi.
      if (Object.FindAnyObjectByType<HandTrackingRig>(FindObjectsInactive.Include) != null) return;

      var prefab = Resources.Load<GameObject>(PrefabName);
      if (prefab == null)
      {
        Debug.LogWarning("[HandTracking] Prefab Resources/HandTrackingRig tidak ditemukan. " +
                         "Jalankan menu Tools > Hand Tracking > Setup Prefab.");
        return;
      }

      var rig = Object.Instantiate(prefab);
      rig.name = PrefabName;
    }
  }
}
