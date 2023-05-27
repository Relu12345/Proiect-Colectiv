using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.Data.Common;
using UnityEngine.UI;

[System.Serializable]
public class highscoreManager : MonoBehaviour
{
    private string connectionString;

    public static List<highscore> highscores = new List<highscore>();

    public GameObject scorePrefab;

    public Transform scoreParent;

    public int topRanks;
    public static int refRanks;

    public int saveScores;

    public InputField enterName;

    public GameObject nameDialog;

    public GameObject exitButton;

    // Start is called before the first frame update
    void Start()
    {
        refRanks = topRanks;
        connectionString = "URI=file:" + Application.persistentDataPath + "/Highscore.db";
        Debug.Log(connectionString);
        CreateTable();
        DeleteExtraScore();
        ShowScores();
    }

    private void CreateTable()
    {
        using (IDbConnection conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (IDbCommand dbCmd = conn.CreateCommand())
            {
                string query = "CREATE TABLE if not exists snake (ID INTEGER NOT NULL UNIQUE,Name TEXT NOT NULL,Score INTEGER NOT NULL,PRIMARY KEY(ID AUTOINCREMENT))";

                dbCmd.CommandText = query;

                dbCmd.ExecuteScalar();

                conn.Close();
            }
        }
    }

    public void EnterName()
    {
        if (enterName.text != string.Empty)
        {
            int score = Snake.finalPoints;
            InsertScore(enterName.text, score);
            Debug.Log(enterName.text + " " + score);
            enterName.text = string.Empty;
            nameDialog.SetActive(false);
            exitButton.SetActive(true);

            ShowScores();
        }
    }
    private void InsertScore(string name, int newScore)
    {
        GetScores();

        int hsCount = highscores.Count;

        if (highscores.Count > 0)
        {
            highscore lowestScore = highscores[highscores.Count - 1];

            if (lowestScore != null && saveScores > 0 && highscores.Count >= saveScores && newScore > lowestScore.Score)
            {
                DeleteScore(lowestScore.ID);
                hsCount--;
            }
        }

        if (hsCount < saveScores)
        {
            using (IDbConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                using (IDbCommand dbCmd = conn.CreateCommand())
                {
                    string query = string.Format("INSERT INTO snake(Name,Score) VALUES(\"{0}\", {1})", name, newScore);

                    dbCmd.CommandText = query;

                    dbCmd.ExecuteScalar();

                    conn.Close();
                }
            }
        }
    }

    private void GetScores() 
    {
        highscores.Clear();
        using (IDbConnection conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (IDbCommand dbCmd = conn.CreateCommand())
            {
                string query = "SELECT * FROM snake";

                dbCmd.CommandText = query;

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        highscores.Add(new highscore(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
                    }
                    
                    conn.Close();
                    reader.Close();
                }
            } 
        }

        highscores.Sort();
    }

    private void DeleteScore(int id)
    {
        using (IDbConnection conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (IDbCommand dbCmd = conn.CreateCommand())
            {
                string query = string.Format("DELETE FROM snake WHERE ID={0}", id);

                dbCmd.CommandText = query;

                dbCmd.ExecuteScalar();

                conn.Close();
            }
        }
    }

    private void ShowScores()
    {
        GetScores();

        foreach (GameObject score in GameObject.FindGameObjectsWithTag("Score"))
        {
            Destroy(score);
        }

        for (int i = 0; i < topRanks; i++)
        {
            if (i <= highscores.Count - 1)
            {
                GameObject tmpObject = Instantiate(scorePrefab);

                highscore tmpScore = highscores[i];

                tmpObject.GetComponent<highscoreScript>().SetScore("#" + (i + 1).ToString(), tmpScore.Name, tmpScore.Score.ToString());

                tmpObject.transform.SetParent(scoreParent);

                tmpObject.GetComponent<RectTransform>().localScale = new Vector3(0.25f, 0.3f, 1);
            }
        }
    }

    private void DeleteExtraScore()
    {
        GetScores();

        if (saveScores < highscores.Count)
        {
            int deleteCount = highscores.Count - saveScores;
            highscores.Reverse();

            using (IDbConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                using (IDbCommand dbCmd = conn.CreateCommand())
                {
                    for (int i = 0; i < deleteCount; i++)
                    {
                        string query = string.Format("DELETE FROM snake WHERE ID={0}", highscores[i].ID);

                        dbCmd.CommandText = query;

                        dbCmd.ExecuteScalar();
                    }
                    
                    conn.Close();
                }
            }
        }
    }
}
