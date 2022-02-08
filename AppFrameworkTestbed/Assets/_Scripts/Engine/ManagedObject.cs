//Matthew Watson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that can send and receive messages via the Messenger system, and runs in the GameManager's update method via the Tick method.
/// </summary>
public abstract class ManagedObject : MonoBehaviour
{
    //Contains abstract method OnNotify
    #region Communication with messenger

    /// <summary>
    /// The Game Manager's Messenger object.
    /// </summary>
    public Messenger messenger;

    /// <summary>
    /// Which categories of messages to listen for.
    /// </summary>
    public List<Category> relevantCategories;

    /// <summary>
    /// Through messenger, send a notification to all objects listening for that category.
    /// </summary>
    /// <param name="category">Broad category of message. Message will only be sent to listeners whose category matches.</param>
    /// <param name="message">The actual message being sent. For instance, "PlayerDeath" or "Extend."<br/>Extra data may be included here, after the name. Separate it with spaces.</param>
    protected void Notify(Category category, string message)
    {
        messenger.Proclaim(category, message, GetLoggingData());
    }

    /// <summary>
    /// Allows the object to respond to notifications.
    /// </summary>
    /// <param name="category">Broad category of message. Message will only be sent to listeners whose category matches.</param>
    /// <param name="message">The actual message being sent. For instance, "PlayerDeath" or "Extend."<br/>Extra data may be included here, after the name, separated by spaces.<br/>For instance, "DealDamage 12" where 12 is taken by the recipient as the amount of damage.</param>
    /// <param name="senderData">Whatever the sender's GetLoggingData method returns.</param>
    public abstract void OnNotify(Category category, string message, string senderData);

    #endregion

    #region Tick
    /// <summary>
    /// Set this to true if you don't want GameManager to update it.
    /// </summary>
    public bool excludeTick = false;

    /// <summary>
    /// Use this instead of the Update method.
    /// </summary>
    public virtual void Tick()
    {

    }
    #endregion

    //Contains abstract method Initialize
    #region Start and Destroy

    public void Start()
    {
        //Get gamemanager's messenger
        messenger = GameManager.Instance.messenger;

        messenger.AddListener(this);
        Initialize();

        GameManager.Instance.AddTicker(this);
    }

    public void OnDestroy()
    {
        messenger.RemoveListener(this);
        DeInitialize();

        GameManager.Instance.RemoveTicker(this);
    }

    /// <summary>
    /// Implement this method instead of Start for objects extending NotifiableObj.
    /// </summary>
    protected abstract void Initialize();

    /// <summary>
    /// Implement this method instead of OnDestroy for objects extending NotifiableObj.
    /// </summary>
    protected virtual void DeInitialize()
    {
        //Global.LogReport("De-initialized object " + name);
    }

    #endregion

    /// <summary>
    /// Return relevant debugging information here.
    /// </summary>
    public abstract string GetLoggingData();
}
