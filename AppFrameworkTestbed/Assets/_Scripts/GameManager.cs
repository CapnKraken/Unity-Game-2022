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
    private List<iTickable> tickers;

    public void AddTicker(iTickable t)
    {
        tickers.Add(t);
    }

    public void RemoveTicker(iTickable t)
    {
        tickers.Remove(t);
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
        tickers = new List<iTickable>();

        //initialize the hit circle list
        hitCircles = new List<HitCircle>();

        //Disable vSync and lock framerate at 60fps
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

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
        foreach(iTickable t in tickers)
        {
            t.Tick();
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
