using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene("Level_"+ LeaderBoardInfo.GetLevel(), LoadSceneMode.Single);
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
