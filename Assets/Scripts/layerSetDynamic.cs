using UnityEngine;

public class layerSetDynamic : MonoBehaviour
{
    SpriteRenderer Renderer;
    Collider2D Collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer = gameObject.GetComponent<SpriteRenderer>();
        Collider = gameObject.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Renderer.sortingOrder = -(int)Mathf.Floor((transform.position.y + Collider.offset.y)*10);
    }
}
