using UnityEngine;
using System.Collections;

public class MenuBehaviour : MonoBehaviour {

    public GameObject[] buttons;
    public GameObject[] effects;
    public Texture2D[] textures;
    public string[] btnTexNames;
    public string[] menuDef;
    public int curMenuID;
    public bool enabledObj;
    public bool showMenu;
    public GameObject parentObj;
    public CubeBehaviour parentScript;
    public int colourMatch;
    public int sizeMatch;

	// Use this for initialization
	void Start () {
        curMenuID = -1;
        enabledObj = false;
        showMenu = false;
        parentObj = transform.parent.gameObject;
        parentScript = (CubeBehaviour)parentObj.GetComponent("CubeBehaviour");
        colourMatch = 0;
        sizeMatch = 1;
        setMenu(0);

        foreach (GameObject effect in effects)
        {
            effect.GetComponent<Renderer>().enabled = false;
            //effect.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void performUpdate(float dTime)
    {
        if (parentScript.getSelectionState() == CubeBehaviour.SelectionState.Selected && parentScript.cubeType != CubeBehaviour.CubeType.Draggable)
        {
            ((LevelBehaviour)parentScript.transform.parent.GetComponent("LevelBehaviour")).closeAllMenus();

            parentScript.removeSelection();
            showMenu = true;
            setMenu(0);
            foreach (GameObject effect in effects)
            {
                effect.GetComponent<Renderer>().enabled = true;
                //effect.SetActive(true);
            }
            foreach (GameObject button in buttons)
            {
                //MenuBtnBehaviour script = (MenuBtnBehaviour)button.GetComponent("MenuBtnBehaviour");
                button.GetComponent<Renderer>().enabled = true;
                //script.gameObject.SetActive(true);
                //print("Button set active: " + button.name);
            }
        }
        else if (parentScript.cubeType == CubeBehaviour.CubeType.Draggable && parentScript.getSelectionState() == CubeBehaviour.SelectionState.Waiting)
        {
            parentScript.cubeType = CubeBehaviour.CubeType.Static;
        }

        //print(transform.parent.name);
        bool isParentEnabled = parentScript.getObjEnabled();
        if (isParentEnabled != enabledObj)
        {
            enabledObj = isParentEnabled;
            foreach (GameObject button in buttons)
            {
                MenuBtnBehaviour script = (MenuBtnBehaviour)button.GetComponent("MenuBtnBehaviour");
                script.setEnabledState(isParentEnabled);
            }
        }

        if (showMenu)
        {
            foreach (GameObject button in buttons)
            {
                MenuBtnBehaviour script = (MenuBtnBehaviour)button.GetComponent("MenuBtnBehaviour");
                if (script.getSelectionState() == MenuBtnBehaviour.SelectionState.Selected)
                {
                    //print("Button Selected");
                    applyAction(script.buttonAction);
                    setMenu(script.menuAction);
                    break;
                }
            }
        }

        foreach (GameObject button in buttons)
        {
            MenuBtnBehaviour script = (MenuBtnBehaviour)button.GetComponent("MenuBtnBehaviour");
            script.performUpdate(dTime);
        }
    }

    public void setMenu(int menuID)
    {
        //print("Jumpting to: " + menuID);
        if (menuID == -1 || menuID >= menuDef.Length)
        {
            //print("Menu Error");
            foreach (GameObject effect in effects)
            {
                effect.GetComponent<Renderer>().enabled = false;
                //effect.SetActive(false);
            }
            foreach (GameObject button in buttons)
            {
                //MenuBtnBehaviour script = (MenuBtnBehaviour)button.GetComponent("MenuBtnBehaviour");
                //script.gameObject.SetActive(false);
                button.GetComponent<Renderer>().enabled = false;
                //print("Button turned off: " + button.name);
            }
            showMenu = false;
            return;
            // TODO close the menu
        }

        string[] splitData = menuDef[menuID].Split(';');
        for(int i = 0; i < splitData.Length; i++) 
        {
            MenuBtnBehaviour script = (MenuBtnBehaviour)buttons[i].GetComponent("MenuBtnBehaviour");
            string[] btnData = splitData[i].Split(':');
            int nameID = getNameID(btnData[0]);
            script.setButtonContext(textures[nameID + 1], textures[nameID], textures[nameID + 2], int.Parse(btnData[1]), btnData[0]);
        }


    }

    public void applyAction(string buttonAction)
    {
        switch (buttonAction)
        {
            case "Small":
                Vector3 scaleSmall = new Vector3(0.126f,  2,  0.1875f) * 0.6f;
                scaleSmall.y = 2;
                parentObj.transform.localScale = scaleSmall;
                sizeMatch = 0;
                break;
            case "Medium":
                Vector3 scaleMed = new Vector3(0.126f, 2, 0.1875f);
                parentObj.transform.localScale = scaleMed;
                sizeMatch = 1;
                break;
            case "Large":
                Vector3 scaleLarge = new Vector3(0.126f, 2, 0.1875f) * 1.3f;
                scaleLarge.y = 2;
                parentObj.transform.localScale = scaleLarge;
                sizeMatch = 2;
                break;
            case "Red":
                //print("doing red");
                parentScript.setExtraColourTex(0);
                colourMatch = 0;
                break;
            case "Blue":
                parentScript.setExtraColourTex(1);
                colourMatch = 1;
                break;
            case "Green":
                parentScript.setExtraColourTex(2);
                colourMatch = 2;
                break;
            case "Move":
                parentScript.completeSelection();
                parentScript.cubeType = CubeBehaviour.CubeType.Draggable;
                break;
        }
    }

    public int getNameID(string name)
    {
        for (int i = 0; i < btnTexNames.Length; i++)
        {
            if (btnTexNames[i].Equals(name))
            {
                return i*3;
            }
        }
        return -1;
    }
}
