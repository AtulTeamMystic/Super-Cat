using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

public class StartButton : MonoBehaviour
{
    [Header("Loading UI")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingBar;
    [SerializeField] private RectTransform loadingCatIcon;
    [SerializeField] private float fillDuration = 1.5f;

    public void StartGame()
    {
        if (PlayerData.instance != null && PlayerData.instance.ftueLevel == 0)
        {
            PlayerData.instance.ftueLevel = 1;
            PlayerData.instance.Save();
#if UNITY_ANALYTICS
            AnalyticsEvent.FirstInteraction("start_button_pressed");
#endif
        }

#if UNITY_PURCHASING
        var module = StandardPurchasingModule.Instance();
#endif

        if (loadingBar != null)
        {
            StartCoroutine(LoadSceneWithProgressRoutine());
        }
        else
        {
            SceneManager.LoadScene("main");
        }
    }

    private IEnumerator LoadSceneWithProgressRoutine()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        transform.GetChild(0).GetComponent<Text>().text = "";

        loadingBar.fillAmount = 0f;
        UpdateCatIconPosition(0f);

        float elapsed = 0f;
        while (elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            float fillAmount = Mathf.Clamp01(elapsed / fillDuration);
            loadingBar.fillAmount = fillAmount;
            UpdateCatIconPosition(fillAmount);
            yield return null;
        }

        loadingBar.fillAmount = 1f;
        UpdateCatIconPosition(1f);
        yield return null;

        SceneManager.LoadScene("main");
    }

    private void UpdateCatIconPosition(float fillAmount)
    {
        if (loadingCatIcon == null || loadingBar == null) return;

        RectTransform barRect = loadingBar.rectTransform;
        float barWidth = barRect.rect.width;
        float localX = (fillAmount - barRect.pivot.x) * barWidth;

        Vector3 localPos = loadingCatIcon.localPosition;
        localPos.x = localX;
        loadingCatIcon.localPosition = localPos;
    }
}


