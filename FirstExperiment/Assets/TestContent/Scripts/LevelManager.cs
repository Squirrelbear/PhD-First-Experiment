using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public enum LevelChange { First, Next, Previous };
    public GameObject[] levels;
    public Texture2D[] levelInfoTextures;
    public int curLevel;
    public int hideCoord;
    public bool loaded;
    public GameObject infoPanelObj;

	// Use this for initialization
	void Start () {
        loaded = false;
        hideCoord = 1000;
        infoPanelObj.GetComponent<Renderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!loaded && levels.Length > 0)
        {
            curLevel = 0;
            Vector3 newPos = new Vector3(0, 0, 0);
            levels[0].transform.position = newPos;
            LevelBehaviour scriptEnable = (LevelBehaviour)levels[curLevel].transform.gameObject.GetComponent("LevelBehaviour");
            scriptEnable.enableLevel();
            //print("Moving: " + levels[0].transform.name);
            loaded = true;
        }

	}

    public void performUpdate(float dTime)
    {
        getCurrentLevel().performUpdate(dTime);
    }

    public LevelBehaviour getCurrentLevel()
    {
        return (LevelBehaviour)levels[curLevel].transform.gameObject.GetComponent("LevelBehaviour");    
    }

    public int changeLevel(LevelChange levelChange)
    {
        if (levelChange == LevelChange.First)
        {
            changeLevel(0);
        }
        else if (levelChange == LevelChange.Next)
        {
            if (curLevel + 1 >= levels.Length)
            {
                return -1;
            }
            changeLevel(curLevel + 1); 
        }
        else if (levelChange == LevelChange.Previous)
        {
            changeLevel(curLevel - 1); 
        }

        return 0;
    }

    public void changeLevel(int newLevelID)
    {
        if (newLevelID >= levels.Length || newLevelID < 0)
        {
            return;
        }
        Vector3 newPos = new Vector3(hideCoord, hideCoord, hideCoord);
        levels[curLevel].transform.position = newPos;
        LevelBehaviour script = (LevelBehaviour)levels[curLevel].transform.gameObject.GetComponent("LevelBehaviour");
        script.disableLevel();
        curLevel = newLevelID;
        Vector3 newPos2 = new Vector3(0, 0, 0);
        levels[curLevel].transform.position = newPos2;
        LevelBehaviour script2 = (LevelBehaviour)levels[curLevel].transform.gameObject.GetComponent("LevelBehaviour");
        script2.enableLevel();

        if (levelInfoTextures[curLevel] == null)
        {
            infoPanelObj.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            infoPanelObj.GetComponent<Renderer>().enabled = true;
            infoPanelObj.GetComponent<Renderer>().material.mainTexture = levelInfoTextures[curLevel];
        }
    }
}
