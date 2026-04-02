using UnityEngine;

//Name: Sam Johnson, Skye Drury
//File: Item.cs
//Purpose: Manages holding data about items, layering them correctly, and glow effect

public class Item : MonoBehaviour
{
    public int itemID = 0; // what item is it?
    public float power = 0; // heal amount, water level, durability, etc...
    float pulseSpeed = 1f; // speed for glow pulse
    float glowOpacity;
    SpriteRenderer glowSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(transform.parent != null){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 20000;
        }
    }
}
