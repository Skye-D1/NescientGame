using UnityEngine;
using System;

//Name: Sam Johnson, Skye Drury
//File: TrailLayerSetDynamic.cs
//Purpose: Manages the sortingLayer of the trailrenderer on objects that have a dynamic position

public class TrailLayerSetDynamic : MonoBehaviour
{
    TrailRenderer Renderer;
    Collider2D Collider;
    public float offset;
    bool hasCollider = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer = gameObject.GetComponent<TrailRenderer>();
        try{
            Collider = gameObject.GetComponent<Collider2D>();
        } catch(Exception E){
            hasCollider = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //sets sortinglayer of the trail
        if(hasCollider){
            Renderer.sortingOrder = -(int)Mathf.Floor((transform.position.y + Collider.offset.y + offset)*10);
        }
        else{
            Renderer.sortingOrder = -(int)Mathf.Floor((transform.position.y + offset)*10);
        }
    }
}
