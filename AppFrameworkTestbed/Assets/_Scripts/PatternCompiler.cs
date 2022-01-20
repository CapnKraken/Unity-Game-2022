//Matthew Watson

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Attach to whatever is in charge of compiling the patterns when the game/level starts

public class PatternCompiler : NotifiableObj, iTickable
{
    /// <summary>
    /// The list of commands/actions that the program reads.
    /// </summary>
    private List<Action> patternData;

    /// <summary>
    /// The program will wait if this is greater than 0.
    /// </summary>
    private int waitTimer;

    /// <summary>
    /// The variable that iterates through the action list.
    /// </summary>
    private int actionIterator;

    public void Tick()
    {
        if(waitTimer == 0)
        {
            //Execute the action
            DoAction(patternData[actionIterator]);
            actionIterator++;

            //Wrap actionIterator if it exceeds the list length
            if (actionIterator >= patternData.Count) actionIterator = 0;
        }
        else
        {
            //Count down if it's waiting.
            waitTimer--;
        }
    }

    private void DoAction(Action action)
    {
        //First number in splitAction should be an int
        switch ((int)action.splitAction[0])
        {
            case 0: //WAIT
                //Convert seconds to frames
                Global.LogReport($"Waiting {action.splitAction[1]} seconds.");
                waitTimer = (int)(action.splitAction[1] * 60);
                break;
            case 1: //SHOOT
                Notify(Category.Shooting, "Shoot");
                break;
            default: break;
        }
    }

    #region Initialize and Deinitialize
    protected override void Initialize()
    {
        GameManager.Instance.AddTicker(this);

        Compile("Pattern.txt");

        //initialize variables
        waitTimer = 0;
        actionIterator = 0;
    }

    protected override void DeInitialize()
    {
        GameManager.Instance.RemoveTicker(this);
    }

    #endregion

    #region Notifications
    public override void OnNotify(Category category, string message, string senderData)
    {
        
    }

    public override string GetLoggingData()
    {
        return name;
    }
    #endregion

    #region Pattern Interpreter/Compiler

    private void Compile(string filePath)
    {
        patternData = new List<Action>();

        //Check if the file exists.
        if (File.Exists(Application.dataPath + "/" + filePath))
        {
            //Declare and initialize the stream reader.
            StreamReader sr = new StreamReader(Application.dataPath + "/" + filePath);

            //keep track of line number for error finding
            int lineNum = 0;
            while (!sr.EndOfStream)
            {
                lineNum++;
                try
                {
                    //Read a line from the file and split it.
                    string line = sr.ReadLine();
                    string[] splitLine = line.Split(' ');

                    //get the ID of the initial action
                    int actionID = ActionStringToInt(splitLine[0]);

                    //-1 is the ignore ID. For comments, empty lines, etc.
                    if (actionID != -1)
                    {
                        List<float> parsedLine = new List<float>();

                        //Add the main action ID first
                        parsedLine.Add(actionID);

                        //Convert to floats
                        for (int i = 1; i < splitLine.Length; i++)
                        {
                            //Add all of the modifiers
                            parsedLine.Add(float.Parse(splitLine[i]));
                        }

                        //Add the action struct
                        patternData.Add(new Action(parsedLine));
                    }
                }
                catch(System.Exception e)
                {
                    ThrowError(e.ToString(), lineNum);
                }
            }

            //Close the streamreader when we're done.
            sr.Close();

            #region Compiled file log
            //Log the compiled file
            string s = "";
            foreach(Action a in patternData)
            {
                s += a.ToString() + "\n";
            }
            Global.LogReport(s);
            #endregion
        }
        else
        {
            Global.LogReport($"File: {Application.dataPath + "/" + filePath} does not exist.");
        }
    }

    /// <summary>
    /// Convert a string action to its corresponding integer ID.
    /// </summary>
    private int ActionStringToInt(string actionString)
    {
        switch (actionString)
        {
            case "Wait": return 0;
            case "Shoot": return 1;
            default: return -1; //Comment or empty line. Signal to ignore
        }
    }

    #region Action Struct
    
    private struct Action
    {
        //list to store the action
        public List<float> splitAction;

        //The length of the list
        public int length;

        public Action(List<float> splitAction)
        {
            this.splitAction = splitAction;
            this.length = splitAction.Count;
        }

        public override string ToString()
        {
            string s = "{";
            foreach(float f in splitAction)
            {
                s += $"[{f}]";
            }
            s += "}";
            return s;
        }
    }
        
        
    #endregion //action struct

    #endregion //compiler

    /// <summary>
    /// Logs a report of an error with the pattern file to the console.
    /// </summary>
    /// <param name="error">The string representation of the error.</param>
    /// <param name="line">The line on the pattern file where the error occured.</param>
    private void ThrowError(string error, int line)
    {
        //Report the error message along with the line number in the file where it occured
        Global.LogReport($"Error on line {line}\n\n" + error.ToString());
    }
}
