using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ReplayReader
{
    public ReplayDatabase eventDatabase;
    public int curFrameID;

    public ReplayReader(string filename)
    {
        eventDatabase = new ReplayDatabase();
        eventDatabase.loadDatabase(filename);

        // read the first frame
        ReplayEvent curFrame = eventDatabase.getNextEvent();
        if (curFrame == null)
        {
            Console.WriteLine("Error: No first frame for replay.");
            return;
        }

        curFrameID = 0;
        if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Look)
        {
            Console.WriteLine("Input mode set to Look At");
        }
        else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Cursor)
        {
            Console.WriteLine("Input mode set to Click");
        }
        else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Mobile)
        {
            Console.WriteLine("Input mode set to Mobile");
        }
    }

    public void processAllEvents()
    {
        ReplayEvent curFrame;
        float time = 0;
        while ((curFrame = eventDatabase.getNextEvent()) != null)
        {
            bool clickOccured = curFrame.triggerEdge;
            time += curFrame.deltaTime;

            if (curFrame.keyActionEvent != ReplayEvent.KeyActionEvent.None)
            {
                Console.Write("Time: " + time + "\t\t");
            }

            if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.NextLevel)
            {
                Console.WriteLine("Next level action event triggered.");
            }
            else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.PreviousLevel)
            {
                Console.WriteLine("Previous level action event triggered.");
            }
            else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.FirstLevel)
            {
                Console.WriteLine("Return to first action event triggered.");
            }
            else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.RestartGame)
            {
                Console.WriteLine("Restart game event was triggered.");
            }
            else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Look)
            {
                Console.WriteLine("Input mode set to Look At");
            }
            else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Cursor)
            {
                Console.WriteLine("Input mode set to Click");
            }
            else if (curFrame.keyActionEvent == ReplayEvent.KeyActionEvent.SetInputMode_Mobile)
            {
                Console.WriteLine("Input mode set to Mobile");
            }
        }
    }
}

