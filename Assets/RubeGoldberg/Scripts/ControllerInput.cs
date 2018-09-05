using UnityEngine;
using UnityEngine.UI;


// This has logic about teleporting using raycasting and object spawner menu

public class ControllerInput : MonoBehaviour
{
	#region Global_Variables
	private GameLogic GL;
	private Player player;
	public AudioSource speaker;
	public AudioClip buttonClickAudio;

	//  TELEPORT
	private GameObject teleportLocation_GO;
	public LineRenderer ray;

	//  OBJECT SPAWNER MENU
	public bool isMenuOpen;
	private GameObject objSpawnMenu_GO;
	private Image obj_img;
	private Text objName_text;
	private Text objCount_text;
	private int displayCount;
	private bool isChangedAlready;
	public Vector3 objSpawnMenu_position; // location of object spawner transform with respect to hand controller
	public Vector3 objSpawnMenu_rotation;
	public Vector3 objSpawn_position; // spawned object location with respect player camera
	#endregion
                                                                                              
	void Awake()
	{
		player = GetComponent<Player>();
	}

	// Init
	public void Init()
	{
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		teleportLocation_GO = GameObject.Find("TeleportLocation");
		TeleportLocation_SetActive(false);

		ray.gameObject.SetActive(false);
		GL.L_controller_GO.GetComponent<ControllerCollision>().Init();
		GL.R_controller_GO.GetComponent<ControllerCollision>().Init();
		/*L_holdingObject = null;
		R_holdingObject = null;*/

		isMenuOpen = false;
		objSpawnMenu_GO = GameObject.Find("ObjSpawnMenu_UI");
		obj_img 	    = GameObject.Find("ObjSpawnMenu_UI/ObjSpawnMenu_Canvas/Obj_Img").GetComponent<Image>();
		objName_text    = GameObject.Find("ObjSpawnMenu_UI/ObjSpawnMenu_Canvas/ObjName_Text").GetComponent<Text>();
		objCount_text   = GameObject.Find("ObjSpawnMenu_UI/ObjSpawnMenu_Canvas/Count_Text").GetComponent<Text>();
		displayCount = 0;
		ObjSpawnMenu_SetActive(false);
	}
	
