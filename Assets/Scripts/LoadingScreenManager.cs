using System.Collections;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject splashScreen, homeScreen, homeScreenCat;
    [SerializeField] private float splashDuration = 2f, fadeDuration = 0.5f;
    private CanvasGroup splashCG, homeCG;

    private void Awake()
    {
        if (splashScreen) splashCG = splashScreen.GetComponent<CanvasGroup>();
        if (homeScreen) homeCG = homeScreen.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        StartCoroutine(TransitionRoutine());
    }

    private void SetState(bool splashActive, bool homeActive)
    {
        if (splashScreen) splashScreen.SetActive(splashActive);
        if (homeScreen) homeScreen.SetActive(homeActive);
        if (homeScreenCat) homeScreenCat.SetActive(homeActive);
    }

    private IEnumerator TransitionRoutine()
    {
        SetState(true, homeCG != null);
        if (homeCG) homeCG.alpha = 0f;
        if (splashCG) splashCG.alpha = 1f;

        yield return StaticData.GetWait(splashDuration);

        if (splashCG || homeCG)
        {
            SetState(true, true);
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float p = t / fadeDuration;
                if (splashCG) splashCG.alpha = 1f - p;
                if (homeCG) homeCG.alpha = p;
                yield return null;
            }
            if (splashCG) splashCG.alpha = 0f;
            if (homeCG) homeCG.alpha = 1f;
        }

        SetState(false, true);
    }
}
