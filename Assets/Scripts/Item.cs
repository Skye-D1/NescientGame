using UnityEngine;

//Name: Sam Johnson, Skye Drury
//File: Item.cs
//Purpose: Manages holding data about items, layering them correctly, and glow effect

public class Item : MonoBehaviour
{
    public int itemID = 0; // what item is it?
    public float power = 0f; // heal amount, water level, durability, etc...
    float opacityRate = 1f; // speed for glow pulse
    float glowOpacity = 0f; // current glow opacity
    SpriteRenderer glowSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        glowSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        if(transform.parent != null){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 20000;
            opacityRate = 0;
        }
    }

    // yea
    void Update()
    {
        glowOpacity += Time.deltaTime * opacityRate;
        // opacity limit and flip direction on limit
        if (glowOpacity > 1 || glowOpacity < 0) {
            glowOpacity = glowOpacity > 0.5 ? 1 : 0;
            opacityRate *= -1;
        }
        glowSprite.color = new Color(1f,1f,1f,(glowOpacity));
    }
}
