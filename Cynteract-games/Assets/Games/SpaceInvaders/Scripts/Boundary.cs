using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script defines the size of the ‘Boundary’ depending on Viewport. When objects go beyond the ‘Boundary’, they are destroyed or deactivated.
/// </summary>
public class Boundary : MonoBehaviour {

    BoxCollider2D boundareCollider;
    public bool destroyElementsOnLeave;
    //receiving collider's component and changing boundary borders
    private void Start()
    {
        boundareCollider = GetComponent<BoxCollider2D>();
        //ResizeCollider();
    }

    //changing the collider's size up to Viewport's size multiply 1.5
    void ResizeCollider()
    {
        Vector2 viewportSize = Camera.main.ViewportToWorldPoint(new Vector2(1, 1)) * 2;
        viewportSize.x *= 10f;
        viewportSize.y *= 1f;
        boundareCollider.size = viewportSize;
    }

    //when another object leaves collider
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (destroyElementsOnLeave)
        {
            if (collision.tag == "Bonus")
                Destroy(collision.gameObject);
            else if (collision.tag == "Brick")
                Destroy(collision.gameObject);
            else if (collision.tag == "Bonus")
                Destroy(collision.gameObject);
            else if (collision.tag == "BrickUn")
                Destroy(collision.gameObject);
            else if (collision.tag == "BrickGold")
                Destroy(collision.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile projectile = collision.GetComponent<Projectile>();
        if (projectile)
        {
            projectile.Destruction();
        }
        else if (collision.tag == "Coin")
            Destroy(collision.gameObject);
    }

}
