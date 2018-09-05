using UnityEngine;

public class Portal : MonoBehaviour {

    #region Global_Variables
    private GameObject destinationTarget;
    private Transform destination;
    public float distance = 0.25f;
    public bool isOrange;
    #endregion
    
    void Update () {
		if(destinationTarget == null)
        {
            if(isOrange == false)
            {
                destinationTarget = GameObject.FindGameObjectWithTag("OrangePortal");
                if(destinationTarget != null)
                    destination = destinationTarget.GetComponent<Transform>();
            }
            else
            {
                destinationTarget = GameObject.FindGameObjectWithTag("BluePortal");
                if (destinationTarget != null)
                    destination = destinationTarget.GetComponent<Transform>();
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Throwable")
        {
            return;
        }
        if(Vector3.Distance(transform.position, other.transform.position) > distance)
        {
            if(destination != null)
            {
                other.transform.position = new Vector3(destination.position.x, destination.position.y, destination.position.z);
            }
        }
    }
}
