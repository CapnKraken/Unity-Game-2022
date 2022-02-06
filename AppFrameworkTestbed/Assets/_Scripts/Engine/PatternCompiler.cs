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

public class PatternCompiler
{
    public PatternCompiler() { }

    #region Pattern Interpreter/Compiler

    /// <summary>
    /// Dictionary to store cluster information during compilation.
    /// </summary>
    private Dictionary<string, List<Action>> functionDictionary;

    /// <summary>
    /// Dictionary to store variable information during compilation.
    /// </summary>
    private Dictionary<string, float> variableDictionary;

    /// <summary>
    /// Compile a pattern file and add it to the global pattern dictionary.
    /// </summary>
    /// <param name="filePath">The name of the file.</param>
    public void AddPattern(string filePath)
    {
        functionDictionary = new Dictionary<string, List<Action>>();
        variableDictionary = new Dictionary<string, float>();

        //Add the pattern to the dictionary
        Global.patternDictionary.Add(filePath, Compile(filePath));
    }

    public List<Action> Compile(string filePath)
    {
        //Initialize lists
        List<Action> patternData = new List<Action>();

        // TODO: Re-Implement logs. Temporarily disabled. It's action type 4.
        //logs = new List<string>();

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

                    //Include another file in the compilation
                    if(splitLine[0].ToLower() == "include")
                    {
                        try
                        {
                            patternData.AddRange(Compile(splitLine[1]));
                        }catch
                        {
                            ThrowError("Error with include directive.", lineNum);
                        }
                    }

                    //define a function
                    if(splitLine[0].ToLower() == "define")
                    {
                        string clusterName = splitLine[1];

                        //Add the cluster to the dictionary
                        functionDictionary.Add(clusterName, new List<Action>());

                        //Read until it reaches an endcluster command
                        lineNum++;
                        line = sr.ReadLine();
                        line = line.Trim();

                        splitLine = line.Split(' ');

                        while (splitLine[0].ToLower() != "endfunction")
                        {
                            //process the line into the cluster
                            ProcessLine(splitLine, line, lineNum, functionDictionary[clusterName], functionDictionary);

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
                        Global.LogReport($"Putting cluster {clusterName} in dictionary:\n" + ActionListToString(functionDictionary[clusterName]));
                    }

                    //process the line
                    ProcessLine(splitLine, line, lineNum, patternData, functionDictionary);
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
            string s = ActionListToString(patternData);
            Global.LogReport(s);
            #endregion
            return patternData;
        }
        else
        {
            Global.LogReport($"File: {Application.dataPath + "/" + filePath} does not exist.");
            return null;
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
            actionList.AddRange(clusterDictionary[splitLine[0]]);
        }
        else if(actionID == 6) //set or create a variable
        {
            if (variableDictionary.ContainsKey(splitLine[1]))
            {
                //update the variable
                variableDictionary[splitLine[1]] = float.Parse(splitLine[2]);
            }
            else
            {
                //create a new variable
                variableDictionary.Add(splitLine[1], float.Parse(splitLine[2]));
            }
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
            case "set": return 6; //set or create a variable
            case "//": return -1;//Comment or empty line. Signal to ignore
            case "": return -1;
            case "endignore": return -1;
            default:
                if (functionDictionary.ContainsKey(actionString))
                {
                    return 5; //call a function
                }
                else
                {
                    Global.LogReport($"String {actionString} not recognized.");
                    return -2; //Error, unrecognized thing
                }
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
