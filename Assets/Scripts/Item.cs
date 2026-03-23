using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemID = 0; // what item is it?
    public int power = 0; // heal amount, water level, durability, etc...

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(transform.parent != null){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 20000;
        }
    }
}
