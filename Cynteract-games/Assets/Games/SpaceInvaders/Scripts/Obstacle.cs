using UnityEngine;

public class Obstacle  : MonoBehaviour {

    public GameObject destructionVFX;
   
    //when colliding with another object, if another objct is 'Player', sending command to the 'Player'
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.tag == "Player") 
        {
            Player.instance.GetDamage(1);
        }
    }
    [Tooltip("Moving speed on Y axis in local space")]
    public float speed;
    public float currentSpeed;
    public float accelleration;

    //moving the object with the defined speed
    private void Update()
    { 
        
        if (currentSpeed<speed)
        {
            currentSpeed = Mathf.Clamp(currentSpeed + accelleration * Time.deltaTime,0, speed);
        }
        transform.Translate(Vector3.down * currentSpeed * Time.deltaTime);
    }
    public void GetDamage(int damage)
    {
        float speedReduction = (float)damage / (4*((PlayerShooting.instance.gunsLevel + 1)));
        speed -= speedReduction;
        currentSpeed -= speedReduction*4;
        if (speed<=0)
        {
            Destruction();
        }
    }
    public void Destruction()
    {
        Instantiate(destructionVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
