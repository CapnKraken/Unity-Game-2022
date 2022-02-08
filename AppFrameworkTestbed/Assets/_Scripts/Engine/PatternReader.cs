using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to enemies and bosses

public class PatternReader : ManagedObject
{
    public string patternFileName;

    /// <summary>
    /// The list of commands/actions that the program reads.
    /// </summary>
    private List<Action> patternData;

    /// <summary>
    /// the list of variables.
    /// </summary>
    private List<float> variables;

    #region Timer and Iterator

    /// <summary>
    /// The program will wait if this is greater than 0.
    /// </summary>
    private int waitTimer;

    /// <summary>
    /// The variable that iterates through the action list. <br/>
    /// Use it to tell you which line the program is on.
    /// </summary>
    private int actionIterator;

    #endregion

    #region Repeater Stack and Struct

    private Stack<Repeater> repeaterStack;
    /// <summary>
    /// Struct to store 2 ints: A repeat counter and a line number representing where to jump back to.
    /// </summary>
    private struct Repeater
    {
        public int repeats, lineNumber;
        public Repeater(int repeats, int lineNumber)
        {
            this.repeats = repeats;
            this.lineNumber = lineNumber;
        }
    }

    #endregion

    /// <summary>
    /// What the bullet script uses to debug log.
    /// </summary>
    private List<string> logs;

    #region Update
    public override void Tick()
    {
        if (patternData.Count == 0) return;

        if (waitTimer == 0)
        {
            //In a dowhile so it can run multiple commands per frame, if there's no wait in between
            do
            {
                //Execute the action
                DoAction(patternData[actionIterator]);
                actionIterator++;

                //Wrap actionIterator if it exceeds the list length
                if (actionIterator >= patternData.Count) actionIterator = 0;
            }
            while (waitTimer == 0);
        }
        else
        {
            //Count down if it's waiting.
            waitTimer--;
        }
    }
    #endregion

    #region Do Action
    /// <summary>
    /// Perform an action.
    /// </summary>
    private void DoAction(Action action)
    {
        //First number in splitAction should be an int
        switch ((int)action.splitAction[0])
        {
            case 0: //WAIT
                //Convert seconds to frames
                //Global.LogReport($"Waiting {action.splitAction[1]} seconds.");

                waitTimer = (int)(GetValueFromPotentialVariable(action.splitAction, 1) * 60);

                break;
            case 1: //SHOOT
                //position to spawn the bullet
                float xPosition, yPosition;

                //get location (0: self, 1: world)
                float location = GetValueFromPotentialVariable(action.splitAction, 1);

                if (location == 0)
                {
                    xPosition = transform.position.x;
                    yPosition = transform.position.y;
                }
                else
                {
                    xPosition = GameManager.Instance.screenParent.position.x;
                    yPosition = GameManager.Instance.screenParent.position.y;
                }

                //get position
                float xOffset = GetValueFromPotentialVariable(action.splitAction, 3);
                float yOffset = GetValueFromPotentialVariable(action.splitAction, 5);

                //get speed and direction
                float speed = GetValueFromPotentialVariable(action.splitAction, 7);
                float direction = GetValueFromPotentialVariable(action.splitAction, 9);

                //get type
                int type = (int)GetValueFromPotentialVariable(action.splitAction, 11);

                Notify(Category.Shooting, $"Shoot {xPosition + xOffset} {yPosition + yOffset} {speed} {direction} {type}");
                //Global.LogReport("Shooting");
                break;
            case 2: //REPEAT
                {
                    float repeatCount = GetValueFromPotentialVariable(action.splitAction, 1);

                    //add a new repeater to the stack
                    Repeater temp1 = new Repeater((int)repeatCount, actionIterator);
                    repeaterStack.Push(temp1);

                    //Global.LogReport($"Repeating {temp1.repeats} times.");
                    break;
                }
            case 3: //ENDREPEAT
                {
                    Repeater temp2 = repeaterStack.Pop();
                    temp2.repeats--;

                    //if r.repeats == 0, then the program will continue
                    if (temp2.repeats > 0)
                    {
                        //Set the actionIterator to point to the start of the repeat
                        actionIterator = temp2.lineNumber;

                        //Global.LogReport($"Repeating {temp2.repeats} more times.");

                        //push it back onto the stack
                        repeaterStack.Push(temp2);
                    }
                    break;
                }
            case 4:
                {
                    //Global.LogReport(logs[(int)action.splitAction[1]]);
                }
                break;
            case 6: //work on a variable
                if(variables.Count <= action.splitAction[1])
                {
                    //create the variable if it doesn't exist already
                    variables.Add(0);
                    //Global.LogReport("Count: " + variables.Count);
                }

                //are we creating a variable and setting it to the value of another?
                float givenNumber = GetValueFromPotentialVariable(action.splitAction, 4);

                if (action.splitAction[2] == 1)
                {
                    switch (action.splitAction[3])
                    {
                        case 0: // +
                            variables[(int)action.splitAction[1]] += givenNumber;
                            break;
                        case 1: // -
                            variables[(int)action.splitAction[1]] -= givenNumber;
                            break;
                        case 2: // *
                            variables[(int)action.splitAction[1]] *= givenNumber;
                            break;
                        case 3: // /
                            variables[(int)action.splitAction[1]] /= givenNumber;
                            break;
                        default: break;
                    }
                }
                else
                {
                    //set the variable to the number
                    //Global.LogReport($"Variables.Count: {variables.Count}\nSplitAction[1]: {action.splitAction[1]}");
                    variables[(int)action.splitAction[1]] = givenNumber;
                }

                break;
            case 7: //random number

                //get min and max
                float min = GetValueFromPotentialVariable(action.splitAction, 2);
                float max = GetValueFromPotentialVariable(action.splitAction, 4);

                variables[(int)action.splitAction[1]] = Random.Range(min, max);
                break;
            case 8: //point at player

                location = GetValueFromPotentialVariable(action.splitAction, 2);

                //x pos and y pos
                float initX = GetValueFromPotentialVariable(action.splitAction, 4);
                float initY = GetValueFromPotentialVariable(action.splitAction, 6);

                //0 = relative to self, 1 = relative to world
                if(location == 0)
                {
                    initX += transform.position.x;
                    initY += transform.position.y;
                }
                else
                {
                    initX += GameManager.Instance.screenParent.position.x;
                    initY += GameManager.Instance.screenParent.position.y;
                }

                variables[(int)action.splitAction[1]] = Global.PointTowards(new Vector3(initX, initY, 0), GameManager.Instance.player.transform.position);
                break;
            default: break;
        }
    }

    /// <summary>
    /// Gets a value either directly from the command, or from a variable value in the variable list.
    /// </summary>
    /// <param name="indexOfIndicator">The index of the flag that indicates whether or not the next index is a variable.</param>
    private float GetValueFromPotentialVariable(List<float> splitAction, int indexOfIndicator)
    {
        if(splitAction[indexOfIndicator] == 1)
        {
            return variables[(int)splitAction[indexOfIndicator + 1]];
        }
        else
        {
            return splitAction[indexOfIndicator + 1];
        }
    }

    #endregion

    #region Initialize
    protected override void Initialize()
    {
        //Initialize variables
        waitTimer = 0;
        actionIterator = 0;

        //Initialize data structures
        repeaterStack = new Stack<Repeater>();

        patternData = Global.patternDictionary[patternFileName];

        variables = new List<float>();
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
}
