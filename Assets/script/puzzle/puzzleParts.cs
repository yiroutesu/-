using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
public class puzzleParts : XRGrabInteractable
{
    public bool isselected = false;
    void Start()
    {
        selectEntered.AddListener(HandleSelectEntered);
        selectExited.AddListener(HandleSelectExited);
    }

    private void HandleSelectExited(SelectExitEventArgs arg0)
    {
        if(arg0.interactorObject.GetType() == typeof(chessPuzzle))
        {
            isselected = false;
        }
    }

    private void HandleSelectEntered(SelectEnterEventArgs arg0)
    {
        if(arg0.interactorObject.GetType() == typeof(chessPuzzle))
        {
            isselected = true;
        }
    }

    void Update()
    {
    }
}
