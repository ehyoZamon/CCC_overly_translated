using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Platformer
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Components")]
        public Animator animator;
        public new Rigidbody2D rigidbody2D;
        public PlayerEmote playerEmote;

        [Header("Values")]
        public bool keyboardMovement;
        public int jumpHeight = 15;
        public float speed = 10;
        public LayerMask ground;
        public float gravity = 30;
        public float airAcceleration = 10;
        public float coyoteTime = .2f;
        public bool stickToWall;
        public float wallDontFallTime = 1;


        [Header("States")]
        [ShowInInspector, ReadOnly]
        private bool sliding;
        [ShowInInspector, ReadOnly]
        private bool batmaning;
        [ShowInInspector, ReadOnly]
        private bool grounded;
        [ShowInInspector, ReadOnly]
        private bool wallrunningLeft;
        [ShowInInspector, ReadOnly]
        private bool wallrunningRight;
        [ShowInInspector, ReadOnly]
        private bool frozen;

        [Header("State Values")]
        [ShowInInspector, ReadOnly]
        private float slidingAngle;
        [ShowInInspector, ReadOnly]
        private float xSpeed;
        [ShowInInspector, ReadOnly]
        private float ySpeed;
        [ShowInInspector, ReadOnly]
        private float inputX;


        private float jumpPressedTime;
        
        private bool groundedBefore = false;

        public static PlayerMovement instance;
        private Vector2 velocityToAdd;
        private bool justDidJumpAction;
        private float lastGroundedTime;
        private float startedWallRunningTime;
        private bool wallrunningBefore;

        Dictionary<Collider2D, ContactPoint2D[]> colliders = new Dictionary<Collider2D, ContactPoint2D[]>();

        private void Awake()
        {
            instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (frozen) return;
            if (GetJumpAction())
            {
                jumpPressedTime = Time.fixedTime;
            }
            inputX = GetMoveAxis();
            if ((grounded || sliding|| IsInCoyoteTime()) && GetJump())
            {
                animator.SetTrigger("Jump");
                ySpeed = jumpHeight;
                xSpeed = 2 * speed * inputX;
                jumpPressedTime = 0;
            }
            if ((wallrunningLeft || IsInCoyoteTime()) && GetJump())
            {
                animator.SetTrigger("Jump");

                ySpeed = jumpHeight;
                xSpeed = jumpHeight;
                jumpPressedTime = 0;
            }
            if ((wallrunningRight || IsInCoyoteTime()) && GetJump())
            {
                animator.SetTrigger("Jump");

                ySpeed = jumpHeight;
                xSpeed = -jumpHeight;
                jumpPressedTime = 0;
            }
            if (grounded || wallrunningLeft || wallrunningRight)
            {
                playerEmote.SetEmote(PlayerEmote.EmoteType.Happy);
            }
            else
            {
                playerEmote.SetEmote(PlayerEmote.EmoteType.Surprised);
            }
            if (wallrunningLeft)
            {
                playerEmote.Look(new Vector2(1, 1));
            }
            else if (wallrunningRight)
            {
                playerEmote.Look(new Vector2(-1, 1));
            }
            else
            {
                playerEmote.Look(new Vector2(xSpeed, ySpeed));

            }

            playerEmote.SetEyesOpen(!wallrunningLeft, !wallrunningRight);
        }

        private  float GetMoveAxis()
        {
            if (keyboardMovement)
            {
                return Input.GetAxis("Horizontal");
            }
            return JumpAndRunInput.GetAxis(JumpAndRunInput.move) * 2 - 1;
        }

        private bool GetJumpAction()
        {
            if (keyboardMovement)
            {
                return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0);
            }
            else
            {
                if (JumpAndRunInput.GetAction(JumpAndRunInput.jump))
                {
                    if (!justDidJumpAction)
                    { 
                        justDidJumpAction = true;
                        return true;
                    }
                    return false;
                }
                justDidJumpAction = false;
                return false;
            }
        }

        private bool GetJump()
        {
            return Time.fixedTime - jumpPressedTime < .2;
        }

        private bool IsInCoyoteTime()
        {
            return Time.fixedTime - lastGroundedTime < coyoteTime;
        }

        private void FixedUpdate()
        {
            if (frozen) return;
            bool _grounded = false;
            bool _wallrunningLeft = false;
            bool _wallrunningRight = false;
            bool _sliding = false;
            bool _batmaning = false;
            if (colliders.Count == 0)
            {
                velocityToAdd = Vector3.zero;
            }
            foreach (var collider in colliders)
            {
                foreach (var item in collider.Value)
                {
                    Debug.DrawRay(item.point, item.normal);
                    float angle = Vector2.Angle(item.normal, Vector2.up);
                    if (angle < 30)
                    {

                        _grounded = true;
                        MovingPlatform movingPlatform = collider.Key.GetComponent<MovingPlatform>();
                        if (movingPlatform)
                        {
                            velocityToAdd = movingPlatform.velocity;
                        }

                    }
                    else if (angle < 60)
                    {
                        _sliding = true;
                        slidingAngle = angle;
                    }
                    if (item.point.x < transform.position.x && Vector2.Angle(item.normal, Vector2.right) < 30)
                    {
                        _wallrunningLeft = true;
                    }
                    if (item.point.x > transform.position.x && Vector2.Angle(item.normal, Vector2.left) < 30)
                    {
                        _wallrunningRight = true;
                    }
                    if (Vector2.Angle(item.normal, Vector2.down) < 30)
                    {
                        _batmaning = true;
                    }
                }
            }
            sliding = _sliding;
            grounded = _grounded;
            batmaning = _batmaning;
            if (grounded && !groundedBefore)
            {
                animator.SetTrigger("Land");
            }
            groundedBefore = grounded;

            wallrunningLeft = _wallrunningLeft;
            wallrunningRight = _wallrunningRight;
            if ((wallrunningLeft||wallrunningRight)&&!wallrunningBefore&&!grounded)
            {
                startedWallRunningTime = Time.fixedTime;
            }
            wallrunningBefore = wallrunningLeft || wallrunningRight;

            if (batmaning)
            {
                if (ySpeed > 0)
                {
                    ySpeed = 0;
                }
            }
            if (grounded)
            {
                xSpeed = speed * inputX;
                if (ySpeed < 0)
                {
                    ySpeed = 0;
                }
                lastGroundedTime = Time.fixedTime;
            }
            else if (sliding)
            {
                ySpeed -= gravity * Time.fixedDeltaTime * Mathf.Sin(Mathf.Deg2Rad * slidingAngle);
                lastGroundedTime = Time.fixedTime;
            }
            else if (wallrunningLeft || wallrunningRight)
            {
                if (stickToWall)
                {
                    xSpeed = LerpXSpeed(0);
                }
                else
                {
                    xSpeed = LerpXSpeed(inputX);
                }
                if (ySpeed<=0)
                {
                    if (Time.fixedTime - startedWallRunningTime < wallDontFallTime)
                    {
                        ySpeed = 0;
                    }
                    else
                    {
                        ySpeed -= gravity * Time.fixedDeltaTime;
                    }
                }
                else
                {
                    ySpeed -= gravity * Time.fixedDeltaTime;
                    startedWallRunningTime = Time.fixedTime;
                }

            }
            else //falling
            {
                xSpeed = LerpXSpeed(inputX);
                ySpeed -= gravity * Time.fixedDeltaTime;
            }

            if (sliding)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, ySpeed) + velocityToAdd;
            }
            else
            {
                rigidbody2D.velocity = new Vector2(xSpeed, ySpeed) + velocityToAdd;
            }

        }

        private float LerpXSpeed(float v)
        {
            return Mathf.Lerp(xSpeed, speed * v, Time.fixedDeltaTime * airAcceleration);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (colliders.ContainsKey(collision.collider))
            {
                colliders[collision.collider] = collision.contacts;
            }
            else
            {
                colliders.Add(collision.collider, collision.contacts);
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (colliders.ContainsKey(collision.collider))
            {
                colliders.Remove(collision.collider);
            }
        }
        public void Freeze()
        {
            frozen = true;
            rigidbody2D.isKinematic = true;
            ySpeed = 0;
            xSpeed = 0;
            colliders.Clear();
        }
        public void Unfreeze()
        {
            rigidbody2D.isKinematic = false;
            rigidbody2D.WakeUp();
            grounded = false;
            frozen = false;
            colliders.Clear();
        }
        public void ActivatePartay()
        {
            animator.SetBool("Partay", true);
            playerEmote.ActivateSunglasses();
        }
        public void DeactivatePartay()
        {
            animator.SetBool("Partay", false);
            playerEmote.DeactivateSunglasses();
        }
    }
}