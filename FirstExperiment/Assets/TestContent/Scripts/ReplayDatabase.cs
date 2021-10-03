using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Xml.Serialization;

public class ReplayDatabase
{
    public List<ReplayEvent> replayEvents;
    public int replayEventID;
    public float excessDeltaTime;
    private string databaseCreation;

    public ReplayDatabase()
    {
        replayEvents = new List<ReplayEvent>();
        System.DateTime now = System.DateTime.Now;
        string pattern = @"yyyy_M_d__hh_mm";
        databaseCreation = now.ToString(pattern);
    }

    public void addEvent(float deltaTime, Quaternion rotation, Vector3 position, bool triggerEdge, ReplayEvent.KeyActionEvent keyActionEvent)
    {
        replayEvents.Add(new ReplayEvent(deltaTime, rotation, position, triggerEdge, keyActionEvent));
    }

    public void addEvent(ReplayEvent replayEvent)
    {
        replayEvents.Add(replayEvent);
    }

    public int getCurFrameID()
    {
        return replayEventID;
    }

    public ReplayEvent getNextEvent()
    {
        if (replayEventID >= replayEvents.Count)
        {
            return null;
        }

        ReplayEvent next = replayEvents[replayEventID];
        replayEventID++;
        return next;
    }

    public void saveDatabase(string fileName)
    {
        try
        {
            using (Stream stream = File.Open(fileName, FileMode.Create))
            {
                //XmlSerializer x = new XmlSerializer(typeof(ReplayEvent));
                //x.Serialize(stream, replayEvents);
                
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, replayEvents);

            }
        }
        catch (IOException)
        {
        }
    }

    public void loadDatabase(string fileName)
    {
        try
        {
            using (Stream stream = File.Open(fileName, FileMode.Open))
            {
                BinaryFormatter bin = new BinaryFormatter();
                
                replayEvents = (List<ReplayEvent>)bin.Deserialize(stream);

                //XmlSerializer x = new XmlSerializer(typeof(ReplayEvent));
                //replayEvents = (List<ReplayEvent>)x.Deserialize(stream);

            }
        }
        catch (IOException e)
        {
            MonoBehaviour.print("ERROR LOADING REPLAY: " + e.Message);
        }
    }

    public string getCreationData()
    {
        return databaseCreation;
    }
}
