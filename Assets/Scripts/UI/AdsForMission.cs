using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AdsForMission : MonoBehaviour
{
    public MissionUI missionUI;

    public Text newMissionText;
    public Button adsButton;

    void OnEnable ()
    {
        adsButton.gameObject.SetActive(false);
        newMissionText.gameObject.SetActive(false);

        // Only present an ad offer if less than 3 missions.
        if (PlayerData.instance.missions.Count >= 3)
        {
            return;
        }

        var isReady = AdMobManager.instance.IsRewardedAdReady();
        newMissionText.gameObject.SetActive(isReady);
        adsButton.gameObject.SetActive(isReady);
    }

    public void ShowAds()
    {
        if (AdMobManager.instance.IsRewardedAdReady())
        {
            AdMobManager.instance.ShowRewardedAd(
                onRewardEarned: () => {
                    AddNewMission();
                },
                onAdClosed: () => {
                    // Do nothing
                }
            );
        }
    }

    void AddNewMission()
    {
        PlayerData.instance.AddMission();
        PlayerData.instance.Save();
        StartCoroutine(missionUI.Open());
    }
}
