using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Skin
{
    public string skinName;
    public int cost;
    public Sprite icon;
    public Texture2D texture; // The custom base map texture for this skin
}

[CreateAssetMenu(fileName = "Skins", menuName = "Trash Dash/Skins Database")]
public class SkinDatabase : ScriptableObject
{
    public string basePrefabName = "Trash Cat"; // The global base prefab key for all skins
    public Skin[] skins;

    static protected SkinDatabase m_Instance;
    static public SkinDatabase instance { get { return m_Instance; } }

    static protected Dictionary<string, Skin> m_SkinsDict;
    static public Dictionary<string, Skin> dictionary { get { return m_SkinsDict; } }

    static protected bool m_Loaded = false;
    static public bool loaded { get { return m_Loaded; } }

    public void Load()
    {
        m_Instance = this;
        if (m_SkinsDict == null)
        {
            m_SkinsDict = new Dictionary<string, Skin>();
            for (int i = 0; i < skins.Length; ++i)
            {
                if (skins[i] != null && !string.IsNullOrEmpty(skins[i].skinName))
                {
                    m_SkinsDict.Add(skins[i].skinName, skins[i]);
                }
            }
            m_Loaded = true;
        }
    }

    static public Skin GetSkin(string name)
    {
        if (m_SkinsDict == null)
            return null;

        Skin s;
        return m_SkinsDict.TryGetValue(name, out s) ? s : null;
    }

    /// <summary>
    /// Swaps the base map texture on the character instance's SkinnedMeshRenderer(s) (ignoring accessories)
    /// </summary>
    public static void ApplySkinTexture(GameObject characterInstance, Texture2D texture)
    {
        if (characterInstance == null)
        {
            Debug.LogWarning("ApplySkinTexture: characterInstance is null!");
            return;
        }
        if (texture == null)
        {
            Debug.LogWarning("ApplySkinTexture: texture is null!");
            return;
        }

        Debug.Log("ApplySkinTexture: Swapping texture to " + texture.name + " on " + characterInstance.name);

        Renderer[] renderers = characterInstance.GetComponentsInChildren<Renderer>(true);
        Debug.Log("ApplySkinTexture: Found " + renderers.Length + " renderers on " + characterInstance.name);

        foreach (var r in renderers)
        {
            // Skip accessories
            if (r.GetComponentInParent<CharacterAccessories>() != null)
            {
                Debug.Log("ApplySkinTexture: Skipping accessory renderer: " + r.name);
                continue;
            }

            // Only update SkinnedMeshRenderer and MeshRenderer (skip ParticleSystemRenderer, Projector, etc.)
            if (!(r is SkinnedMeshRenderer) && !(r is MeshRenderer))
                continue;

            Material[] mats = r.materials;
            Debug.Log("ApplySkinTexture: Renderer " + r.name + " has " + mats.Length + " materials.");
            bool changed = false;
            for (int i = 0; i < mats.Length; ++i)
            {
                if (mats[i] != null)
                {
                    Debug.Log("ApplySkinTexture: Material name: " + mats[i].name + ", shader: " + mats[i].shader.name);
                    if (mats[i].HasProperty("_BaseMap"))
                    {
                        mats[i].SetTexture("_BaseMap", texture);
                        changed = true;
                        Debug.Log("ApplySkinTexture: Successfully set _BaseMap on " + mats[i].name);
                    }
                    if (mats[i].HasProperty("_MainTex"))
                    {
                        mats[i].SetTexture("_MainTex", texture);
                        changed = true;
                        Debug.Log("ApplySkinTexture: Successfully set _MainTex on " + mats[i].name);
                    }
                }
            }
            if (changed)
            {
                r.materials = mats;
            }
        }
    }
}
