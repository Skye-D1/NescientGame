using UnityEngine;
using System;

//Name: Sam Johnson
//File: layerSetStatic.cs
//Purpose: Manages the sortingLayer of objects that have a static position

public class layerSetStatic : MonoBehaviour
{
    public float offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set sorting layer of object on start
        try{
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = -(int)Mathf.Floor((transform.position.y + gameObject.GetComponent<Collider2D>().offset.y + offset)*10);
        }
        catch(Exception E){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = -(int)Mathf.Floor((transform.position.y + offset)*10);
        }
    }
}
