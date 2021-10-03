using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class ExtraDataRecorder : MonoBehaviour {

    public List<string> replayEvents;
    //public string saveFile = "";

	// Use this for initialization
	void Start () {
        PersistantStateBehaviour pSB = (PersistantStateBehaviour)GameObject.Find("PersistantStateManager").GetComponent("PersistantStateBehaviour");

        replayEvents = pSB.getExtraDataRef();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /*public void setSaveName(string saveFile)
    {
        this.saveFile = saveFile;
    }*/

    public void logData(string data)
    {
        string now = DateTime.Now.ToString();
        replayEvents.Add(now + " " + data);
    }

    /*void OnApplicationQuit()
    {
        GameObject oculus = GameObject.Find("OVRCameraRig");
        bool usingOculus;
        if (oculus == null)
        {
            usingOculus = false;
        }
        else
        {
            usingOculus = oculus.activeSelf;
        }
        if (saveFile.Equals("") || !usingOculus)
        {
            return;
        }

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(saveFile))
        {
            foreach (string line in replayEvents)
            {
                file.WriteLine(line);
            }
        }
    }*/
}
