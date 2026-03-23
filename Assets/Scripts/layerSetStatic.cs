using UnityEngine;

public class layerSetStatic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = -(int)Mathf.Floor((transform.position.y + gameObject.GetComponent<Collider2D>().offset.y)*10);
    }
}
