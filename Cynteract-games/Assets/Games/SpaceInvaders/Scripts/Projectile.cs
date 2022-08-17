using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the damage and defines whether the projectile belongs to the ‘Enemy’ or to the ‘Player’, whether the projectile is destroyed in the collision, or not and amount of damage.
/// </summary>

public class Projectile : MonoBehaviour {

    public int damage;

    public bool enemyBullet;

    public bool destroyedByCollision;
    public Enemy owner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyBullet)
        {
            if (collision.tag == "Player")
            {
                Player.instance.GetDamage(damage);
                if (destroyedByCollision)
                {
                    Destruction();
                }
            }
        }
        else
        {
             if (collision.tag == "Enemy")
            {
                if (collision.gameObject.GetComponent<Enemy>() == owner)
                {
                    return;
                }
                collision.GetComponent<Enemy>().GetDamage(damage);
                if (destroyedByCollision)
                {
                    GizmoUtility.DrawText(collision.transform.position, "Destroyed hitting Enemy", 1, Color.red);
                    Destruction();
                }
            }
            else if (collision.tag == "Brick")
            { 
                collision.GetComponent<Brick>().GetDamage(damage);
                if (destroyedByCollision)
                {
                    GizmoUtility.DrawText(collision.transform.position, "Destroyed hitting Brick", 1, Color.white);
                    Destruction();
                }
            }
            else if (collision.GetComponent<Obstacle>())
            {

                collision.GetComponent<Obstacle>().GetDamage(damage);
                if (destroyedByCollision)
                {
                    GizmoUtility.DrawText(collision.transform.position, "Destroyed hitting Brick", 1, Color.white);
                    Destruction();
                }
            }
        }



        

    }

    public void Destruction() 
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {

    }
}