	// INPUT
	public void CheckInput()
	{
		// B Button Press - Teleporting button, hold to see the ray and release to teleport
		if(OVRInput.Get(OVRInput.Button.Two, GL.R_controller))
		{
			ray.gameObject.SetActive(true);
			ray.SetPosition(0, GL.R_controller_GO.transform.position);

			RaycastHit hit;
			// Hit a ray
			if(Physics.Raycast(GL.R_controller_GO.transform.position,
			                   GL.R_controller_GO.transform.forward, out hit, GL.rayRange, GL.rayMask))
			{
				ray.SetPosition(1, hit.point);

				if(hit.transform.tag != "Button") // if ray doesnt hit button, make the button to it's normal color
					GL.ResetAllButtons();

				if(hit.transform.tag == "Button")  // if ray hits button, highlight that button
				{
					TeleportLocation_SetActive(false);
					hit.collider.gameObject.GetComponent<VRButton>().Hover();
				}
				else
				if(hit.transform.tag == "Ground") // ray hits ground
				{
					if(teleportLocation_GO != null)
						teleportLocation_GO.transform.position = hit.point;
					TeleportLocation_SetActive(true);
				}
				else // if ray hits something, check the ground
                    if(hit.transform.tag != "CannotTeleport")
					    GroundRay(hit.point, true);
			}
			else // if ray doesnt hit anything, check ground at the end or ray
			{
				GL.ResetAllButtons();
				Vector3 rayEndPoint = (GL.R_controller_GO.transform.forward * GL.rayRange) +
				                      GL.R_controller_GO.transform.position;
				ray.SetPosition(1, rayEndPoint);

				GroundRay(rayEndPoint, true);
			}
		}

		// B Button Up
		if(OVRInput.GetUp(OVRInput.Button.Two, GL.R_controller)) // when teleporting button is released
		{
			////Debug.Log("B button released");
			ray.gameObject.SetActive(false);
			TeleportLocation_SetActive(false);

			RaycastHit hit;
			if(Physics.Raycast(GL.R_controller_GO.transform.position,
			                   GL.R_controller_GO.transform.forward, out hit, GL.rayRange, GL.rayMask))
			{
				if(hit.transform.tag != "Button")
					GL.ResetAllButtons();

				if(hit.transform.tag == "Button") // call button function
				{
					hit.collider.gameObject.GetComponent<VRButton>().Click();
					speaker.clip = buttonClickAudio;
					speaker.Play();
					GL.R_haptics.Vibrate(VibrationForce.Hard);
				}
				else
				if(hit.transform.tag == "Ground" || hit.transform.tag == "Stage")
					GL.InitTeleportPlayer(hit.point);
				else
					GroundRay(hit.point, false);
			}
			else
			{
				GL.ResetAllButtons();
				Vector3 rayEndPoint = (GL.R_controller_GO.transform.forward * GL.rayRange) +
				                      GL.R_controller_GO.transform.position;
				ray.SetPosition(1, rayEndPoint);

				GroundRay(rayEndPoint, false);
			}
		}
        Vector2 joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, GL.R_controller);
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, GL.R_controller))
        {
            // Joystick input to move view of player
            if (joystickInput != Vector2.zero)
                GL.MoveViewPlayer(joystickInput);
        } else
        {
            // Joystick input to move player
            if (joystickInput != Vector2.zero)
                GL.MovePlayer(joystickInput);
        }

        

		// Object Spawner Menu
		if(OVRInput.GetDown(OVRInput.Button.Two, GL.L_controller))
		{
			GL.OpenObjectSpawMenu(!isMenuOpen); // Toggle for object menu spawner
		}

		if(isMenuOpen) // if object menu is open, take the joystick input for that
		{
			joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, GL.L_controller);
			/*if(joystickInput != Vector2.zero)
				Debug.Log("Joystick Input: " + joystickInput.x);*/
			if(Mathf.Abs(joystickInput.x) < 0.3f && isChangedAlready) // change only when joystick again goes back
				isChangedAlready = false;
			else if(Mathf.Abs(joystickInput.x) >= 0.8f && !isChangedAlready) // change oonly once when joystick is pushed left or right
			{
				isChangedAlready = true;
				SetSpawnObject((int)Mathf.Round(joystickInput.x));
			}
			if(OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, GL.L_controller)) // spawn shown object when joystick button is pressed
			{
				///Debug.Log("Joystick Button Pressed");
				///////////////////////////////////Spawn object
				if(GL.objSpawner[displayCount].left > 0) // check if objects are left to be spawned
				{
					Instantiate(GL.objSpawner[displayCount].GO,
					            GL.L_controller_GO.transform.TransformPoint(objSpawn_position),
					            Quaternion.Euler(new Vector3(0, GL.L_controller_GO.transform.rotation.eulerAngles.y, 0)));
					GL.objSpawner[displayCount].left--;
					objCount_text.text = GL.objSpawner[displayCount].left + " of " +
					GL.objSpawner[displayCount].count + "  left";
					GL.L_haptics.Vibrate(VibrationForce.Medium);
				}
				else
				{
					GL.L_haptics.Vibrate(VibrationForce.Medium);
					GL.L_haptics.Vibrate(VibrationForce.Medium);
				}	
			}
		}
	}

	public void ObjSpawnMenu_SetActive(bool state) // neable disable object spawner menu
	{
		isMenuOpen = state;
		if(state)
		{
			isChangedAlready = false;
			objSpawnMenu_GO.SetActive(true);
			SetSpawnObject(0);
			objSpawnMenu_GO.transform.SetParent(GL.L_controller_GO.transform);
			objSpawnMenu_GO.transform.rotation = GL.L_controller_GO.transform.rotation;
			objSpawnMenu_GO.transform.Rotate(objSpawnMenu_rotation, Space.Self);
			objSpawnMenu_GO.transform.position = GL.L_controller_GO.transform.TransformPoint(objSpawnMenu_position);
		}
		else
		{
			objSpawnMenu_GO.transform.SetParent(null);
			objSpawnMenu_GO.SetActive(false);
		}
	}

	private void SetSpawnObject(int inc) // change displayed object in menu - left/right
	{
		displayCount = displayCount + inc;
		if(displayCount < 0)
			displayCount = GL.objSpawner.Length - 1;
		else if(displayCount >= GL.objSpawner.Length)
			displayCount = 0;
		///Debug.Log("Object set to " + displayCount);
		obj_img.sprite = GL.objSpawner[displayCount].sprite;
		objName_text.text = GL.objSpawner[displayCount].name;
		objCount_text.text = GL.objSpawner[displayCount].left + " of " + GL.objSpawner[displayCount].count + "  left";
		GL.L_haptics.Vibrate(VibrationForce.Light);
	}

	// Ground Ray
	private void GroundRay(Vector3 startPoint, bool isButtonPress) // checking ground at the teleporting point by again raycasting down towards ground
	{
		RaycastHit groundHit;
		if(Physics.Raycast(startPoint, -Vector3.up, out groundHit, GL.rayRange, GL.rayMask))
		{
			if(isButtonPress && groundHit.transform.tag != "CannotTeleport")
			{
				if(teleportLocation_GO != null)
					teleportLocation_GO.transform.position = groundHit.point;
				TeleportLocation_SetActive(true);
			}
			else
				GL.InitTeleportPlayer(groundHit.point);
		}
		else
		{
			if(isButtonPress)
				TeleportLocation_SetActive(false);
			Debug.Log("Ground Ray didn't hit ground: Cannot Teleport!");
		}
	}

	private void TeleportLocation_SetActive(bool state)
	{
		if(teleportLocation_GO != null)
			teleportLocation_GO.SetActive(state);
	}
}
