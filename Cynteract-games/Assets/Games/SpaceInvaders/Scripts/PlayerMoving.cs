using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cynteract.SpaceInvaders;

/// <summary>
/// This script defines the borders of ‘Player’s’ movement. Depending on the chosen handling type, it moves the ‘Player’ together with the pointer.
/// </summary>


[System.Serializable]
public class Borders
{
    [Tooltip("offset from viewport borders for player's movement")]
    public float minXOffset = 1.5f, maxXOffset = 1.5f, minYOffset = 1.5f, maxYOffset = 1.5f;
    [HideInInspector] public float minX, maxX, minY, maxY;
}

public class PlayerMoving : MonoBehaviour {

    [Tooltip("offset from viewport borders for player's movement")]
    public Borders borders;
    Camera mainCamera;
    public static PlayerMoving instance; //unique instance of the script for easy access to the script

    Rigidbody2D rb;
    [SerializeField] [Range(0, 1)] float LerpConstant;
    public float baseSpeed=1;

    //Other stuff here
    void FixedUpdate()
    {
        float h = SpaceInvaderInput.GetAxis(SpaceInvaderInput.move)*2-1;
        Vector2 movement = new Vector2(h, rb.velocity.y);
        rb.velocity = Vector2.Lerp(rb.velocity, movement * 8*baseSpeed, LerpConstant);


        transform.position = new Vector3(
        Mathf.Clamp(transform.position.x, borders.minX, borders.maxX), Mathf.Clamp(transform.position.y, borders.minY, borders.maxY),0);
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        GetComponent<PlayerShooting>().shootingIsActive = true;
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        ResizeBorders();                //setting 'Player's' moving borders deending on Viewport's size
    }

//    private void Update()
//    {
//        if (controlIsActive && ButtonManager.gameStarted == true)
//        {
//#if UNITY_STANDALONE || UNITY_EDITOR    //if the current platform is not mobile, setting mouse handling 

//            if (Input.GetMouseButton(0)) //if mouse button was pressed       
//            {
//                GetComponent<PlayerShooting>().shootingIsActive = true;
//                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition); //calculating mouse position in the worldspace
//                mousePosition.z = transform.position.z;
//                transform.position = Vector3.MoveTowards(transform.position, mousePosition, 30 * Time.deltaTime);
//                //if (!EventSystem.current.IsPointerOverGameObject())
//                //{
//                //}
//            }
//            //if (Input.GetAxis("horizontal"))
//            //{

//            //}
//#endif
//            
//        }
//    }

    //setting 'Player's' movement borders according to Viewport size and defined offset
    void ResizeBorders() 
    {
        borders.minX = mainCamera.ViewportToWorldPoint(Vector2.zero).x + borders.minXOffset;
        borders.minY = mainCamera.ViewportToWorldPoint(Vector2.zero).y + borders.minYOffset;
        borders.maxX = mainCamera.ViewportToWorldPoint(Vector2.right).x - borders.maxXOffset;
        borders.maxY = mainCamera.ViewportToWorldPoint(Vector2.up).y - borders.maxYOffset;
    }
}
