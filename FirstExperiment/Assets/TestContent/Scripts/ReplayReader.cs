using UnityEngine;
using System.Collections;

public class ReplayReader : MonoBehaviour {

    GameObject currentCube;
    public enum CursorMode { LookAt, Click, MobileTap };
    public CursorMode cursorMode;
    public bool lastKeyState;
    private LevelManager levelManager;
    public ReplayDatabase eventDatabase;
    public string filename;
    public int curFrameID;

	// Use this for initialization
	void Start () {
	    currentCube = null;
        lastKeyState = false;
        Cursor.visible = false;
        
        levelManager = (LevelManager)GameObject.Find("LevelManager").GetComponent("LevelManager");
        PersistantStateBehaviour pSB = (PersistantStateBehaviour)GameObject.Find("PersistantStateManager").GetComponent("PersistantStateBehaviour");
        eventDatabase = pSB.configDatabaseNew();
        eventDatabase.loadDatabase(filename);
        
        // read the first frame
        ReplayEvent curFrame = eventDatabase.getNextEvent();
        if (curFrame == null)
        {
            print("Error: No first frame for replay.");
            return;
        }

        Camera.main.transform.rotation = curFrame.rotation;
        curFrameID = 0;
        if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Look)
        {
            cursorMode = CursorMode.LookAt;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(0);
            //print("Input mode set to Look At");
        }
        else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Cursor)
        {
            cursorMode = CursorMode.Click;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(1);
            print("Input mode set to Click");
        }
        else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Mobile)
        {
            cursorMode = CursorMode.MobileTap;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(2);
            print("Input mode set to Mobile");
        }
    }
	
	// Update is called once per frame
	void Update () {
        // replay event info
        curFrameID = eventDatabase.getCurFrameID();
        ReplayEvent curFrame = eventDatabase.getNextEvent(); //new ReplayEvent();

        if (curFrame == null)
            return;

        Camera.main.transform.rotation = curFrame.rotation;
        Camera.main.transform.position = curFrame.position;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        

        bool clickUsed = false;
        bool clickOccured = curFrame.triggerEdge;

        RaycastHit[] hit;
        hit = Physics.RaycastAll(ray);
        if (hit.Length > 0)
        {
            bool hitCube = false;
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].transform.name.StartsWith("Cube_menu"))
                {
                    if (hit[i].transform.gameObject != currentCube)
                    {
                        //print("Changed selection to: " + hit[i].transform.name);
                        if (currentCube != null)
                        {
                            cancalSelection(currentCube);
                            currentCube = null;
                        }

                        currentCube = hit[i].transform.gameObject;
                        MenuBtnBehaviour scriptNext = (MenuBtnBehaviour)currentCube.transform.gameObject.GetComponent("MenuBtnBehaviour");
                        scriptNext.beginSelection((cursorMode == CursorMode.LookAt) ? MenuBtnBehaviour.SelectionMode.Time : MenuBtnBehaviour.SelectionMode.HoverSelect);
                    }
                    else if (cursorMode != CursorMode.LookAt && clickOccured && !clickUsed)
                    {
                        MenuBtnBehaviour scriptNext = (MenuBtnBehaviour)currentCube.transform.gameObject.GetComponent("MenuBtnBehaviour");
                        scriptNext.completeSelection();
                        clickUsed = true;
                    }
                    hitCube = true;
                    break;
                }
                else if (hit[i].transform.name.StartsWith("Cube"))
                {
                    if (hit[i].transform.gameObject != currentCube)
                    {
                        //print("Changed selection to: " + hit[i].transform.name);
                        if (currentCube != null)
                        {
                            cancalSelection(currentCube);
                            currentCube = null;
                        }

                        currentCube = hit[i].transform.gameObject;
                        CubeBehaviour scriptNext = (CubeBehaviour)currentCube.transform.gameObject.GetComponent("CubeBehaviour");
                        scriptNext.beginSelection((cursorMode == CursorMode.LookAt) ? CubeBehaviour.SelectionMode.Time : CubeBehaviour.SelectionMode.HoverSelect);
                    }
                    else if (cursorMode != CursorMode.LookAt && clickOccured && !clickUsed)
                    {
                        CubeBehaviour scriptNext = (CubeBehaviour)currentCube.transform.gameObject.GetComponent("CubeBehaviour");
                        scriptNext.completeSelection();
                        clickUsed = true;
                    }
                    hitCube = true;
                    break;
                }
            }

            if (currentCube != null && !hitCube)
            {
                cancalSelection(currentCube);
                currentCube = null;
            }
        } else if(currentCube != null) {
            cancalSelection(currentCube);
            currentCube = null;
        }

        /*if (levelManager.getCurrentLevel().isWon())
        {
            levelManager.changeLevel(LevelManager.LevelChange.Next);
        }*/
        
        if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.NextLevel)
        {
            levelManager.changeLevel(LevelManager.LevelChange.Next);
            //print("Next level action event triggered.");
        }
        else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.PreviousLevel)
        {
            levelManager.changeLevel(LevelManager.LevelChange.Previous);
            //print("Previous level action event triggered.");
        }
        else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.FirstLevel)
        {
            levelManager.changeLevel(LevelManager.LevelChange.First);
        }
        else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.RestartGame)
        {
            Application.LoadLevel(0);
        }
        else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Look)
        {
            cursorMode = CursorMode.LookAt;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(0);
            //print("Input mode set to Look At");
        }
        else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Cursor)
        {
            cursorMode = CursorMode.Click;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(1);
            print("Input mode set to Click");
        }
        else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Mobile)
        {
            cursorMode = CursorMode.MobileTap;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(2);
            print("Input mode set to Mobile");
        }

        levelManager.performUpdate(curFrame.deltaTime);
    }

    void cancalSelection(GameObject obj) {
        if(obj == null) return;

        if (currentCube.transform.name.StartsWith("Cube_menu"))
        {
            MenuBtnBehaviour script = (MenuBtnBehaviour)currentCube.transform.gameObject.GetComponent("MenuBtnBehaviour");
            script.cancelSelection();
        }
        else if (currentCube.transform.name.StartsWith("Cube"))
        {
            CubeBehaviour script = (CubeBehaviour)currentCube.transform.gameObject.GetComponent("CubeBehaviour");
            script.cancelSelection();
        }
    }

    /*void OnApplicationQuit()
    {
        eventDatabase.saveDatabase(eventDatabase.getCreationData() + ".dat");
    }*/

        /*RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject != currentCube)
            {
                print("Changed selection to: " + hit.transform.name);
                if(currentCube != null) {
                    CubeBehaviour script = (CubeBehaviour)hit.transform.gameObject.GetComponent("CubeBehaviour");
                    script.cancelSelection();
                    currentCube = null;
                }

                if (hit.transform.name.StartsWith("Cube"))
                {
                    currentCube = hit.transform.gameObject;
                    CubeBehaviour scriptNext = (CubeBehaviour)currentCube.transform.gameObject.GetComponent("CubeBehaviour");
                    scriptNext.beginSelection();
                }
            } 
            
        } else if(currentCube != null) {
            CubeBehaviour script = (CubeBehaviour)currentCube.transform.gameObject.GetComponent("CubeBehaviour");
            script.cancelSelection();
            currentCube = null;
        }*/

        /*RaycastHit[] hit;
        hit = Physics.RaycastAll(ray);
        if (hit.Length > 0)
        {
            for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].transform.name.StartsWith("Cube"))
                    {
                        if (currentCube != null && hit[i].transform.gameObject != currentCube)
                        {
                            CubeBehaviour script = (CubeBehaviour)hit[i].transform.gameObject.GetComponent("CubeBehaviour");
                            script.cancelSelection();
                        }
                    }
        }*/

        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.cyan);
        /*RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            print("I am looking at " + hit.transform.name);

        } else {
            print("I am looking at nothing!");
        }*/

        /*if (Input.GetMouseButtonDown(0))
        {
            RaycastHit[] hit;
            hit = Physics.RaycastAll(ray);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].transform.name.StartsWith("Cube"))
                    {
                        CubeBehaviour script = (CubeBehaviour)hit[i].transform.gameObject.GetComponent("CubeBehaviour");
                        script.beginSelection();
                        print("Selection begun.");
                    }
                    //float rayDistance = hit[i].distance;
                    //print("I am looking at " + hit[i].transform.name + ", " + rayDistance + " units away");
                }
            }
        }*/
	
}
