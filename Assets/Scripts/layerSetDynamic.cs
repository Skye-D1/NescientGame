using UnityEngine;
using System;

public class layerSetDynamic : MonoBehaviour
{
    SpriteRenderer Renderer;
    Collider2D Collider;
    public float offset;
    bool hasCollider = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer = gameObject.GetComponent<SpriteRenderer>();
        try{
            Collider = gameObject.GetComponent<Collider2D>();
        } catch(Exception E){
            hasCollider = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(hasCollider){
            Renderer.sortingOrder = -(int)Mathf.Floor((transform.position.y + Collider.offset.y + offset)*10);
        }
        else{
            Renderer.sortingOrder = -(int)Mathf.Floor((transform.position.y + offset)*10);
        }
    }
}
