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
                if(action.splitAction[1] == 0)
                {
                    //set wait to the value of action.splitAction[2]
                    waitTimer = (int)(action.splitAction[2] * 60);
                }
                else
                {
                    //set wait to the value of the variable indexed at action.splitAction[2]
                    waitTimer = (int)(variables[(int)action.splitAction[2]] * 60); 
                }
                break;
            case 1: //SHOOT
                //position to spawn the bullet
                float xPosition, yPosition;
                //speed and direction of bullet
                float speed, direction;

                if(action.splitAction[1] == 0)
                {
                    xPosition = transform.position.x;
                    yPosition = transform.position.y;
                }
                else
                {
                    xPosition = GameManager.Instance.screenParent.position.x;
                    yPosition = GameManager.Instance.screenParent.position.y;
                }

                float xOffset, yOffset;

                //get x position
                if (action.splitAction[2] == 0)
                {
                    xOffset = action.splitAction[3];
                }
                else
                {
                    xOffset = variables[(int)action.splitAction[3]];
                }

                //get y position
                if (action.splitAction[4] == 0)
                {
                    yOffset = action.splitAction[5];
                }
                else
                {
                    yOffset = variables[(int)action.splitAction[5]];
                }

                //get speed
                if (action.splitAction[6] == 0)
                {
                    speed = action.splitAction[7];
                }
                else
                {
                    speed = variables[(int)action.splitAction[7]];
                }

                //get direction
                if (action.splitAction[8] == 0)
                {
                    direction = action.splitAction[9];
                }
                else
                {
                    direction = variables[(int)action.splitAction[9]];
                }


                Notify(Category.Shooting, $"Shoot {xPosition + xOffset} {yPosition + yOffset} {speed}, {direction}");
                //Global.LogReport("Shooting");
                break;
            case 2: //REPEAT
                {
                    if(action.splitAction[1] == 0)
                    {
                        //Add a new repeater struct to the stack
                        Repeater temp1 = new Repeater((int)action.splitAction[2], actionIterator);
                        repeaterStack.Push(temp1);
                    }
                    else
                    {
                        //Add a new repeater struct to the stack, using the variable as the thing
                        Repeater temp1 = new Repeater((int)variables[(int)action.splitAction[2]], actionIterator);
                        repeaterStack.Push(temp1);
                    }

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

                    //are we creating a variable and setting it to the value of another?
                    if(action.splitAction[4] == 1)
                    {

                    }
                    else //if not, set the value directly
                    {

                    }
                }
                break;
            default: break;
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
