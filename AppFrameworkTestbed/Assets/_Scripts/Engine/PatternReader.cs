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
                Global.LogReport($"Waiting {action.splitAction[1]} seconds.");
                waitTimer = (int)(action.splitAction[1] * 60);
                break;
            case 1: //SHOOT
                Notify(Category.Shooting, $"Shoot {transform.position.x} {transform.position.y} 10, -90");
                //Global.LogReport("Shooting");
                break;
            case 2: //REPEAT
                {
                    //Add a new repeater struct to the stack
                    Repeater temp1 = new Repeater((int)action.splitAction[1], actionIterator);
                    repeaterStack.Push(temp1);

                    Global.LogReport($"Repeating {temp1.repeats} times.");
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

                        Global.LogReport($"Repeating {temp2.repeats} more times.");

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
