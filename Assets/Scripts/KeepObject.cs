using Gtec.UnityInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepObject : MonoBehaviour
{
    public charController CharController;
    private GameObject flashingObj;
    private string Old = "";

    private void Awake()
    {
        KeepObject[] existingInstances = FindObjectsOfType<KeepObject>();
        if (existingInstances.Length > 1)
        {
            // Destroy this instance if another one already exists
            CharController.InitPos();
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void changeClassSelector(string Old, string New)
    {
        if(Old == "arcade" && New == "snake")
        {
            gameObject.GetComponent<ClassSelectionAvailableArcade>().enabled = false;
            gameObject.GetComponent<ClassSelectionAvailableSnake>().enabled = true;
        }
        else if (Old == "snake" && New == "arcade")
        {
            gameObject.GetComponent<ClassSelectionAvailableArcade>().enabled = true;
            gameObject.GetComponent<ClassSelectionAvailableSnake>().enabled = false;
        }
        else if (Old == "arcade" && New == "oridoi")
        {
            gameObject.GetComponent<ClassSelectionAvailableArcade>().enabled = false;
            gameObject.GetComponent<ClassSelectionAvailable2048>().enabled = true;
        }
        else if (Old == "oridoi" && New == "arcade")
        {
            gameObject.GetComponent<ClassSelectionAvailableArcade>().enabled = true;
            gameObject.GetComponent<ClassSelectionAvailable2048>().enabled = false;
        }
    }

    private void changeParameteres(ERPFlashController2D flasher)
    {
        gameObject.GetComponent<ERPFlashController2D>().ApplicationObjects = flasher.ApplicationObjects;
        gameObject.GetComponent<ERPFlashController2D>().FlashtimeMs = flasher.FlashtimeMs;
        gameObject.GetComponent<ERPFlashController2D>().NumberOfClasses = flasher.NumberOfClasses;
        gameObject.GetComponent<ERPFlashController2D>().NumberOfTrainingTrials = flasher.NumberOfTrainingTrials;
        gameObject.GetComponent<ERPFlashController2D>().TrainingObject = flasher.TrainingObject;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "snake")
        {
            flashingObj = GameObject.Find("bciManager2D_snake");
            flashingObj.SetActive(false);
            changeParameteres(flashingObj.GetComponent<ERPFlashController2D>());
            changeClassSelector("arcade", "snake");
            Old = "snake";
        }

        if (scene.name == "oridoi")
        {
            flashingObj = GameObject.Find("bciManager2D_2048");
            flashingObj.SetActive(false);
            changeParameteres(flashingObj.GetComponent<ERPFlashController2D>());
            changeClassSelector("arcade", "oridoi");
            Old = "oridoi";
        }

        else if(scene.name == "arcade" && Old == "snake")
        {
            flashingObj = GameObject.Find("bciManager2D_ref");
            flashingObj.SetActive(false);
            changeParameteres(flashingObj.GetComponent<ERPFlashController2D>());
            changeClassSelector("snake", "arcade");
            Old = "arcade";
        }

        else if (scene.name == "arcade" && Old == "oridoi")
        {
            flashingObj = GameObject.Find("bciManager2D_ref");
            flashingObj.SetActive(false);
            changeParameteres(flashingObj.GetComponent<ERPFlashController2D>());
            changeClassSelector("oridoi", "arcade");
            Old = "arcade";
        }

        else if (scene.name == "arcade" && Old == "")
        {
            flashingObj = GameObject.Find("bciManager2D_ref");
            flashingObj.SetActive(false);
            Old = "arcade";
        }
    }
}