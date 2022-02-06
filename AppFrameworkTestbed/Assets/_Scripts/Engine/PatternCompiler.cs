//Matthew Watson

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#region Action Struct

public struct Action
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
        foreach (float f in splitAction)
        {
            s += $"[{f}]";
        }
        s += "}";
        return s;
    }
}


#endregion //action struct

//Attach to whatever is in charge of compiling the patterns when the game/level starts

//Eventually, move the thing that runs the compiled pattern to a seperate component

public class PatternCompiler
{
    public PatternCompiler() { }

    #region Pattern Interpreter/Compiler

    public void Compile(string filePath)
    {
        //Initialize lists
        List<Action> patternData = new List<Action>();

        // TODO: Re-Implement logs. Temporarily disabled. It's action type 4.
        //logs = new List<string>();

        //Create temporary dictionary to store cluster information
        Dictionary<string, List<Action>> clusterDictionary = new Dictionary<string, List<Action>>();

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
                    line = line.Trim();

                    string[] splitLine = line.Split(' ');

                    //Ignore sections of code
                    if(splitLine[0].ToLower() == "ignore")
                    {
                        do
                        {
                            lineNum++;
                            line = sr.ReadLine();
                            line = line.Trim();

                            splitLine = line.Split(' ');
                        } 
                        while (splitLine[0].ToLower() != "endignore");
                    }

                    if(splitLine[0].ToLower() == "define")
                    {
                        string clusterName = splitLine[1];

                        //Add the cluster to the dictionary
                        clusterDictionary.Add(clusterName, new List<Action>());

                        //Read until it reaches an endcluster command
                        lineNum++;
                        line = sr.ReadLine();
                        line = line.Trim();

                        splitLine = line.Split(' ');

                        while (splitLine[0].ToLower() != "endcluster")
                        {
                            //process the line into the cluster
                            ProcessLine(splitLine, line, lineNum, clusterDictionary[clusterName], clusterDictionary);

                            lineNum++;
                            line = sr.ReadLine();
                            line = line.Trim();

                            splitLine = line.Split(' ');
                        }

                        lineNum++;
                        line = sr.ReadLine();
                        line = line.Trim();

                        splitLine = line.Split(' ');

                        //Log the dictionary entry
                        Global.LogReport($"Putting cluster {clusterName} in dictionary:\n" + ActionListToString(clusterDictionary[clusterName]));
                    }

                    //process the line
                    ProcessLine(splitLine, line, lineNum, patternData, clusterDictionary);
                }
                catch(System.Exception e)
                {
                    ThrowError(e.ToString(), lineNum);
                }
            }

            //Close the streamreader when we're done.
            sr.Close();

            //Add the completed compiled pattern to the global dictionary
            Global.patternDictionary.Add(filePath, patternData);

            #region Compiled file log
            //Log the compiled file
            string s = ActionListToString(patternData);
            Global.LogReport(s);
            #endregion
        }
        else
        {
            Global.LogReport($"File: {Application.dataPath + "/" + filePath} does not exist.");
        }
    }

    private string ActionListToString(List<Action> actionList)
    {
        string s = "";
        foreach (Action a in actionList)
        {
            s += a.ToString() + "\n";
        }

        return s;
    }

    /// <summary>
    /// Process a single line from the pattern file. <br/>
    /// This is just to reuse the code twice, but I may change it, as it's kinda bloated.
    /// </summary>
    /// <param name="splitLine">The split line</param>
    /// <param name="line">the entire line</param>
    /// <param name="lineNum">the current line number</param>
    /// <param name="actionList">the action list to modify</param>
    /// <param name="clusterDictionary">the dictionary storing the clusters</param>
    private void ProcessLine(string[] splitLine, string line, int lineNum, List<Action> actionList, Dictionary<string, List<Action>> clusterDictionary)
    {
        //get the ID of the initial action
        int actionID = ActionStringToInt(splitLine[0]);

        //Throw a compile error if the command isn't recognized
        if (actionID == -2)
        {
            ThrowError("Compile error. Check file.", lineNum);
            return;
        }
        else if (actionID == 4) //add logging data to the logs list
        {
            //current length of logs list is the index we'll find the log in
            //actionList.Add(new Action(new List<float>() { actionID, logs.Count }));

            //substring is to remove "log" from the log
            //logs.Add(line.Substring(3));
        }
        else if(actionID == 5) //call a cluster
        {
            //insert cluster from the dictionary
            actionList.AddRange(clusterDictionary[splitLine[1]]);
        }
        else if (actionID != -1) //-1 is the ignore ID. For comments, empty lines, etc.
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
            actionList.Add(new Action(parsedLine));
        }
    }

    /// <summary>
    /// Convert a string action to its corresponding integer ID.
    /// </summary>
    private int ActionStringToInt(string actionString)
    {
        switch (actionString.ToLower())
        {
            case "wait": return 0;
            case "shoot": return 1;
            case "repeat": return 2;
            case "endrepeat": return 3;
            case "log": return 4;
            case "call": return 5;
            case "//": return -1;//Comment or empty line. Signal to ignore
            case "": return -1;
            case "endignore": return -1;
            default: Global.LogReport($"String {actionString} not recognized."); return -2; //Error, unrecognized thing
        }
    }

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
