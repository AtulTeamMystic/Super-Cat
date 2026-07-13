using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    public string screenshotFolder = "Screenshots";

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    void CaptureScreenshot()
    {
        
        string folderPath = System.IO.Path.Combine(Application.persistentDataPath, screenshotFolder);
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string screenshotFileName = "Screenshot_" + timestamp + ".png";

        
        string screenshotPath = System.IO.Path.Combine(folderPath, screenshotFileName);

        
        ScreenCapture.CaptureScreenshot(screenshotPath);
        Debug.Log("Screenshot captured and saved to: " + screenshotPath);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            
            CaptureScreenshot();
        }
    }

    [ContextMenu("Clear All Player Prefs")]public void ClearAllPrefs() {    PlayerPrefs.DeleteAll();}
}
