using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelBehaviour : MonoBehaviour {

    public enum WinCondition { SelectAll, Other, None };
    public WinCondition winCondition;
    public GameObject[] cubes;
    public bool levelWon;
    public GameObject[] loadExtra;
    //public Bounds bounds;
    public float levelStartTime;
    private List<MenuBehaviour> menus;

	// Use this for initialization
	void Start () {
        levelWon = false;

        menus = new List<MenuBehaviour>();
        foreach (GameObject cube in cubes)
        {
            foreach (Transform t in cube.transform)
            {
                if (t.name.Equals("MenuController"))
                {
                    MenuBehaviour mB = (MenuBehaviour)t.gameObject.GetComponent("MenuBehaviour");
                    menus.Add(mB);
                }
            }

        }
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void performUpdate(float dTime)
    {
        if (winCondition == WinCondition.SelectAll)
        {
            bool won = true;
            foreach (GameObject cube in cubes)
            {
                CubeBehaviour script = (CubeBehaviour)cube.GetComponent("CubeBehaviour");
                if (script.getSelectionState() != CubeBehaviour.SelectionState.Selected)
                {
                    won = false;
                    break;
                }
            }
            if (won)
            {
                //print("Level won!");
                levelWon = true;
            }
        }
        else if (winCondition == WinCondition.Other)
        {
            bool won = true;
            foreach (GameObject cube in cubes)
            {
                CubeBehaviour script = (CubeBehaviour)cube.GetComponent("CubeBehaviour");
                if (!script.getObjComplete())
                {
                    won = false;
                    break;
                }
            }
            if (won)
            {
                //print("Level won!");
                levelWon = true;
            }
        }

        if (levelWon && !gameObject.name.Equals("LevelComplete1to6"))
        {
            ExtraDataRecorder dataRecorder = (ExtraDataRecorder)GameObject.Find("LevelManager").GetComponent("ExtraDataRecorder");
            float time = Time.time - levelStartTime;
            dataRecorder.logData(gameObject.name + " time to finish: " + time);

            if (gameObject.name.Equals("Task5Level"))
            {
                string d = "Task5 Collisions: [";
                foreach (Transform a in transform)
                {
                    if (a.name.Equals("Walls"))
                    {
                        foreach (Transform b in a.transform)
                        {
                            WallBehaviour wB = (WallBehaviour)b.GetComponent("WallBehaviour");
                            d += wB.hitCount + ", ";
                        }
                        break;
                    }
                }
                d += "]";
                dataRecorder.logData(d);
            }
        }

        foreach (GameObject cube in cubes)
        {
            CubeBehaviour script = (CubeBehaviour)cube.GetComponent("CubeBehaviour");
            script.performUpdate(dTime);
        }
    }

    public bool isWon()
    {
        return levelWon;
    }

    public void enableLevel()
    {
        // update2DBounds();
        levelWon = false;
        foreach (GameObject cube in cubes)
        {
            CubeBehaviour script = (CubeBehaviour)cube.GetComponent("CubeBehaviour");
            script.setEnabledState(true);
            //script.setBounds(bounds);
        }

        foreach (GameObject extra in loadExtra)
        {
            extra.SetActive(true);
        }

        levelStartTime = Time.time;
    }

    public void disableLevel()
    {
        foreach (GameObject cube in cubes)
        {
            CubeBehaviour script = (CubeBehaviour)cube.GetComponent("CubeBehaviour");
            script.setEnabledState(false);
        }

        foreach (GameObject extra in loadExtra)
        {
            extra.SetActive(false);
        }
    }

    public void closeAllMenus()
    {
        foreach (MenuBehaviour m in menus)
        {
            m.setMenu(-1);
        }
    }

    /*
    public void update2DBounds()
    {
        MeshFilter mshFilt = ((MeshFilter)gameObject.GetComponent("MeshFilter")); 
        Mesh cubeMesh = mshFilt.mesh;

        Vector3 cubeSize = cubeMesh.bounds.size;
        cubeSize.y = 0;
        bounds = new Bounds(new Vector3(0, 1.5f, 0), cubeSize);
        print("Levelbounds: X: " + bounds.min.x + "/" + bounds.max.x
               + " Z: " + bounds.min.z + "/" + bounds.max.z);
    }

    public Bounds get2DBounds()
    {
        return bounds;
    }*/
}
