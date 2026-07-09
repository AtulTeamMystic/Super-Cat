using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
   public GameObject bgPanel;
    public AudioMixer mixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider masterSFXSlider;

    public LoadoutState loadoutState;
    public DataDeleteConfirmation confirmationPopup;

    protected float m_MasterVolume;
    protected float m_MusicVolume;
    protected float m_MasterSFXVolume;

    protected const float k_MinVolume = -80f;
    protected const string k_MasterVolumeFloatName = "MasterVolume";
    protected const string k_MusicVolumeFloatName = "MusicVolume";
    protected const string k_MasterSFXVolumeFloatName = "MasterSFXVolume";
    
    public void Open()
    {
        bgPanel.SetActive(true);
        LoadoutState.FindObjectOfType<LoadoutState>().charPosition.gameObject.SetActive(false);
        gameObject.SetActive(true);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.3f);
        UpdateUI();
    }

    public void Close()
    {
        bgPanel.SetActive(false);
        LoadoutState.FindObjectOfType<LoadoutState>().charPosition.gameObject.SetActive(true);
        if (DataDeleteConfirmation.FindObjectOfType<DataDeleteConfirmation>()!= null)
        {
            Debug.Log("Found");
            DataDeleteConfirmation.FindObjectOfType<DataDeleteConfirmation>().gameObject.SetActive(false);
        }
        PlayerData.instance.Save ();
        gameObject.SetActive(false);
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), 0.3f);
    }

    void UpdateUI()
    {
        mixer.GetFloat(k_MasterVolumeFloatName, out m_MasterVolume);
        mixer.GetFloat(k_MusicVolumeFloatName, out m_MusicVolume);
        mixer.GetFloat(k_MasterSFXVolumeFloatName, out m_MasterSFXVolume);

        masterSlider.value = 1.0f - (m_MasterVolume / k_MinVolume);
        musicSlider.value = 1.0f - (m_MusicVolume / k_MinVolume);
        masterSFXSlider.value = 1.0f - (m_MasterSFXVolume / k_MinVolume);
    }

    public void DeleteData()
    {
        confirmationPopup.Open(loadoutState);
    }


    public void MasterVolumeChangeValue(float value)
    {
        m_MasterVolume = k_MinVolume * (1.0f - value);
        mixer.SetFloat(k_MasterVolumeFloatName, m_MasterVolume);
		PlayerData.instance.masterVolume = m_MasterVolume;
    }

    public void MusicVolumeChangeValue(float value)
    {
        m_MusicVolume = k_MinVolume * (1.0f - value);
        mixer.SetFloat(k_MusicVolumeFloatName, m_MusicVolume);
		PlayerData.instance.musicVolume = m_MusicVolume;
    }

    public void MasterSFXVolumeChangeValue(float value)
    {
        m_MasterSFXVolume = k_MinVolume * (1.0f - value);
        mixer.SetFloat(k_MasterSFXVolumeFloatName, m_MasterSFXVolume);
		PlayerData.instance.masterSFXVolume = m_MasterSFXVolume;
    }
}
