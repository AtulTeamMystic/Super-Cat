using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif
using System.Collections.Generic;
using TMPro;

/// <summary>
/// state pushed on top of the GameManager when the player dies.
/// </summary>
public class GameOverState : AState
{
    public TrackManager trackManager;
    public Canvas canvas;
    public MissionUI missionPopup;

    public AudioClip gameOverTheme;

    public Leaderboard miniLeaderboard;
    public Leaderboard fullLeaderboard;
    public GameObject HighScoreBg;
    public GameObject addButton;

    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI yourScoreText;

    private void OnEnable()
    {
        if (HighScoreBg != null)
        {
            HighScoreBg.SetActive(true);
            HighScoreBg.transform.localScale = Vector3.zero;
            LeanTween.scale(HighScoreBg, Vector3.one, 0.3f);
        }
    }

    private void OnDisable()
    {
        if (HighScoreBg != null)
        {
            LeanTween.scale(HighScoreBg, Vector3.zero, 0.3f).setOnComplete(() =>
            {
                HighScoreBg.SetActive(false);
            });
        }
    }

    public override void Enter(AState from)
    {
        canvas.gameObject.SetActive(true);

        int previousHighScore = (PlayerData.instance.highscores.Count > 0) ? PlayerData.instance.highscores[0].score : 0;
        int displayHighScore = Mathf.Max(previousHighScore, trackManager.score);

        if (highScoreText != null)
        {
            highScoreText.text = displayHighScore.ToString();
        }
        if (yourScoreText != null)
        {
            yourScoreText.text = trackManager.score.ToString();
        }

        //miniLeaderboard.playerEntry.inputName.text = PlayerData.instance.previousName;

        //miniLeaderboard.playerEntry.score.text = trackManager.score.ToString();
        //miniLeaderboard.Populate();

        if (PlayerData.instance.AnyMissionComplete())
            StartCoroutine(missionPopup.Open());
        else
            missionPopup.gameObject.SetActive(false);

        CreditCoins();

        if (MusicPlayer.instance.GetStem(0) != gameOverTheme)
        {
            MusicPlayer.instance.SetStem(0, gameOverTheme);
            StartCoroutine(MusicPlayer.instance.RestartAllStems());
        }
    }

    public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);
        FinishRun();
    }

    public override string GetName()
    {
        return "GameOver";
    }

    public override void Tick()
    {

    }

    public void OpenLeaderboard()
    {
        fullLeaderboard.forcePlayerDisplay = false;
        fullLeaderboard.displayPlayer = true;
        fullLeaderboard.playerEntry.playerName.text = miniLeaderboard.playerEntry.inputName.text;
        fullLeaderboard.playerEntry.score.text = trackManager.score.ToString();

        fullLeaderboard.Open();
    }

    public void GoToStore()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("shop", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }


    public void GoToLoadout()
    {
        trackManager.isRerun = false;
        Camera.main.transform.position = new Vector3(0, 5.3f, -3);
        manager.SwitchState("Loadout");
        Debug.Log("Go to Loadout" + PlayerData.instance.coins);
        manager.GetPlayerEssentials();

        //Destroy The Character
        CharacterInputController chrCtrl = trackManager.characterController;
        Destroy(chrCtrl.character.gameObject);
    }

    public void RunAgain()
    {
        trackManager.isRerun = false;
        manager.SwitchState("Game");
    }

    protected void CreditCoins()
    {
        PlayerData.instance.Save();

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        var transactionId = System.Guid.NewGuid().ToString();
        var transactionContext = "gameplay";
        var level = PlayerData.instance.rank.ToString();
        var itemType = "consumable";
        
        if (trackManager.characterController.coins > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Soft, // Currency type
                transactionContext,
                trackManager.characterController.coins,
                "fishbone",
                PlayerData.instance.coins,
                itemType,
                level,
                transactionId
            );
        }

        if (trackManager.characterController.premium > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Premium, // Currency type
                transactionContext,
                trackManager.characterController.premium,
                "anchovies",
                PlayerData.instance.premium,
                itemType,
                level,
                transactionId
            );
        }
#endif
    }

    protected void FinishRun()
    {
        string playerName = PlayerData.instance.previousName;
        if (miniLeaderboard != null && miniLeaderboard.playerEntry != null && miniLeaderboard.playerEntry.inputName != null)
        {
            if (string.IsNullOrEmpty(miniLeaderboard.playerEntry.inputName.text))
            {
                miniLeaderboard.playerEntry.inputName.text = PlayerData.instance.previousName;
            }
            else
            {
                PlayerData.instance.previousName = miniLeaderboard.playerEntry.inputName.text;
            }
            playerName = miniLeaderboard.playerEntry.inputName.text;
        }

        PlayerData.instance.InsertScore(trackManager.score, playerName);

        CharacterCollider.DeathEvent de = trackManager.characterController.characterCollider.deathData;
        //register data to analytics
#if UNITY_ANALYTICS
        AnalyticsEvent.GameOver(null, new Dictionary<string, object> {
            { "coins", de.coins },
            { "premium", de.premium },
            { "score", de.score },
            { "distance", de.worldDistance },
            { "obstacle",  de.obstacleType },
            { "theme", de.themeUsed },
            { "character", de.character },
        });
#endif

        PlayerData.instance.Save();

        trackManager.End();
    }

    //----------------
}
