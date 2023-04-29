using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UI;

public class highscoreScript : MonoBehaviour
{
    public GameObject Score;
    public GameObject ScoreName;
    public GameObject Rank;

    public void SetScore(string rank, string name, string score)
    {
        this.Rank.GetComponent<Text>().text = rank;
        this.ScoreName.GetComponent<Text>().text = name;
        this.Score.GetComponent<Text>().text = score;
    }
}
