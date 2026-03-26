using UnityEngine;

public class Building : MonoBehaviour
{
    GameObject player;
    public float outRadius;
    public float inRadius;
    SpriteRenderer roofSprite;
    bool isFirstFrame = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        roofSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isFirstFrame){
            isFirstFrame = false;
            roofSprite.sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if(distance-inRadius < outRadius-inRadius){
            roofSprite.color = new Color(1f, 1f, 1f, (distance-inRadius) / (outRadius-inRadius));
        }
    }
}
