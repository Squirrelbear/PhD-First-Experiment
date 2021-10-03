using UnityEngine;
using System.Collections;

public class MouseBehaviour : MonoBehaviour {

    GameObject currentCube;
    public enum CursorMode { LookAt = 0, Click = 1, MobileTap = 2 };
    public CursorMode cursorMode;
    public bool lastKeyState;
    public bool usingOculus;
    private LevelManager levelManager;
    private ReplayDatabase eventDatabase;
    public Camera mCam;
    public ServerBehaviour server;

	// Use this for initialization
	void Start () {
	    currentCube = null;
        lastKeyState = false;
        if (usingOculus)
        {
            Cursor.visible = false;
        }
        levelManager = (LevelManager)GameObject.Find("LevelManager").GetComponent("LevelManager");
        PersistantStateBehaviour pSB = (PersistantStateBehaviour)GameObject.Find("PersistantStateManager").GetComponent("PersistantStateBehaviour");
        eventDatabase = pSB.configDatabaseNew();

        // generate initial data
        ReplayEvent firstFrame = new ReplayEvent();
        firstFrame.rotation = Camera.main.transform.rotation;
        firstFrame.deltaTime = 0;
        firstFrame.keyActionEvent = ReplayEvent.KeyActionEvent.None;

        int nextInputMode = pSB.getNextInputMode();
        if (nextInputMode == 0)
        {
            pSB.getExtraDataRef().Add("Setting Input Mode to Look At");
            cursorMode = CursorMode.LookAt;
            firstFrame.keyActionEvent = ReplayEvent.KeyActionEvent.SetInputMode_Look;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(0);
        }
        else if (nextInputMode == 1)
        {
            pSB.getExtraDataRef().Add("Setting Input Mode to Click");
            cursorMode = CursorMode.Click;
            firstFrame.keyActionEvent = ReplayEvent.KeyActionEvent.SetInputMode_Cursor;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(1);
        }
        else if (nextInputMode == 2)
        {
            pSB.getExtraDataRef().Add("Setting Input Mode to Mobile");
            cursorMode = CursorMode.MobileTap;
            firstFrame.keyActionEvent = ReplayEvent.KeyActionEvent.SetInputMode_Mobile;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(2);
        }
        else if (nextInputMode == 3)
        {
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(3);
        }

        firstFrame.triggerEdge = false;
        eventDatabase.addEvent(firstFrame);
        pSB.enableSaveExtra();
        //((ExtraDataRecorder)levelManager.gameObject.GetComponent("ExtraDataRecorder")).setSaveName(eventDatabase.getCreationData()+"_extra.dat");

        server = (ServerBehaviour)GameObject.Find("PersistantStateManager").GetComponent("ServerBehaviour");
        server.setSaveFName(eventDatabase.getCreationData());
    }
	
	// Update is called once per frame
	void Update () {
        mCam = Camera.main;
        // replay event info
        ReplayEvent curFrame = new ReplayEvent();
        curFrame.deltaTime = Time.deltaTime;
        curFrame.rotation = Camera.main.transform.rotation;
        curFrame.position = Camera.main.transform.position;
        Ray ray;
        if (usingOculus)
        {
            ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        }
        else
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        bool clickUsed = false;
        bool clickOccured = false;

        if (cursorMode == CursorMode.Click)
        {
            clickOccured = Input.GetMouseButtonDown(0) == false && lastKeyState == true;
            // replay event info
            curFrame.triggerEdge = clickOccured;
            lastKeyState = Input.GetMouseButtonDown(0);
        }
        else if (cursorMode == CursorMode.MobileTap)
        {
            if (server.tapped)
            {
                server.tapped = false;
                clickOccured = true;
                curFrame.triggerEdge = true;
            }
        }

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

        // TODO: Record level skips in events (or perhaps trigger recording when level goes from 0 to 1
        if (levelManager.getCurrentLevel().isWon())
        {
            levelManager.changeLevel(LevelManager.LevelChange.Next);
            curFrame.keyActionEvent = ReplayEvent.KeyActionEvent.NextLevel;
            //print("Next level by won level.");
        }
        else if (Input.GetKeyUp("f1"))
        {
            int response = levelManager.changeLevel(LevelManager.LevelChange.Next);
            if (response == -1)
            {
                curFrame.keyActionEvent = ReplayEvent.KeyActionEvent.RestartGame;
                eventDatabase.addEvent(curFrame);
                Application.LoadLevel(0);
                return;
            }
            else
            {
                curFrame.keyActionEvent = ReplayEvent.KeyActionEvent.NextLevel;
            }
            //print("Next level by keypress.");
        }
        else if (Input.GetKeyUp("f3"))
        {
            levelManager.changeLevel(LevelManager.LevelChange.Previous);
            curFrame.keyActionEvent = ReplayEvent.KeyActionEvent.PreviousLevel;
        }
        else if (Input.GetKeyUp("f5"))
        {
            levelManager.changeLevel(LevelManager.LevelChange.First);
            curFrame.keyActionEvent = ReplayEvent.KeyActionEvent.FirstLevel;
        }
        else if (Input.GetKeyUp("f12"))
        {
            curFrame.keyActionEvent = ReplayEvent.KeyActionEvent.RestartGame;
            eventDatabase.addEvent(curFrame);
            Application.LoadLevel(0);
            return;
        }
        else if (Input.GetKeyUp("f8"))
        {
            if (cursorMode == CursorMode.MobileTap)
            {
                server.StopReceiving();
            }
            
            PersistantStateBehaviour pSB = (PersistantStateBehaviour)GameObject.Find("PersistantStateManager").GetComponent("PersistantStateBehaviour");
            pSB.getExtraDataRef().Add("Setting Input Mode to Look At");
            cursorMode = CursorMode.LookAt;
            curFrame.keyActionEvent = ReplayEvent.KeyActionEvent.SetInputMode_Look;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(0);
        }
        else if (Input.GetKeyUp("f9"))
        {
            if (cursorMode == CursorMode.MobileTap)
            {
                server.StopReceiving();
            }

            PersistantStateBehaviour pSB = (PersistantStateBehaviour)GameObject.Find("PersistantStateManager").GetComponent("PersistantStateBehaviour");
            pSB.getExtraDataRef().Add("Setting Input Mode to Click");
            cursorMode = CursorMode.Click;
            curFrame.keyActionEvent = ReplayEvent.KeyActionEvent.SetInputMode_Cursor;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(1);
        }
        else if (Input.GetKeyUp("f10"))
        {
            PersistantStateBehaviour pSB = (PersistantStateBehaviour)GameObject.Find("PersistantStateManager").GetComponent("PersistantStateBehaviour");
            pSB.getExtraDataRef().Add("Setting Input Mode to Mobile");
            cursorMode = CursorMode.MobileTap;
            curFrame.keyActionEvent = ReplayEvent.KeyActionEvent.SetInputMode_Mobile;
            ((StartScreenBehaviour)GameObject.Find("StartScreen").GetComponent("StartScreenBehaviour")).setTexture(2);
            server.StartReceiving();
        }

        levelManager.performUpdate(Time.deltaTime);

        eventDatabase.addEvent(curFrame);
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

    void OnApplicationQuit()
    {
        eventDatabase.saveDatabase(eventDatabase.getCreationData() + ".dat");
    }

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
