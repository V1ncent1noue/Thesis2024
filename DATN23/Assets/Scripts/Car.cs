using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public class Car : MonoBehaviour
{
    public Package package;
//l
    public Action<Car> OnCarLeftClicked, OnCarMouseEnter, OnCarMouseExit;
    public CarData carData;


    void Start()
    {
        if (package != null)
        {
            package.gameObject.SetActive(false);
        }
    }
    void OnMouseDown()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnCarLeftClicked?.Invoke(this);
        }
    }

    private void OnMouseEnter()
    {
        OnCarMouseEnter?.Invoke(this);
    }

    private void OnMouseExit()
    {
        OnCarMouseExit?.Invoke(this);
    }
}

