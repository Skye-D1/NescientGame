using UnityEngine;

//Name: Sam Johnson
//File: Building.cs
//Purpose: Manages the sortingLayer of buildings and their roofs as well as the transparency of the roof

public class Building : MonoBehaviour
{
    GameObject player;
    public float outRadius;
    public float inRadius;
    SpriteRenderer roofSprite;
    public float sortingOrderOffset;
    int frame = 0;
    public Vector2 transparencyRadiusOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        roofSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(frame < 3){
            frame += 1;
        }
        if(frame == 3){
            frame += 1;
            roofSprite.sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + (int)Mathf.Floor(sortingOrderOffset * 10f);
        }
        float distance = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y), new Vector2(transform.position.x, transform.position.y) + transparencyRadiusOffset);
        if(distance-inRadius < outRadius-inRadius){
            roofSprite.color = new Color(1f, 1f, 1f, (distance-inRadius) / (outRadius-inRadius));
        }
    }
}
