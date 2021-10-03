using UnityEngine;
using System.Collections;

public class StartScreenBehaviour : MonoBehaviour {

    public Texture2D[] textures;

	// Use this for initialization
	void Start () {
        setTexture(0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setTexture(int textureID)
    {
        GetComponent<Renderer>().material.mainTexture = textures[textureID];
    }
}
