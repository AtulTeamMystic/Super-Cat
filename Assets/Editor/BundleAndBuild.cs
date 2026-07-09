using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

public class BundleAndBuild
{
	[MenuItem("Build/Build Project with Addressables")]
	public static void BuildProjectWithAddressables()
	{
		BuildPlayer();
		BuildAddressables();
	}

	private static void BuildPlayer()
	{
		BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "C:/Users/ADMIN/Documents/LiveProjects/SuperCat/SuperCatUnity/Build/Supercat.apk", BuildTarget.Android, BuildOptions.None);
	}

	private static void BuildAddressables()
	{
		AddressableAssetSettings.CleanPlayerContent();
		AddressableAssetSettings.BuildPlayerContent();
	}
}
