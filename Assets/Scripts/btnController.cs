using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class btnController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject lvlSelect;

    public void ExitGame()
    {
        Application.Quit();
    }

    public void DismissLeaderboardEntry()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
    
    public void SelectLevel(int choice)
    {
        LeaderBoardInfo.SetLevel(choice);
        Debug.Log(LeaderBoardInfo.GetLevel());
    }

    public void StartLevel()
    {
        SceneManager.LoadScene("Level_" + LeaderBoardInfo.GetLevel(), LoadSceneMode.Single);
    }

    public void ChangeMenu()
    {
        if (!(mainMenu == null || lvlSelect == null))
        {
            if (mainMenu.activeSelf == true)
            {
                lvlSelect.SetActive(true);
                mainMenu.SetActive(false);
            }
            else
            {
                lvlSelect.SetActive(false);
                mainMenu.SetActive(true);
            }
        }
    }

}
