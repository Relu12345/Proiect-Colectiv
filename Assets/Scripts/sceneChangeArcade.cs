using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChangeArcade : MonoBehaviour
{
    public int scene;
    void OnTriggerEnter(Collider Other)
    {
        SceneManager.LoadScene(scene);
    }

}
