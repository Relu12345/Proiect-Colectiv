using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void scenaSnake()
    {
        SceneManager.LoadScene("snake");
    }

    public void scenaArcade()
    {
        SceneManager.LoadScene("arcade");
    }
}
