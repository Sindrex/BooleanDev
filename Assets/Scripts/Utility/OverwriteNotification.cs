using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OverwriteNotification : MonoBehaviour {

    public Button overwriteButton;
    public Button cancelButton;
    public GameObject wrapper;

    public void Start()
    {
        cancel();
    }

    public void setOverwrite(UnityAction methodYes)
    {
        print("Setting overwrite method");
        wrapper.SetActive(true);
        removeListeners();

        overwriteButton.onClick.AddListener(methodYes);
        overwriteButton.onClick.AddListener(cancel);
        cancelButton.onClick.AddListener(cancel);
    }

    public void setOverwrite(UnityAction methodYes, UnityAction methodNo)
    {
        print("Setting overwrite methods");
        wrapper.SetActive(true);
        removeListeners();

        overwriteButton.onClick.AddListener(methodYes);
        overwriteButton.onClick.AddListener(cancel);
        cancelButton.onClick.AddListener(methodNo);
        cancelButton.onClick.AddListener(cancel);
    }

    public void removeListeners()
    {
        overwriteButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }

    public void dummyOverwrite()
    {
        print("Dummy overwrite!");
    }

    public void cancel()
    {
        wrapper.SetActive(false);
    }
}
