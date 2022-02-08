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
    /// List to store variable information during compilation.
    /// </summary>
    private List<string> variableList;

    /// <summary>
    /// Compile a pattern file and add it to the global pattern dictionary.
    /// </summary>
    /// <param name="filePath">The name of the file.</param>
    public void AddPattern(string filePath)
    {
        functionDictionary = new Dictionary<string, List<Action>>();
        variableList = new List<string>();

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
            //TextAsset t = Resources.Load<TextAsset>(filePath);
            //StreamReader sr = new StreamReader(t.text);

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

                    //TODO: test include directive
                    //Include another file in the compilation
                    if(splitLine[0].ToLower() == "include")
                    {
                        
                        List<Action> includedFile = Compile(splitLine[1]);
                        patternData.AddRange(includedFile);

                        //set up next line
                        lineNum++;
                        line = sr.ReadLine();
                        line = line.Trim();

                        splitLine = line.Split(' ');
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

                        //TODO: Bug: compilation error when file ends with endfunction

                        while (splitLine[0].ToLower() != "endfunction")
                        {
                            //process the line into the cluster
                            ProcessLine(splitLine, line, lineNum, functionDictionary[clusterName]);

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
                    ProcessLine(splitLine, line, lineNum, patternData);
                }
                catch(System.Exception e)
                {
                    ThrowError(e.ToString() + "\nFilename: " + filePath, lineNum);
                }
            }

            //Close the streamreader when we're done.
            sr.Close();

            #region Compiled file log
            //Log the compiled file
            string s = ActionListToString(patternData);
            Global.LogReport($"Filename: {filePath}\nCompiled:\n{s}");
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
    private void ProcessLine(string[] splitLine, string line, int lineNum, List<Action> actionList)
    {
        //get the ID of the initial action
        int actionID = ActionStringToInt(splitLine[0]);

        //Throw a compile error if the command isn't recognized
        if (actionID == -2)
        {
            ThrowError("Compile error. Check file.", lineNum);
            return;
        }
        else if(actionID == 1) //spawn a bullet
        {
            //model bullet command:
            //1 0 0 0 0 0 10 0 0 //every other number signifies variable or not

            List<float> parsedLine = new List<float>();

            //Add the main action ID first
            parsedLine.Add(1);

            //Add the location
            AddPotentialVariable(splitLine, parsedLine, 1);

            //x coordinate
            AddPotentialVariable(splitLine, parsedLine, 2);

            //y coordinate
            AddPotentialVariable(splitLine, parsedLine, 3);

            //speed
            AddPotentialVariable(splitLine, parsedLine, 4);

            //direction
            AddPotentialVariable(splitLine, parsedLine, 5);

            //type
            AddPotentialVariable(splitLine, parsedLine, 6);

            //add the action
            actionList.Add(new Action(parsedLine));
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
            
            actionList.AddRange(functionDictionary[splitLine[0]]);
        }
        //do work on a variable. Set, create, increment by
        else if(actionID == 6) //set or create a variable
        {
            if (!variableList.Contains(splitLine[1]))
            {
                //If there's an operator, throw an error
                if (IsOperator(splitLine[2]))
                {
                    ThrowError("Error: operator cannot be used in variable initialization", lineNum);
                    return;
                }

                //create a new variable
                variableList.Add(splitLine[1]);
                #region PrintVarList
                //string str = "";
                //foreach(string s in variableList)
                //{
                //    str += s + " ";
                //}
                //Global.LogReport(str);
                #endregion
            }

            //Create the compiled line, and add 6 to it (the command number for creating/setting variables)
            List<float> parsedLine = new List<float>();
            parsedLine.Add(6);
            //Add the index of the variable to operate on
            parsedLine.Add(variableList.IndexOf(splitLine[1]));

            if (IsOperator(splitLine[2]))
            {
                parsedLine.Add(1); //1 signifies there is an operator
                switch (splitLine[2])
                {
                    case "+":
                        parsedLine.Add(0);
                        break;
                    case "-":
                        parsedLine.Add(1);
                        break;
                    case "*":
                        parsedLine.Add(2);
                        break;
                    case "/":
                        parsedLine.Add(3);
                        break;
                    case "%":
                        parsedLine.Add(4);
                        break;
                    default: break;
                }
            }
            else
            {
                parsedLine.Add(0); //0 signifies no operator
                parsedLine.Add(0); // no operator, this is the space it would be in       
            }

            AddPotentialVariable(splitLine, parsedLine, 3);

            //add the action
            actionList.Add(new Action(parsedLine));

        }
        else if(actionID == 7) //Generate random number in a range
        {
            List<float> parsedLine = new List<float>();
            parsedLine.Add(7);

            //If the variable doesn't exist, throw an error
            if (!variableList.Contains(splitLine[1]))
            {
                ThrowError("Variable not recognized.", lineNum);
                return;
            }

            //add the variable to set
            parsedLine.Add(variableList.IndexOf(splitLine[1]));

            //Minimum value
            AddPotentialVariable(splitLine, parsedLine, 2);

            //Maximum value
            AddPotentialVariable(splitLine, parsedLine, 3);

            actionList.Add(new Action(parsedLine));
        }
        else if(actionID == 8) //point a variable at the player
        {
            List<float> parsedLine = new List<float>();
            parsedLine.Add(8);

            //If the variable doesn't exist, throw an error
            if (!variableList.Contains(splitLine[1]))
            {
                ThrowError("Variable not recognized.", lineNum);
                return;
            }

            //add the variable to set
            parsedLine.Add(variableList.IndexOf(splitLine[1]));

            //Location
            AddPotentialVariable(splitLine, parsedLine, 2);

            //X position
            AddPotentialVariable(splitLine, parsedLine, 3);

            //Y position
            AddPotentialVariable(splitLine, parsedLine, 4);

            actionList.Add(new Action(parsedLine));
        }
        else if (actionID != -1) //-1 is the ignore ID. For comments, empty lines, etc.
        {
            List<float> parsedLine = new List<float>();

            //Add the main action ID first
            parsedLine.Add(actionID);

            //if it's a simple one-word thing, like endrepeat, go ahead and add it
            if(splitLine.Length == 1)
            {
                //Add the action struct
                actionList.Add(new Action(parsedLine));
                return;
            }

            AddPotentialVariable(splitLine, parsedLine, 1);

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
            case "spawn": return 1;
            case "repeat": return 2;
            case "endrepeat": return 3;
            case "log": return 4;
            case "set": return 6; //set or create a variable
            case "random": return 7; //sets a variable to a random number within a range
            case "angletoplayer": return 8; //sets a variable to the angle between given coordinates and the player
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

    /// <summary>
    /// Checks to see whether a given string is a mathematical operator.
    /// </summary>
    private bool IsOperator(string str)
    {
        List<string> operators = new List<string>() 
        {
            "+", "-", "*", "%", "/"
        };

        if (operators.Contains(str))
        {
            return true;
        }

        return false;
    }

    /*
     //check if the number to modify the variable by is a variable itself
            if (variableList.Contains(splitLine[3]))
            {
                parsedLine.Add(1); //signify that it's a variable to follow
                parsedLine.Add(variableList.IndexOf(splitLine[3])); //add the variable index
            }
            else
            {
                parsedLine.Add(0); //signify no variable
                parsedLine.Add(float.Parse(splitLine[3]));
            }
     */

    /// <summary>
    /// Adds a number that may or not be a variable to an action.
    /// </summary>
    /// <param name="valueIndex">The index in the split line that has the number.</param>
    private void AddPotentialVariable(string[] splitLine, List<float> parsedLine, int valueIndex)
    {
        if (variableList.Contains(splitLine[valueIndex]))
        {
            parsedLine.Add(1); //signify that it's a variable to follow
            parsedLine.Add(variableList.IndexOf(splitLine[valueIndex])); //add the variable index
        }
        else
        {
            parsedLine.Add(0); //signify no variable
            parsedLine.Add(float.Parse(splitLine[valueIndex]));
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
