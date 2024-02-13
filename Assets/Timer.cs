using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public static Timer instance;
    private bool isPaused = false;
    public int currentTime = 60 * 60 * 30;
    public TMPro.TextMeshProUGUI timerText;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        instance.update();
    }


    private void update() {
        if (!isPaused) {
            currentTime--;
        }

        timerText.text = "" + currentTime / 20;

        if (currentTime < 0) {
            // end game
        }
    }

    public void setPaused(bool paused) {
        isPaused = paused;
    }


}
