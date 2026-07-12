using UnityEngine;
using UnityEditor;

public class SkinsGeneratorWindow : EditorWindow
{
    public SkinDatabase databaseAsset;
    public string basePrefabName = "Trash Cat";

    // Separate arrays for bulk editing
    public string[] skinNames = new string[10];
    public int[] skinCosts = new int[10];
    public Sprite[] skinIcons = new Sprite[10];
    public Texture2D[] skinTextures = new Texture2D[10];

    private SerializedObject serializedWindow;
    private SerializedProperty namesProp;
    private SerializedProperty costsProp;
    private SerializedProperty iconsProp;
    private SerializedProperty texturesProp;

    [MenuItem("Trash Dash Debug/Skins Generator")]
    public static void ShowWindow()
    {
        GetWindow<SkinsGeneratorWindow>("Skins Generator");
    }

    private void OnEnable()
    {
        InitializeProps();
    }

    private void InitializeProps()
    {
        if (skinNames == null || skinNames.Length != 10) skinNames = new string[10];
        if (skinCosts == null || skinCosts.Length != 10) skinCosts = new int[10];
        if (skinIcons == null || skinIcons.Length != 10) skinIcons = new Sprite[10];
        if (skinTextures == null || skinTextures.Length != 10) skinTextures = new Texture2D[10];

        if (string.IsNullOrEmpty(skinNames[0]))
            skinNames[0] = "Trash Cat";

        serializedWindow = new SerializedObject(this);
        namesProp = serializedWindow.FindProperty("skinNames");
        costsProp = serializedWindow.FindProperty("skinCosts");
        iconsProp = serializedWindow.FindProperty("skinIcons");
        texturesProp = serializedWindow.FindProperty("skinTextures");
    }

    private Vector2 scrollPosition;

    private void OnGUI()
    {
        if (serializedWindow == null || namesProp == null)
        {
            InitializeProps();
        }

        serializedWindow.Update();

        GUILayout.Label("Skins Database Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");
        databaseAsset = (SkinDatabase)EditorGUILayout.ObjectField("Database Asset", databaseAsset, typeof(SkinDatabase), false);
        basePrefabName = EditorGUILayout.TextField("Base Prefab Name", basePrefabName);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        if (databaseAsset != null)
        {
            if (GUILayout.Button("Load Current Database Data", GUILayout.Height(30)))
            {
                LoadFromDatabase();
            }
        }

        EditorGUILayout.Space();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Render the array fields natively to support bulk drag-and-drop
        EditorGUILayout.LabelField("Bulk Configurations (10 Elements each)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(namesProp, new GUIContent("Skin Names (1 to 10)"), true);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(costsProp, new GUIContent("Skin Costs (1 to 10)"), true);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(iconsProp, new GUIContent("Skin Icons (1 to 10)"), true);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(texturesProp, new GUIContent("Skin Textures (1 to 10)"), true);
        EditorGUILayout.Space();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();

        serializedWindow.ApplyModifiedProperties();

        GUI.enabled = (databaseAsset != null);
        if (GUILayout.Button("Save & Update Skins Database", GUILayout.Height(40)))
        {
            SaveToDatabase();
        }
        GUI.enabled = true;

        if (databaseAsset == null)
        {
            EditorGUILayout.HelpBox("Please assign a Skins Database ScriptableObject asset at the top to save changes.", MessageType.Warning);
        }
    }

    private void LoadFromDatabase()
    {
        if (databaseAsset == null) return;

        basePrefabName = databaseAsset.basePrefabName;
        
        int length = databaseAsset.skins != null ? databaseAsset.skins.Length : 0;
        
        skinNames = new string[10];
        skinCosts = new int[10];
        skinIcons = new Sprite[10];
        skinTextures = new Texture2D[10];

        for (int i = 0; i < 10; ++i)
        {
            if (i < length && databaseAsset.skins[i] != null)
            {
                skinNames[i] = databaseAsset.skins[i].skinName;
                skinCosts[i] = databaseAsset.skins[i].cost;
                skinIcons[i] = databaseAsset.skins[i].icon;
                skinTextures[i] = databaseAsset.skins[i].texture;
            }
            else
            {
                // Fallbacks
                skinNames[i] = (i == 0) ? "Trash Cat" : "";
                skinCosts[i] = 0;
                skinIcons[i] = null;
                skinTextures[i] = null;
            }
        }
        
        InitializeProps();
        Repaint();
        Debug.Log("Successfully loaded skin data from database asset.");
    }

    private void SaveToDatabase()
    {
        if (databaseAsset == null) return;

        // Record Undo for safety
        Undo.RecordObject(databaseAsset, "Update Skins Database");

        databaseAsset.basePrefabName = basePrefabName;
        databaseAsset.skins = new Skin[10];

        for (int i = 0; i < 10; ++i)
        {
            databaseAsset.skins[i] = new Skin();
            databaseAsset.skins[i].skinName = (skinNames != null && i < skinNames.Length) ? skinNames[i] : "";
            databaseAsset.skins[i].cost = (skinCosts != null && i < skinCosts.Length) ? skinCosts[i] : 0;
            databaseAsset.skins[i].icon = (skinIcons != null && i < skinIcons.Length) ? skinIcons[i] : null;
            databaseAsset.skins[i].texture = (skinTextures != null && i < skinTextures.Length) ? skinTextures[i] : null;
        }

        EditorUtility.SetDirty(databaseAsset);
        AssetDatabase.SaveAssets();
        
        Debug.Log("Successfully generated & updated Skins Database asset: " + databaseAsset.name);
    }
}
