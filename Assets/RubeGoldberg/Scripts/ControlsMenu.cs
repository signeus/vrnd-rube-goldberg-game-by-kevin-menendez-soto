using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
	public GameObject info_A;
	public GameObject info_B;
	private ControllerLayout controllerLayout;

	void Start()
	{
		controllerLayout = GameObject.Find("ControllerLayout").GetComponent<ControllerLayout>();

		ChangeControllersInfo();
	}

	public void ChangeControllersInfo()
	{
		if(controllerLayout.layout == ControllerLayout.layoutEnum.normal)
		{
			info_A.SetActive(true);
			info_B.SetActive(false);
		}
		else
		{
			info_A.SetActive(false);
			info_B.SetActive(true);
		}
	}
}
