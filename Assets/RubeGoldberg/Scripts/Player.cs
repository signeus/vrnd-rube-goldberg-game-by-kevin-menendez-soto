using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	#region Global_Variables
	private GameLogic GL;

	//  Teleport
	private bool isMoving;
	public bool isOnPlatform;
	private Vector3 teleportLocation;
	private Vector3 startPosition;
	private float lerpDistance;
	private float totalDistance;

	//  Scene Transition
	public GameObject camera;

	//  RigidBody
	private Rigidbody rigidbody;

	//  Game Over Menu
	public GameObject gameOverMenu_GO;
	public Text 	  levelFinished_text;
	public GameObject nextLevelText_GO;
	public GameObject gameOverText_GO; // display gameover infront of camera
	#endregion
	                                                                                                   
	void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}
    

	public void Init()
	{
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		///camera = GameObject.Find("Player/OVRCameraRig/TrackingSpace/CenterEyeAnchor");

		isMoving = false;
		///isOnPlatform = false;

		levelFinished_text.text = "Level " + GL.currentLevel + " - Finished!";
	}

	public void InitTeleport(Vector3 tLoc)
	{
		teleportLocation = tLoc;
		startPosition = rigidbody.position;
		isMoving = true;
		rigidbody.isKinematic = true; // make player not colliding with anything when teleporting
		lerpDistance = 0;
		totalDistance = Vector3.Distance(startPosition, teleportLocation); // setting up distance to use lerp
	}

	//   U P D A T E                                                                                                    
	public void Teleport()
	{
		if(isMoving)
		{
			lerpDistance = lerpDistance + ((Time.deltaTime * GL.teleportSpeed) / totalDistance);
			rigidbody.position = Vector3.Lerp(startPosition, teleportLocation, lerpDistance); // calculate lerp by framerate for smooth movement
			if(lerpDistance >= 1)
			{
				transform.position = teleportLocation;
				isMoving = false;
				rigidbody.isKinematic = false; // make player collider with objects again after teleporting
			}
		}
	}

	public void Move(Vector2 joystickInput)
	{
		if(isMoving) // stop teleporting when joystick input is given
		{
			isMoving = false;
			rigidbody.isKinematic = false;
		}
		Vector3 joystickDirection = new Vector3(joystickInput.x, 0, joystickInput.y); // convert joystick input to XZ direction with respect to camera
		Vector3 direction = camera.transform.TransformDirection(joystickDirection);
		direction = new Vector3(direction.x, 0, direction.z);
		direction = direction * GL.moveSpeed * Time.deltaTime; // move player with certain speed set in gamelogic and framte rate
		transform.position += direction;
	}

    public void MoveHead(Vector2 joystickInput)
    {
        if (isMoving) // stop teleporting when joystick input is given
        {
            isMoving = false;
            rigidbody.isKinematic = false;
        }

        Vector3 joystickDirection = new Vector3(joystickInput.x, 0, joystickInput.y);
        transform.rotation = Quaternion.LookRotation(joystickDirection * Time.deltaTime * 0.03f);
    }

    void OnTriggerStay(Collider collider)
	{
		///Debug.Log("Player standing on object with tag: " + collider.transform.tag + "; " + isOnPlatform);
		if(collider.transform.tag == "Stage" && !isOnPlatform && !isMoving) // to check if player is standing on stage
		{
			GL.DisplayMessage("Throw the ball from here");
			isOnPlatform = true;
		}
		else if(collider.transform.tag != "Stage" && isOnPlatform)
		{
			GL.DisplayMessage("You are <b>not</b> on stage");
			isOnPlatform = false;
		}
	}

	void OnTriggerExit()
	{
		if(isOnPlatform)
		{
			GL.DisplayMessage("You are <b>not</b> on stage");
			isOnPlatform = false;
		}
	}

	void Update()
	{
		if(rigidbody.velocity.magnitude >= GL.maxPlayerVelocity) // this is to limit player speed, to reduce VR sickness
		{
			rigidbody.velocity = rigidbody.velocity.normalized * GL.maxPlayerVelocity;
		}
	}
    
}
