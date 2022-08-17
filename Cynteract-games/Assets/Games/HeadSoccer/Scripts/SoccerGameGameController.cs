using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Cynteract.SoccerGame
{
	public class SoccerGameGameController : GameController
	{

		public struct Match : IComparable<Match>
		{
			int playerGoals, cpuGoals;

			public Match(int playerGoals, int cpuGoals)
			{
				this.playerGoals = playerGoals;
				this.cpuGoals = cpuGoals;
			}
			public int GetValue()
			{
				if (cpuGoals>playerGoals)
				{
					return playerGoals;
				}
				return  (playerGoals - cpuGoals) +playerGoals;
			}
			int IComparable<Match>.CompareTo(Match other)
			{

				if (other.GetValue() > GetValue())
					return -1;
				else if (other.GetValue() == GetValue())
					return 0;
				else
					return 1;
			}
			public static Match Max( IEnumerable<Match> matches)
			{
				return matches.Max();
			}
		}
		/// <summary>
		/// Main game controller class.
		/// This class controls the game time (passed and remaining) and also manages game status
		/// like win, lose or draw.
		/// This class resets the game elements after each goal events.
		/// </summary>

		public static int gameTime = 90;                //gameplay time for each round
		private int remainingTime;                      //time left to finish the game
		public static int playerGoals;                  //total goals by player
		public static int cpuGoals;                     //total goals by cpu
		public static float startDelay = 3.0f;          //cooldown timer before starting the game

		public static bool gameIsFinished = false;      //global flag to finish the game
		private bool gameFinishFlag = false;            //private flag

		public AudioClip startWistle;                   //Audioclip
		public AudioClip endWistle;                     //Audioclip
		public AudioClip[] goalReceived;                //Audioclip
		public AudioClip[] goalLanded;                  //Audioclip

		//Reference to important game objects used inside the game
		public GameObject counter;
		public TextMeshProUGUI  hudPlayerGoals;
		public TextMeshProUGUI hudCpuGoals;
		public TextMeshProUGUI hudGameTime;
		public GameObject ball;
		public GameObject goalPlane;
		public GameObject GameFinishPlane;
		public GameObject gameFinishStatusImage;

		public Texture2D[] statusImages;                //available images for end game status

		private bool isFirstRun = true;                 //flag to initialize the game
		public static bool isGoalTransition = false;    //flag to manage after goal events
		private bool shouldUpdate=true;
		private float startTime;

		List<Match> matches = new List<Match>();
		public  void StartMatch()
		{
			if (GameFinishPlane)
				GameFinishPlane.SetActive(false);
			remainingTime = gameTime;
			startTime = Time.fixedTime;
			isFirstRun = true;
			playerGoals = 0;
			hudPlayerGoals.text = cpuGoals.ToString();
			cpuGoals = 0;
			hudCpuGoals.text = cpuGoals.ToString();
			StartCoroutine(InitRoutine());


		}


		//setup the game for the first run
		IEnumerator InitRoutine()
		{
			//if this is the first run, init and stop the physics for a bit
			if (isFirstRun)
			{

				//freeze the ball
				ball.GetComponent<Rigidbody>().useGravity = false;
				ball.GetComponent<Rigidbody>().isKinematic = true;

				//show the countdown timer
				GameObject Counter = Instantiate(counter, new Vector3(0, 1.8f, -1), Quaternion.Euler(0, 180, 0)) as GameObject;
				Counter.name = "Starting-Time-Counter";
				yield return new WaitForSeconds(startDelay);

				//start the game
				isFirstRun = false;
				PlaySfx(startWistle);
				yield return new WaitForSeconds(startWistle.length);

				//unfreeze the ball
				ball.GetComponent<Rigidbody>().useGravity = true;
				ball.GetComponent<Rigidbody>().isKinematic = false;

			}
			gameIsFinished = false;
			gameFinishFlag = false;
		}


		void Start()
		{
			remainingTime = gameTime;
		}


		void Update()
		{

			if (shouldUpdate)
			{
				//show game timer
				ManageGameTime();

				if (!gameIsFinished)
					ManageGoals();

				//if time is up, setup game finish events
				if (gameIsFinished && !gameFinishFlag)
				{
					gameFinishFlag = true;

					//play end wistle
					PlaySfx(endWistle);

					//declare the winner
					if (playerGoals > cpuGoals)
					{
						print("Player Wins");
						gameFinishStatusImage.GetComponent<Renderer>().material.mainTexture = statusImages[0];
					}
					else if (playerGoals < cpuGoals)
					{
						print("CPU Wins");
						gameFinishStatusImage.GetComponent<Renderer>().material.mainTexture = statusImages[1];
					}
					else if (playerGoals == cpuGoals)
					{
						print("Draw!");
						gameFinishStatusImage.GetComponent<Renderer>().material.mainTexture = statusImages[2];
					}

					//show gamefinish plane
					GameFinishPlane.SetActive(true);
					matches.Add(new Match(playerGoals, cpuGoals));
					StartCoroutine(RestartAfterSeconds(3));
				}

			}

		}

		private IEnumerator RestartAfterSeconds(int v)
		{
			yield return new WaitForSeconds(3);
			StartMatch();
		}




		/// <summary>
		/// Update game data after a goal happens.
		/// Arg Parameters:
		/// 0 = received a goal
		/// 1 = scored a goal
		/// </summary>
		public IEnumerator ManageGoalEvent(GameSide side)
		{

			//fake pause the game for a while
			isGoalTransition = true;

			//blow the wistle && add the score
			if (side == GameSide.Player)
			{
				//player received a goal
				PlaySfx(goalReceived[UnityEngine.Random.Range(0, goalReceived.Length)]);
				cpuGoals++;
			}
			else
			{
				//cpu received a goal
				PlaySfx(goalLanded[UnityEngine.Random.Range(0, goalLanded.Length)]);
				playerGoals++;
			}

			//activate the goal event plane
			GameObject gp = null;
			if (side == GameSide.CPU)
			{
				gp = Instantiate(goalPlane, new Vector3(15, 2, -1), Quaternion.Euler(0, 180, 0)) as GameObject;
				float t = 0;
				float speed = 2.0f;
				while (t < 1)
				{
					t += Time.deltaTime * speed;
					gp.transform.position = new Vector3(Mathf.SmoothStep(15, 0, t), 2, -1);
					yield return 0;
				}

				yield return new WaitForSeconds(0.75f);

				float t2 = 0;
				while (t2 < 1)
				{
					t2 += Time.deltaTime * speed;
					gp.transform.position = new Vector3(Mathf.SmoothStep(0, -15, t2), 2, -1);
					yield return 0;
				}
			}

			yield return new WaitForSeconds(1.5f);
			Destroy(gp);
			isGoalTransition = false;

			//do not continue the game if the time is up
			if (gameIsFinished)
				yield break;

			//continue the game
			PlaySfx(startWistle);
			yield return new WaitForSeconds(startWistle.length);
			ball.GetComponent<Rigidbody>().isKinematic = false;
			ball.GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0), ForceMode.Impulse);

		}


		//show goals on the screen
		void ManageGoals()
		{
			hudCpuGoals.text = cpuGoals.ToString();
			hudPlayerGoals.text = playerGoals.ToString();
		}


		//show game timer on the screen
		void ManageGameTime()
		{
			remainingTime = (int)(gameTime - (Time.fixedTime- startTime));

			//finish the game if time is up
			if (remainingTime <= 0)
			{
				gameIsFinished = true;
				remainingTime = 0;
			}

			hudGameTime.text = remainingTime.ToString();
		}


		//*****************************************************************************
		// Play sound clips
		//*****************************************************************************
		void PlaySfx(AudioClip _clip)
		{
			GetComponent<AudioSource>().clip = _clip;
			if (!GetComponent<AudioSource>().isPlaying)
			{
				GetComponent<AudioSource>().Play();
			}
		}

		public override int GetScore()
		{
			return Match.Max(matches).GetValue()*100;
		}
	}
}