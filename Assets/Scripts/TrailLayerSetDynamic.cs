/*
* Name: TrailLayerSetDynamic.cs
* Author: Sam Johnson, Skye Drury
* Email: samuel.johnson, skye.drury
* Desc: Manages the sortingLayer of the trailrenderer on objects that have a dynamic position
*/

using UnityEngine;
using System;

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
