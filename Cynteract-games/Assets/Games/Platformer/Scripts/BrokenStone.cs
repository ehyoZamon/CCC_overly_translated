using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenStone : MonoBehaviour
{
    public bool reset = false;
    public bool resetOnCollition = false;
    public float resetTime = 0;
    public float tillFallTime = 0.1F;

    private Vector3 orgPos;
    private Vector3 orgEuAng;
    private IEnumerable corutine;
    private bool falling = false;
    private bool fallingAndHit = false;

    private void Start()
    {
        orgPos = transform.position;
        orgEuAng = transform.eulerAngles;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player" && !falling)
        {
            falling = true;
            StartCoroutine(breaking());
            if (reset)
            {
                StartCoroutine(resetEllement());
            }
        }else if (falling && resetOnCollition && collision.gameObject.name != "Player")
        {
            fallingAndHit = true;
        }
    }

    private IEnumerator breaking()
    {
        yield return new WaitForSeconds(tillFallTime);
        this.gameObject.AddComponent<Rigidbody2D>().freezeRotation = true;

        if (!reset)
        {
            Destroy(GetComponent<BrokenStone>());
        }
    }

    private IEnumerator resetEllement()
    {
        if (resetOnCollition)
        {
            yield return new WaitUntil(() => fallingAndHit == true);
        }
        yield return new WaitForSeconds(resetTime);
        Destroy(GetComponent<Rigidbody2D>());
        gameObject.transform.position = orgPos;
        gameObject.transform.eulerAngles = orgEuAng;
        falling = false;
        fallingAndHit = false;
    }
}
