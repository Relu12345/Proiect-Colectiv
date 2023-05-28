using Gtec.UnityInterface;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Gtec.UnityInterface.BCIManager;

public class ClassSelectionAvailable2048 : MonoBehaviour
{
    private uint _selectedClass = 0;
    private bool _update = false;
    public ERPFlashController2D _flashController;
    private Dictionary<int, SpriteRenderer> _selectedObjects;

    private readonly int maxSelectionCounter = 3;

    private uint selectionCounter = 0;
    private uint currentSelection = 0;

    void Start()
    {
        _selectedObjects = new Dictionary<int, SpriteRenderer>();
        List<ERPFlashObject2D> applicationObjects = _flashController.ApplicationObjects;
        foreach (ERPFlashObject2D applicationObject in applicationObjects)
        {
            SpriteRenderer[] spriteRenderers = applicationObject.GameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer.name.Contains("selected"))
                {
                    _selectedObjects.Add(applicationObject.ClassId, spriteRenderer);
                }
            }
        }

        foreach (KeyValuePair<int, SpriteRenderer> kvp in _selectedObjects)
        {
            kvp.Value.gameObject.SetActive(false);
        }

        //attach to class selection available event
        BCIManager.Instance.ClassSelectionAvailable += OnClassSelectionAvailable;
    }

    void OnApplicationQuit()
    {
        //detach from class selection available event
        BCIManager.Instance.ClassSelectionAvailable -= OnClassSelectionAvailable;
    }

    void Update()
    {
        //TODO ADD YOUR CODE HERE
        if(_update)
        {
            if (_selectedClass > 0 && _selectedClass < 5)
            {
                if (currentSelection == 0)
                {
                    currentSelection = _selectedClass;
                }
                else if (currentSelection == _selectedClass)
                {
                    if (selectionCounter >= maxSelectionCounter)
                    {
                        Manager2048.selection = _selectedClass;
                        currentSelection = 0;
                        selectionCounter = 0;
                    }
                    else
                    {
                        selectionCounter++;
                    }
                }
                else
                {
                    currentSelection = 0;
                    selectionCounter = 0;
                }
            }

            _update = false;
        } 
    }

    /// <summary>
    /// This event is called whenever a new class selection is available. Th
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnClassSelectionAvailable(object sender, EventArgs e)
    {
        ClassSelectionAvailableEventArgs ea = (ClassSelectionAvailableEventArgs)e;
       _selectedClass = ea.Class;
        _update = true;
        Debug.Log(string.Format("Selected class: {0}", ea.Class));
    }
}
