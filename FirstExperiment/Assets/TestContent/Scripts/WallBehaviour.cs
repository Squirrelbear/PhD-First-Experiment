using UnityEngine;
using System.Collections;

public class WallBehaviour : MonoBehaviour {

    public Texture2D defaultTexture; // orange texture
    public Texture2D hitTexture; // green texture
    public int hitCount;
    public bool triggerRemove;
    public int removeOnHitCount;

	// Use this for initialization
	void Start () {
        hitCount = 0;
        GetComponent<Renderer>().material.mainTexture = defaultTexture;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.name.StartsWith("Cube"))
        {
            CubeBehaviour script = (CubeBehaviour)other.collider.transform.gameObject.GetComponent("CubeBehaviour");
            if (script == null || !script.getObjEnabled())
                return;
            hitCount++;
            if (triggerRemove && hitCount >= removeOnHitCount)
            {
                gameObject.SetActive(false);
            }
            GetComponent<Renderer>().material.mainTexture = hitTexture;
            //print("Collided Wall Script");
        }
    }

}
