using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChange : MonoBehaviour
{
    public void LoadScene(int scene)
    {   
        SceneManager.LoadScene(scene);
    }
}
