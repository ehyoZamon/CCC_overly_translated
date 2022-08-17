using UnityEngine;
using System.Collections;
using System;

namespace Cynteract.SoccerGame
{
	public class PlayerController : MonoBehaviour
	{

		/// <summary>
		/// Main player controller.
		/// </summary>
		public LayerMask wallLayerMask;
		public static float playerBouncePower = 3.2f;

		//public Texture2D[] availableAvatars;			//different avatar images for player character
		public StatusImage[] availableAvatars;          //different avatar images for player character

		//starting position of player (slider)
		[Range(-7.0f, -3.0f)]
		public float startingX = -5.0f;
		private Vector3 startingPosition;

		//reference to child game objects
		public GameObject myShoe;
		public GameObject myHead;
		private float maxShoeTurn = 20.0f;      //in degrees

		public bool canTurnHead = true;         //flag
		public bool canTurnShoe = true;         //flag

		private float moveSpeed = 8.0f;        //player movement speed
		private float jumpSpeed = 500;          //player jump power (avoid setting it higher than 400)
		private bool canJump = true;
		private bool landCooldown;
		private bool justJumped;

		new Rigidbody rigidbody;
		//init
		void Awake()
		{

			landCooldown = false;

			//reset player's position
			startingPosition = new Vector3(startingX, 0.5f, 0);
			transform.position = startingPosition;

			//change player avatar image based on the selection made from the Menu scene
			myHead.GetComponent<Renderer>().material.mainTexture = availableAvatars[PlayerPrefs.GetInt("selectedAvatar", 0)].normal;
			rigidbody = GetComponent<Rigidbody>();
		}



		void Update()
		{

			rotateShoe();

			//swing the head to the default rotation when player is not moving or jumping
			if (!Input.anyKey && canTurnHead)
			{
				JointSpring js = myHead.GetComponent<HingeJoint>().spring;
				js.targetPosition = 0;
				myHead.GetComponent<HingeJoint>().spring = js;
			}


		}


		/// <summary>
		/// Here we have 2 functions to move the player with keyboard keys, mouse clicks or touch events.
		/// </summary>
		void FixedUpdate()
		{

			/*if(!GameController.isGoalTransition && !GameController.gameIsFinished) {
				movePlayerWithKeyboard();
				StartCoroutine(movePlayerWithTouch());
			}*/
		}

		/// <summary>
		/// Here we have 2 functions to move the player with keyboard keys, mouse clicks or touch events.
		/// </summary>
		void LateUpdate()
		{

			if (!SoccerGameGameController.isGoalTransition && !SoccerGameGameController.gameIsFinished)
			{
				MovePlayerWithGlove();
				//MovePlayerWithKeyboard();
				//StartCoroutine(movePlayerWithTouch());
			}
		}

		private void MovePlayerWithGlove()
		{
			float modifier = 1;
			if (SoccerGameInput.GetAction(SoccerGameInput.jump)&&!justJumped  )
			{
				justJumped = true;
                if (canJump)
                {
					doJump();
                }
			}
            else if (!SoccerGameInput.GetAction(SoccerGameInput.jump))
            {
				justJumped = false;

			}
			var x = SoccerGameInput.GetAxis(SoccerGameInput.move);
			x = x * 2 - 1;
			if (x>0)
			{
				RotateHead(1);
				if (Physics.Raycast(transform.position, transform.right, out RaycastHit hit, 100, wallLayerMask))
				{
					modifier = Mathf.Clamp01( Vector3.Distance(transform.position, hit.point) - 1);
				}
			}
			else
			{
				RotateHead(-1);
				if (Physics.Raycast(transform.position, -transform.right, out RaycastHit hit, 100, wallLayerMask))
				{
					modifier = Mathf.Clamp01(Vector3.Distance(transform.position, hit.point));


				}
			}
			rigidbody.velocity = new Vector3(x * moveSpeed*modifier, rigidbody.velocity.y, 0);

		}


		/// <summary>
		/// Swing the head of the player avatar
		/// </summary>
		void RotateHead(int _dir)
		{

			if (!canTurnHead)
				return;

			if (myHead)
			{
				//swing the head by setting a new rotation for the spring
				JointSpring js = myHead.GetComponent<HingeJoint>().spring;
				js.targetPosition = 15 * _dir;
				myHead.GetComponent<HingeJoint>().spring = js;
			}
		}


		/// <summary>
		/// Rotate player shows when we are moving him around.
		/// reset rotation when player is not moving.
		/// </summary>
		void rotateShoe()
		{

			if (!myShoe || !canTurnShoe)
				return;

			float rotateMinVelocity = 0.1f;

			if (GetComponent<Rigidbody>().velocity.x >= rotateMinVelocity)
			{
				myShoe.transform.localEulerAngles = new Vector3(0, maxShoeTurn, 180);
			}
			else if (GetComponent<Rigidbody>().velocity.x <= rotateMinVelocity * -1)
			{
				myShoe.transform.localEulerAngles = new Vector3(0, maxShoeTurn * -1, 180);
			}
			else
			{
				myShoe.transform.localEulerAngles = new Vector3(0, 0, 180);
			}
		}


