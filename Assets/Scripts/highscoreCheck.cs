using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mono.Data.Sqlite;
using UnityEngine;

public class highscoreCheck : MonoBehaviour
{
    private List<highscore> highscores = new List<highscore>();
    public GameObject EnterName;

    void OnEnable()
    {
        highscores = highscoreManager.highscores;
        if(highscores.Count > 0)
        {
            if (highscores.Last().Score < Snake.finalPoints || highscores.Count < highscoreManager.refRanks)
            {
                EnterName.SetActive(true);
            }
        }
        else
        {
            EnterName.SetActive(true);
        }
    }
}
