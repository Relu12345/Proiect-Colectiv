using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using Unity.ItemRecever;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    public static int finalPoints;
    public int xSize, ySize; 
    public GameObject block, block_food, highscoreWindow, mainCanvas, posmgr, arrows, btnBack; 

    GameObject head, last_tail;
    List<GameObject> tail;
    public Sprite blockSprite, appleSprite, bodySprite;
    public Sprite[] headSprites, tailSprites;
    private Vector3 oldPosition;
    public static uint selection = 0; 

    KeyCode W = KeyCode.W;
    KeyCode A = KeyCode.A;
    KeyCode S = KeyCode.S;
    KeyCode D = KeyCode.D;

    Vector2 dir;

    public UnityEngine.UI.Image plaque;
    public Text points;
    public Text finalScore;
    // Start is called before the first frame update

    public void ResumeGame()
    {
        mainCanvas.SetActive(true);
        posmgr.SetActive(false);
        arrows.SetActive(true);
        Time.timeScale = 1;
        this.transform.position = new Vector3(0, 0, -10);
    }

    void Start()
    {
        btnBack.SetActive(true);
        Time.timeScale = 0;
        timeBetweenMovements = 2.5f;
        dir = Vector2.right;
        createGrid();
        createPlayer();
        spawnFood();
        ResumeGame();
        block.SetActive(false);
        block_food.SetActive(false);
        isAlive = true;
    }

    private Vector2 getRandomPos(){
        return new Vector2(UnityEngine.Random.Range(-xSize/2+1, xSize/2), UnityEngine.Random.Range(-ySize/2+1, ySize/2)); 
    }

    private bool containedInSnake(Vector2 spawnPos){
        bool isInHead = spawnPos.x == head.transform.position.x && spawnPos.y == head.transform.position.y;
        bool isInTail = false; 
        foreach (var item in tail)
        {
            if(item.transform.position.x == spawnPos.x && item.transform.position.y == spawnPos.y){
                isInTail = true; 
            }
        }
        return isInHead || isInTail;
    }
    GameObject food;
    bool isAlive;

    private void spawnFood(){
        Vector2 spawnPos = getRandomPos();
        while(containedInSnake(spawnPos)){
            spawnPos = getRandomPos();
        }
        food = Instantiate(block_food);
        food.GetComponent<SpriteRenderer>().sprite = appleSprite;
        food.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);
        food.SetActive(true);
    }

    private void createPlayer(){
        head = Instantiate(block) as GameObject; 
        head.GetComponent<SpriteRenderer>().sprite = headSprites[3];
        tail = new List<GameObject>(); 
    }

    private void createGrid(){
        for(int x = 0; x <= xSize; x++){
            GameObject borderBottom = Instantiate(block) as GameObject; 
            borderBottom.GetComponent<SpriteRenderer>().sprite = blockSprite;
            borderBottom.GetComponent<Transform>().position = new Vector3(x-xSize/2, -ySize/2, 0);

            GameObject borderTop = Instantiate(block) as GameObject;
            borderTop.GetComponent<SpriteRenderer>().sprite = blockSprite;
            borderTop.GetComponent<Transform>().position = new Vector3(x-xSize/2, ySize-ySize/2, 0);
        }

        for(int y = 0; y <= ySize; y++){
            GameObject borderRight = Instantiate(block) as GameObject;
            borderRight.GetComponent<SpriteRenderer>().sprite = blockSprite;
            borderRight.GetComponent<Transform>().position = new Vector3(-xSize/2, y-(ySize/2), 0); 

            GameObject borderLeft = Instantiate(block) as GameObject;
            borderLeft.GetComponent<SpriteRenderer>().sprite = blockSprite;
            borderLeft.GetComponent<Transform>().position = new Vector3(xSize-(xSize/2), y-(ySize/2), 0); 
        }
    }

    float passedTime, timeBetweenMovements;

    public GameObject gameOverUI;
    private bool enableTimer = false;
    private float ElapsedTime = 5.0f;

    private void gameOver(){
        isAlive = false; 
        gameOverUI.SetActive(true);
        arrows.SetActive(false);
        enableTimer = true;
        finalScore.text = "Points: " + tail.Count;
        finalPoints = tail.Count;
        points.enabled = false;
        plaque.enabled = false;
    }

    public void restart(){
        SceneManager.LoadScene(2);
    }

    public void change_head(Vector3 oldPosition, Vector3 newPosition)
    {
        if (oldPosition.x == newPosition.x - 1 && oldPosition.y == newPosition.y)
            head.GetComponent<SpriteRenderer>().sprite = headSprites[3];
        else if (oldPosition.x == newPosition.x + 1 && oldPosition.y == newPosition.y)
            head.GetComponent<SpriteRenderer>().sprite = headSprites[2];
        else if (oldPosition.x == newPosition.x && oldPosition.y == newPosition.y - 1)
            head.GetComponent<SpriteRenderer>().sprite = headSprites[1];
        else if (oldPosition.x == newPosition.x && oldPosition.y == newPosition.y + 1)
            head.GetComponent<SpriteRenderer>().sprite = headSprites[0];
        head.GetComponent<SpriteRenderer>().sortingLayerName = "Over";
    }

    public void change_tail(Vector3 oldPosition, Vector3 newPosition, int ok)
    {
        if (ok == 1)
        {
                if (oldPosition.x == newPosition.x - 1 && oldPosition.y == newPosition.y)
                   last_tail.GetComponent<SpriteRenderer>().sprite = tailSprites[2];
                else if (oldPosition.x == newPosition.x + 1 && oldPosition.y == newPosition.y)
                    last_tail.GetComponent<SpriteRenderer>().sprite = tailSprites[3];
                else if (oldPosition.x == newPosition.x && oldPosition.y == newPosition.y - 1)
                    last_tail.GetComponent<SpriteRenderer>().sprite = tailSprites[0];
                else if (oldPosition.x == newPosition.x && oldPosition.y == newPosition.y + 1)
                    last_tail.GetComponent<SpriteRenderer>().sprite = tailSprites[1];
                last_tail.GetComponent<SpriteRenderer>().sortingLayerName = "Behind";
        }
        else
        {
            if (oldPosition.x == newPosition.x - 1 && oldPosition.y == newPosition.y)
                head.GetComponent<SpriteRenderer>().sprite = tailSprites[2];
            else if (oldPosition.x == newPosition.x + 1 && oldPosition.y == newPosition.y)
                head.GetComponent<SpriteRenderer>().sprite = tailSprites[3];
            else if (oldPosition.x == newPosition.x && oldPosition.y == newPosition.y - 1)
                head.GetComponent<SpriteRenderer>().sprite = tailSprites[0];
            else if (oldPosition.x == newPosition.x && oldPosition.y == newPosition.y + 1)
                head.GetComponent<SpriteRenderer>().sprite = tailSprites[1];
            head.GetComponent<SpriteRenderer>().sortingLayerName = "Behind";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if ((selection == 2 || Input.GetKey(S)) && dir != Vector2.up)
        {
            selection = 0;
            dir = Vector2.down;
        }
        else if ((selection == 1 || Input.GetKey(W)) && dir != Vector2.down)
        {
            selection = 0;
            dir = Vector2.up; 
        } 
        else if((selection == 4 || Input.GetKey(D)) && dir != Vector2.left)
        {
            selection = 0;
            dir = Vector2.right;
        } 
        else if((selection == 3 || Input.GetKey(A)) && dir != Vector2.right)
        {
            selection = 0;
            dir = Vector2.left;
        }

        passedTime += Time.deltaTime;
        if(timeBetweenMovements < passedTime && isAlive){
            passedTime = 0;
            // Move
            Vector3 newPosition = head.GetComponent<Transform>().position + new Vector3(dir.x, dir.y, 0);

            // Check if collides with border
            if(newPosition.x >= xSize/2
            || newPosition.x <= -xSize/2
            || newPosition.y >= ySize/2
            || newPosition.y <= -ySize/2){
                gameOver();
            }

            // check if collides with any tail tile
            foreach (var item in tail)
            {
                if(item.transform.position == newPosition){
                    gameOver();
                }
            }
            if(newPosition.x == food.transform.position.x && newPosition.y == food.transform.position.y){
                GameObject newTile = Instantiate(block);
                newTile.SetActive(true);
                newTile.transform.position = food.transform.position;
                DestroyImmediate(food);
                if (tail.Count == 0)
                    change_tail(newPosition, oldPosition, 0);
                else {
                    head.GetComponent<SpriteRenderer>().sprite = bodySprite;
                    head.GetComponent<SpriteRenderer>().sortingLayerName = "Middle";
                }
                tail.Add(head);
                head = newTile;
                change_head(oldPosition, newPosition);
                spawnFood();
                points.text = "Points: " + tail.Count;
            } else {
                if (tail.Count == 0) {
                    head.transform.position = newPosition;
                    change_head(oldPosition, newPosition);
                } else {
                    head.GetComponent<SpriteRenderer>().sprite = bodySprite;
                    head.GetComponent<SpriteRenderer>().sortingLayerName = "Middle";
                    tail.Add(head);
                    last_tail = tail[1];
                    if (tail.Count == 2) {
                        change_tail(newPosition, oldPosition, 1);
                    }
                    else if (tail.Count > 2)
                        change_tail(tail[2].transform.position, last_tail.transform.position, 1);
                    else
                        change_tail(oldPosition, last_tail.transform.position, 1);
                    if (tail.Count - 2 != 0 && tail.Count > 2)
                        if(tail.Count - 2 != 1) {
                            tail[tail.Count - 2].GetComponent<SpriteRenderer>().sprite = bodySprite;
                            tail[tail.Count - 2].GetComponent<SpriteRenderer>().sortingLayerName = "Middle";
                        }
                    head = tail[0];
                    change_head(oldPosition, newPosition);
                    tail.RemoveAt(0);
                    head.transform.position = newPosition;
                }
            }
            oldPosition = newPosition;
        }
        
    }
}