		/// <summary>
		/// Move player with keyboard keys.
		/// </summary>
		void MovePlayerWithKeyboard()
		{

			if (Input.GetKey(KeyCode.LeftArrow))
			{
				MoveLeft();
			}

			if (Input.GetKey(KeyCode.RightArrow))
			{
				MoveRight();
			}

			if (Input.GetKeyDown(KeyCode.Space) && canJump)
			{
				doJump();
			}
		}


		/// <summary>
		/// We use a simple multitouch (detects 2 touches at once) routine to monitor 
		/// user interaction with UI buttons.
		/// </summary>
		private RaycastHit hitInfo;
		private RaycastHit hitInfo2;
		private Ray ray;
		private Ray ray2;
		IEnumerator movePlayerWithTouch()
		{

			if (Input.touches.Length > 0)
			{
				ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
				if (Input.touches.Length > 1)
					ray2 = Camera.main.ScreenPointToRay(Input.touches[1].position);
				else
					ray2 = Camera.main.ScreenPointToRay(Input.touches[0].position);
			}
			else if (Input.GetMouseButton(0))
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			else
				yield break;

			if (Physics.Raycast(ray, out hitInfo))
			{
				GameObject objectHit = hitInfo.transform.gameObject;
				switch (objectHit.name)
				{
					case "Button-Jump":
						if (canJump)
							doJump();
						break;
					case "Button-Left":
						MoveLeft();
						break;
					case "Button-Right":
						MoveRight();
						break;
				}
			}

			if (Physics.Raycast(ray2, out hitInfo2))
			{
				GameObject objectHit2 = hitInfo2.transform.gameObject;
				switch (objectHit2.name)
				{
					case "Button-Jump":
						if (canJump)
							doJump();
						break;
					case "Button-Left":
						MoveLeft();
						break;
					case "Button-Right":
						MoveRight();
						break;
				}
			}
		}


		/// <summary>
		/// Change status image for avatar face
		/// </summary>
		public IEnumerator changeFaceStatus()
		{
			myHead.GetComponent<Renderer>().material.mainTexture = availableAvatars[PlayerPrefs.GetInt("selectedAvatar", 0)].hit;
			yield return new WaitForSeconds(0.3f);
			myHead.GetComponent<Renderer>().material.mainTexture = availableAvatars[PlayerPrefs.GetInt("selectedAvatar", 0)].normal;
		}


		void MoveLeft()
		{
			//print ("Left arrow pressed!");
			rigidbody.velocity = new Vector3(-moveSpeed, rigidbody.velocity.y, 0);
			RotateHead(-1);
		}


		void MoveRight()
		{
			//print ("Right arrow pressed!");
			rigidbody.velocity = new Vector3(moveSpeed, rigidbody.velocity.y, 0);
			RotateHead(1);
		}


		void doJump()
		{
			//print ("Space pressed!");
			canJump = false;
			Vector3 jumpPower = new Vector3(0, jumpSpeed, 0);
			GetComponent<Rigidbody>().AddForce(jumpPower, ForceMode.Impulse);
		}


		/// <summary>
		/// take the object to its initial position
		/// </summary>
		public void resetPosition()
		{
			transform.position = startingPosition;
			GetComponent<Rigidbody>().sleepThreshold = 0.005f;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}


		/// <summary>
		/// Shoot the ball upon collision
		/// </summary>
		void OnCollisionEnter(Collision other)
		{

			if (other.gameObject.tag == "Field")
			{
				canJump = true;

				if (landCooldown)
					return;

				landCooldown = true;
				StartCoroutine(ResetLandCooldown());

				return;
			}
			if (other.gameObject.GetComponent<BallController>())
			{
				Vector3 bounceDir = other.gameObject.transform.position - gameObject.transform.position;
				Vector3 shootForce;
				bounceDir.Normalize();
				//shootForce = Vector3.Scale(bounceDir, new Vector3(0.15f, 0.15f, 0.15f));
				shootForce = Vector3.Scale(bounceDir, new Vector3(playerBouncePower, playerBouncePower, playerBouncePower));
				other.gameObject.GetComponent<Rigidbody>().AddForce(shootForce, ForceMode.Impulse);


			}


			//print (other.gameObject.name + " - Time: " + Time.time + " - Force: " + shootForce);
		}

		public IEnumerator ResetLandCooldown()
		{
			yield return new WaitForSeconds(0.25f);
			//canJump = true;
			landCooldown = false;
		}
	}
}