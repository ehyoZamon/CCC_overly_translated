using UnityEngine;

public class Bonus : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.tag == "Player")
        {
            PlayerShooting.instance.IncreaseGunLevel();
            Destroy(gameObject);
        }
    }


}
