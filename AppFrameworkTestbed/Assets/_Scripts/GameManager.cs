//Matthew Watson

//GameManager object
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton class to operate as the heart of the game's code. <br/>
/// Unless an exception is absolutely necessary, other scripts should only reference this one.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The notification management object
    /// </summary>
    public Messenger messenger;

    /// <summary>
    /// The object pool.
    /// </summary>
    public ObjectPool objectPool;

    /// <summary>
    /// Pattern Compiler
    /// </summary>
    private PatternCompiler patternCompiler;

    #region Singleton

    //The allowed instance of the GameManager
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        Global.gameTime = 0;

        if (_instance != null && _instance != this)
        {
            //Get rid of it if there's already one in the scene.
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;

            //I want my Game Manager to stick around
            DontDestroyOnLoad(gameObject);

            //Separate this into another part of code so
            //it's not hidden in the Singleton implementation.
            Initialize();
        }
    }
    #endregion

    #region Ticker List
    /// <summary>
    /// List of objects that need determinstic updates.
    /// </summary>
    private List<ManagedObject> managedObjects;

    public void AddTicker(ManagedObject t)
    {
        managedObjects.Add(t);
    }

    public void RemoveTicker(ManagedObject t)
    {
        managedObjects.Remove(t);
    }
    #endregion

    #region HitCircle List
    private List<HitCircle> hitCircles;

    public void AddHitCircle(HitCircle hc)
    {
        hitCircles.Add(hc);
    }

    public void RemoveHitCircle(HitCircle hc)
    {
        hitCircles.Remove(hc);
    }
    #endregion

    /// <summary>
    /// Called after creating the singleton. Place setup information here.<br/>
    /// Keep it to a minimum, please, future me.
    /// </summary>
    private void Initialize()
    {
        //construct the messenger
        messenger = new Messenger();

        //initialize the ticker list
        managedObjects = new List<ManagedObject>();

        //initialize the hit circle list
        hitCircles = new List<HitCircle>();

        //Disable vSync and lock framerate at 60fps
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        //Sets the aspect ratio to 6:9
        SetAspectRatio(6, 9);

        //compile the test file
        Global.patternDictionary = new Dictionary<string, List<Action>>();
        patternCompiler = new PatternCompiler();
        patternCompiler.Compile("Pattern.txt");
    }

    #region Screen Setup
    /// <summary>
    /// Sets the aspect ratio to the specified numbers. <br/>
    /// Form- height:width
    /// </summary>
    /// <param name="width">Second number</param>
    /// <param name="height">First number</param>
    private void SetAspectRatio(int height, int width)
    {
        int screenHeight = Screen.height;
        int screenWidth = Screen.width;



        if (screenWidth >= screenHeight * 1.5f)
        {
            screenWidth = (int)((width * screenHeight) / (float)height);
        }
        else
        {
            screenHeight = (int)((height * screenWidth) / (float)width);
        }

        Screen.SetResolution(screenWidth, screenHeight, true);
    }
    #endregion

    /// <summary>
    /// In this method, update all notifiable objects and the messenger.
    /// </summary>
    private void Update()
    {
        /* Order:
         * -Get input data
         * -Update objects
         * -Respond to notifications
         */

        //Update each tickable object
        foreach(ManagedObject t in managedObjects)
        {
            if(!t.excludeTick) t.Tick();
        }

        //make objects respond to notifications
        messenger.Update();
    }

    /// <summary>
    /// Tests whether or not the HitCircle self is touching a HitCircle on an object tagged with tag.
    /// </summary>
    /// <param name="self">The object doing the searching.</param>
    /// <param name="tag">The tag to search for.</param>
    /// <returns>True if collision, False if not.</returns>
    public bool TestForHit(HitCircle self, string tag)
    {
        //Iterate through each hitcircle in the list.
        foreach(HitCircle hc in hitCircles)
        {
            //Check if self is touching the current hitcircle
            if(hc != self && self.isTouching(hc))
            {
                //If the tag matches what you're looking for, return true
                if(hc.tag == tag)
                {
                    return true;
                }
            }
        }

        //If we make it to the end of the list, nothing we were looking for collided. Return false.
        return false;
    }
}
