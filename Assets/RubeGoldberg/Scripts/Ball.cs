using UnityEngine;

// This is for ball to detect collisions with ball, such as goal, fan, ground, etc

public class Ball : MonoBehaviour
{
	private GameLogic GL;
	private Vector3 startPosition;
	private Rigidbody rigidbody;
	private bool ballStopped; // this is to determine if the ball got stuck
	private float ballStoppedDuration;

	void Start()
	{
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		rigidbody = GetComponent<Rigidbody>();
		startPosition = transform.position;
		ballStopped = false;
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.transform.tag == "Ground" ||
		   collider.transform.tag == "BallReset" ||
		   collider.transform.tag == "Stage")
		{
			GL.BallTouchedGround();
			GL.DisplayMessage("Ball fell down, it should fall on the goal");
		}
		else if(collider.transform.tag == "Finish")
			GL.BallTouchedFinish();
		else if(collider.transform.tag == "Star")
			GL.BallTouchedStar(collider.gameObject);
	}

	public void Reset()
	{
		transform.position = startPosition;
		transform.rotation = Quaternion.Euler(Vector3.zero);
		rigidbody.isKinematic = true;
		ballStopped = false;
	}

	void Update()
	{
		// If ball speed is less, then start timer, if timer runs out, reset ball
		if(rigidbody.velocity.magnitude <= GL.ballResetSpeed && rigidbody.isKinematic == false)/// && transform.parent.gameObject == null)
		{
			if(!ballStopped)
			{
				ballStopped = true;
				ballStoppedDuration = 0;
			}
			ballStoppedDuration += Time.deltaTime;
			if(ballStoppedDuration >= GL.ballResetTime)
			{
				GL.BallTouchedGround();
				GL.DisplayMessage("Ball got stuck! Reseting ball");
			}
		}
	}
}
