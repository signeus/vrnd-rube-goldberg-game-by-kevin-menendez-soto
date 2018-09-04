using UnityEngine;

// THis is for All buttons, has button type and functions for each button type

public class VRButton : MonoBehaviour
{
	private Renderer rend;
	public Color normalColor;
	public Color highlightColor;
	public Color clickColor;
	public enum buttonType
	{
		start,
		restartLevel,
		exit,
		switchControllers,
		resetBall
	};
	public buttonType type;

	private GameLogic GL;
                                                                                                    
	void Awake()
	{
		rend = GetComponent<Renderer>();
	}

	void Start()
	{
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
	}

	public void Init()
	{
		rend.material.color = normalColor;
	}

	public void Hover()
	{
		rend.material.color = highlightColor;
	}

	public void Click()
	{
		rend.material.color = clickColor;

		if(type == buttonType.start)
			GL.StartButton();
		else if(type == buttonType.exit)
			GL.ExitButton();
		else if(type == buttonType.switchControllers)
		{
			GL.SwitchControllersButton();
			Invoke("Init", 0.125f);
		}
		else if(type == buttonType.restartLevel)
			GL.RestartLevelButton();
		else if(type == buttonType.resetBall)
		{
			GL.ResetBallButton();
			Invoke("Init", 0.125f);
		}
	}
}
