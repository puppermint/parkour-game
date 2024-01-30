using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class Entry
{
    public string name;
    public int time;

    public Entry(string name, int time)
    {
        this.name = name;
        this.time = time;
    }

}

public class LeaderBoardInfo : MonoBehaviour
{

    List<Entry> lbEntries;
    public TMP_InputField nameInput;
    public TMP_Text timeDisplay;
    public GameObject entryPrefab;
    public GameObject container;
    public Image completionMedal;
    int awardedMedal;
    public Sprite[] medals;

    static int level;

    void Start()
    {
        lbEntries = new List<Entry>();
        GetLeaderboardEntries();
        if (GetLevel() == 0)
        {
            if (PlayerMovement.GetTime() <= 11)
            {
                awardedMedal = 2;
            }
            else if (PlayerMovement.GetTime() <= 25)
            {
                awardedMedal = 1;
            }
            else
            {
                awardedMedal = 0;
            }
        } else if (GetLevel() == 1)
        {
            if (PlayerMovement.GetTime() <= 28)
            {
                awardedMedal = 2;
            }
            else if (PlayerMovement.GetTime() <= 40)
            {
                awardedMedal = 1;
            }
            else
            {
                awardedMedal = 0;
            }
        }

        ChangeLeaderboard();

    }

    private void ChangeLeaderboard()
    {
        if (container != null)
        {
            for (int i = 0; i < lbEntries.Count; i++)
            {
                int j = 0;
                if (GetLevel() == 0)
                {
                    if (lbEntries[i].time <= 11)
                    {
                        j = 2;
                    }
                    else if (lbEntries[i].time <= 25)
                    {
                        j = 1;
                    }
                    else
                    {
                        j = 0;
                    }
                }
                else if (GetLevel() == 1)
                {
                    if (lbEntries[i].time <= 28)
                    {
                        j = 2;
                    }
                    else if (lbEntries[i].time <= 40)
                    {
                        j = 1;
                    }
                    else
                    {
                        j = 0;
                    }
                }


                GameObject obj = Instantiate(entryPrefab);
                obj.transform.SetParent(container.transform, false);
                obj.transform.GetChild(0).GetComponent<TMP_Text>().text = lbEntries[i].name + " : " + lbEntries[i].time;
                obj.transform.GetChild(1).GetComponent<Image>().sprite = medals[j];
            }
        }
    }

    private void Update()
    {
        if (timeDisplay != null && completionMedal != null)
        {
            timeDisplay.text = "Time: " + PlayerMovement.GetTime();
            completionMedal.GetComponent<Image>().sprite = medals[awardedMedal];
        } 
        else if (entryPrefab != null && container != null)
        {
            lbEntries.Clear();
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            GetLeaderboardEntries();
            ChangeLeaderboard();
        }
    }

    private void GetLeaderboardEntries()
    {
        string[] leaderboard = PlayerPrefs.GetString("Leaderboard_"+ GetLevel(), "").Split(',');
        for (int i = 0; i < leaderboard.Length - 2; i += 2)
        {
            Entry temp = new Entry(leaderboard[i], int.Parse(leaderboard[i + 1]));
            lbEntries.Add(temp);
        }
    }

    public void SubmitScore()
    {
        if (nameInput.text.Length == 3)
        {
            Entry newEntry = new Entry(nameInput.text, PlayerMovement.GetTime());
            lbEntries.Add(newEntry);
            SortLeaderboard();
            UpdateLeaderboard();
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }

    void SortLeaderboard()
    {
        for (int i = lbEntries.Count - 1; i > 0; i--)
        {
            if (lbEntries[i].time < lbEntries[i - 1].time)
            {
                Entry temp = lbEntries[i - 1];
                lbEntries[i - 1] = lbEntries[i];
                lbEntries[i] = temp;
            }
        }
    }

    void UpdateLeaderboard()
    {
        string newLB = "";

        for (int i = 0; i < lbEntries.Count; i++)
        {
            newLB += lbEntries[i].name + ",";
            newLB += lbEntries[i].time + ",";
        }

        PlayerPrefs.SetString("Leaderboard_"+ GetLevel(), newLB);
    }

    void ClearList()
    {
        lbEntries.Clear();
    }

    public static void SetLevel(int levelChoice)
    {
        level = levelChoice;
    }

    public static int GetLevel()
    {
        return level;
    }

}