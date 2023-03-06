using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    public int xSize, ySize; 
    public GameObject block; 

    GameObject head; 
    public Material headMaterial, tailMaterial; 
    List<GameObject> tail; 

    Vector2 dir;

    public Text points;
    public Text finalScore;
    // Start is called before the first frame update
    public void med()
    {
        timeBetweenMovements -= 0.15f;
    }
    public void hard()
    {
        timeBetweenMovements -= 0.3f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        this.transform.position = new Vector3(0, 0, -10);
    }
    void Start()
    {
        Time.timeScale = 0;
        timeBetweenMovements = 0.5f;
        dir = Vector2.right;
        createGrid();
        createPlayer(); 
        spawnFood(); 
        block.SetActive(false);
        isAlive = true;
    }

    private Vector2 getRandomPos(){
        return new Vector2(Random.Range(-xSize/2+1, xSize/2), Random.Range(-ySize/2+1, ySize/2)); 
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
        food = Instantiate(block);
        food.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);
        food.SetActive(true);
    }

    private void createPlayer(){
        head = Instantiate(block) as GameObject; 
        head.GetComponent<MeshRenderer>().material = headMaterial;
        tail = new List<GameObject>(); 
    }

    private void createGrid(){
        for(int x = 0; x <= xSize; x++){
            GameObject borderBottom = Instantiate(block) as GameObject; 
            borderBottom.GetComponent<Transform>().position = new Vector3(x-xSize/2, -ySize/2, 0);

            GameObject borderTop = Instantiate(block) as GameObject; 
            borderTop.GetComponent<Transform>().position = new Vector3(x-xSize/2, ySize-ySize/2, 0);
        }

        for(int y = 0; y <= ySize; y++){
            GameObject borderRight = Instantiate(block) as GameObject;
            borderRight.GetComponent<Transform>().position = new Vector3(-xSize/2, y-(ySize/2), 0); 

            GameObject borderLeft = Instantiate(block) as GameObject;
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
        enableTimer = true;
        finalScore.text = "Points: " + tail.Count;
        points.enabled = false;
    }

    public void restart(){
        SceneManager.LoadScene("joculet 2d");
    }

    // Update is called once per frame
    void Update()
    {
        if(enableTimer)
        {
            ElapsedTime -= Time.deltaTime;
            if (ElapsedTime < 0)
                restart();
        }
        if(Input.GetKey(KeyCode.S) && dir != Vector2.up){
            dir = Vector2.down;
        } else if(Input.GetKey(KeyCode.W) && dir != Vector2.down)
        {
            dir = Vector2.up; 
        } else if(Input.GetKey(KeyCode.D) && dir != Vector2.left)
        {
            dir = Vector2.right;
        } else if(Input.GetKey(KeyCode.A) && dir != Vector2.right)
        {
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
                head.GetComponent<MeshRenderer>().material = tailMaterial;
                tail.Add(head); 
                head = newTile;
                head.GetComponent<MeshRenderer>().material = headMaterial;
                spawnFood();
                points.text = "Points: " + tail.Count;
            } else {
                if(tail.Count == 0){
                    head.transform.position = newPosition;
                } else {
                    head.GetComponent<MeshRenderer>().material = tailMaterial;
                    tail.Add(head); 
                    head = tail[0];
                    head.GetComponent<MeshRenderer>().material = headMaterial;
                    tail.RemoveAt(0);
                    head.transform.position = newPosition;
                }
            }

        }
        
    }
}
