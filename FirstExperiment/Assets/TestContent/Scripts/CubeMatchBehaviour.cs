using UnityEngine;
using System.Collections;

public class CubeMatchBehaviour : MonoBehaviour {

    public GameObject cubeObject;
    public CubeBehaviour cubeScript;
    public MenuBehaviour menuScript;
    public Material mat;
    bool increasing;

    public int matchColour;
    public int matchSize;
    public float matchMaxDistance;

	// Use this for initialization
	void Start () {
        cubeScript = (CubeBehaviour)cubeObject.GetComponent("CubeBehaviour");
        menuScript = (MenuBehaviour)cubeObject.transform.FindChild("MenuController").GetComponent("MenuBehaviour");
        mat = GetComponentInChildren<MeshRenderer>().material;
        Color newColor = mat.color;
        switch (matchColour)
        {
            case 0:
                newColor = Color.red;
                break;
            case 1:
                newColor = Color.blue;
                break;
            case 2:
                newColor = Color.green;
                break;
        }
        switch (matchSize)
        {
            case 0:
                Vector3 scaleSmall = new Vector3(0.126f,  2,  0.1875f) * 0.6f;
                scaleSmall.y = 2;
                transform.localScale = scaleSmall;
                break;
            case 1:
                Vector3 scaleMed = new Vector3(0.126f, 2, 0.1875f);
                transform.localScale = scaleMed;
                break;
            case 2:
                Vector3 scaleLarge = new Vector3(0.126f, 2, 0.1875f) * 1.3f;
                scaleLarge.y = 2;
                transform.localScale = scaleLarge;
                break;
        }
        newColor.a = 0.2f;
        mat.color = newColor;
	}
	
	// Update is called once per frame
	void Update () {
        if ((cubeScript.getSelectionState() == CubeBehaviour.SelectionState.Selected || menuScript.showMenu) && mat.color.a == 0)
        {
            Color newColor = mat.color;
            newColor.a = 0.2f;
            mat.color = newColor;
        }
        else if(!menuScript.showMenu)
        {
            Color newColor = mat.color;
            newColor.a = 0;
            mat.color = newColor;

            if (matchColour == menuScript.colourMatch && matchSize == menuScript.sizeMatch
            && Vector3.Distance(transform.position, cubeObject.transform.position) <= matchMaxDistance)
            {
                cubeScript.cubeObjComplete = true;
            }

            return;
        }
        Color newColor2 = mat.color;
        if (increasing)
        {
            newColor2.a += Time.deltaTime;
        }
        else
        {
            newColor2.a -= Time.deltaTime;
        }
        if (newColor2.a < 0.2) increasing = true;
        else if (newColor2.a > 0.8) increasing = false;
        mat.color = newColor2;

        if (matchColour == menuScript.colourMatch && matchSize == menuScript.sizeMatch
            && Vector3.Distance(transform.position, cubeObject.transform.position) <= matchMaxDistance)
        {
            cubeScript.cubeObjComplete = true;
            cubeScript.completeSelection();
        } else if(cubeScript.getSelectionState() == CubeBehaviour.SelectionState.Selected
            && Vector3.Distance(transform.position, cubeObject.transform.position) <= matchMaxDistance)
        {
            cubeScript.completeSelection();
        }
	}
}
