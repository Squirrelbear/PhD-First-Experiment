using UnityEngine;
using System.Collections;

public class PathCornerBehaviour : MonoBehaviour {

    public GameObject nextCorner;

	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().enabled = false;
        //GetComponent("Mesh Renderer").renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
