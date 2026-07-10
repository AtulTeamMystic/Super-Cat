#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AdMobSettingsSetup
{
    static AdMobSettingsSetup()
    {
        // Delay execution to make sure package assemblies are fully loaded
        EditorApplication.delayCall += SetupSettings;
    }

    private static void SetupSettings()
    {
        var settingsType = System.Type.GetType("GoogleMobileAds.Editor.GoogleMobileAdsSettings, GoogleMobileAds.Editor");
        if (settingsType != null)
        {
            var loadInstanceMethod = settingsType.GetMethod("LoadInstance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (loadInstanceMethod != null)
            {
                var settings = loadInstanceMethod.Invoke(null, null);
                if (settings != null)
                {
                    var androidAppIdProp = settingsType.GetProperty("GoogleMobileAdsAndroidAppId");
                    var iosAppIdProp = settingsType.GetProperty("GoogleMobileAdsIOSAppId");
                    
                    if (androidAppIdProp != null)
                        androidAppIdProp.SetValue(settings, "ca-app-pub-3940256099942544~3347511713");
                    if (iosAppIdProp != null)
                        iosAppIdProp.SetValue(settings, "ca-app-pub-3940256099942544~1458002511");
                        
                    EditorUtility.SetDirty((UnityEngine.Object)settings);
                    AssetDatabase.SaveAssets();
                    Debug.Log("AdMobSettingsSetup: Configured AdMob test app IDs successfully.");
                }
                else
                {
                    Debug.LogWarning("AdMobSettingsSetup: Failed to load GoogleMobileAdsSettings instance.");
                }
            }
            else
            {
                Debug.LogWarning("AdMobSettingsSetup: Failed to find LoadInstance method.");
            }
        }
    }
}
#endif
