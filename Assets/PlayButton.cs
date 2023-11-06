using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    
    public void playButtonPressed() {
        SceneManager.LoadScene("Overworld");
    }

    public void pauseButtonPressed() {
        // <code to stop timer here>

        SceneManager.LoadScene("PauseMenu");

    }

    public void resumeButonPressed() {
        SceneManager.LoadScene("Overworld");
    }

}
