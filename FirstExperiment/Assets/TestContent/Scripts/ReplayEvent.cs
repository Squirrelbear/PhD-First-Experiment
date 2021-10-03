using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class ReplayEvent : ISerializable
{
    public enum KeyActionEvent { None = 0, NextLevel = 1, PreviousLevel = 2, FirstLevel = 3, RestartGame = 4,
                                 SetInputMode_Look, SetInputMode_Cursor, SetInputMode_Mobile };

    [NonSerializedAttribute]
    public float deltaTime;
    [NonSerializedAttribute]
    public Quaternion rotation;
    [NonSerializedAttribute]
    public Vector3 position;
    [NonSerializedAttribute]
    public bool triggerEdge;
    [NonSerializedAttribute]
    public KeyActionEvent keyActionEvent;

    public ReplayEvent()
    {
        this.deltaTime = 0;
        this.rotation = new Quaternion();
        this.triggerEdge = false;
        this.keyActionEvent = KeyActionEvent.None;
    }

    public ReplayEvent(float deltaTime, Quaternion rotation, Vector3 position, bool triggerEdge, KeyActionEvent keyActionEvent)
    {
        this.deltaTime = deltaTime;
        this.rotation = new Quaternion();
        this.rotation = rotation;
        this.triggerEdge = triggerEdge;
        this.keyActionEvent = keyActionEvent;
        this.position = position;
    }

     public void GetObjectData(SerializationInfo info, StreamingContext context)
     {
         info.AddValue("dT", deltaTime);
         info.AddValue("trigger", triggerEdge);
         info.AddValue("ROTx",rotation.x);
         info.AddValue("ROTy",rotation.y);
         info.AddValue("ROTz",rotation.z);
         info.AddValue("ROTw",rotation.w);
         info.AddValue("POSx", position.x);
         info.AddValue("POSy", position.y);
         info.AddValue("POSz", position.z);
         info.AddValue("KeyEvent", (int)keyActionEvent);
     }

     public ReplayEvent(SerializationInfo info, StreamingContext ctxt)
     {
         //Get the values from info and assign them to the appropriate properties
         deltaTime = (float)info.GetValue("dT", typeof(float));
         triggerEdge = (bool)info.GetValue("trigger", typeof(bool));
         rotation.x = (float)info.GetValue("ROTx", typeof(float));
         rotation.y = (float)info.GetValue("ROTy", typeof(float));
         rotation.z = (float)info.GetValue("ROTz", typeof(float));
         rotation.w = (float)info.GetValue("ROTw", typeof(float));
         position.x = (float)info.GetValue("POSx", typeof(float));
         position.y = (float)info.GetValue("POSy", typeof(float));
         position.z = (float)info.GetValue("POSz", typeof(float));
         keyActionEvent = (KeyActionEvent)info.GetValue("KeyEvent", typeof(int));
     }
	
}
