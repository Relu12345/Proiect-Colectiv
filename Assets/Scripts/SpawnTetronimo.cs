using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnTetronimo : MonoBehaviour
{
    public GameObject[] Tetrominoes;
    private bool enableTimer = false;
    private float ElapsedTime = 5.0f;
    public GameObject highscoreWindow, gameOverUI;
    public TextMeshPro loseScreenPoints, currentPoints;

    bool IsOccupied(Vector3 position)
    {
        int roundedX = Mathf.RoundToInt(position.x);
        int roundedY = Mathf.RoundToInt(position.y);

        if (TetrisBlock.grid[roundedX, roundedY] != null)
            return true;

        return false;
    }

    void Start()
    {
        NewTetronimo();
    }

    void Update()
    {
        currentPoints.text = "Points: " + TetrisBlock.points;
        if (enableTimer)
        {
            ElapsedTime -= Time.deltaTime;
            if (ElapsedTime < 0)
            {
                highscoreWindow.SetActive(true);
                gameOverUI.SetActive(false);
                enableTimer = false;
            }
        }
    }

    public void NewTetronimo()
    {
        Vector3 spawnPosition = transform.position;

        if (IsOccupied(spawnPosition))
        {
            loseScreenPoints.text = "Points: " + TetrisBlock.points;
            gameOverUI.SetActive(true);
            enableTimer = true;
            Camera.main.transform.position = new Vector3(55, 7, -10);
            return;
        }

        Instantiate(Tetrominoes[Random.Range(0, Tetrominoes.Length)], transform.position, Quaternion.identity);
    }
}
