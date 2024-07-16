using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Unity.Collections;

public class Node : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Action<Node> OnNodeCtrlLeftClicked, OnNodeAltLeftClicked;
    public NodeType nodeType;
    public int nodeIndex;
    public int x;
    public int y;

    //Pathfinding stats
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }
    public Node parentNode;
    public List<Node> Neighbours = new List<Node>();

    
    public void SetNodeSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }   

    void OnMouseDown()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame  && Keyboard.current.leftCtrlKey.isPressed)
        {
            Debug.Log("From <Node.cs> Left Mouse Button Pressed on " + gameObject.name);
            OnNodeCtrlLeftClicked?.Invoke(this);
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame  && Keyboard.current.leftAltKey.isPressed)
        {
            Debug.Log("From <Node.cs> Left Mouse Button Pressed on " + gameObject.name);
            OnNodeAltLeftClicked?.Invoke(this);
        }
    }
}


