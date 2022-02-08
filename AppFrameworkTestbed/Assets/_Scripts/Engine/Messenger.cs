//Matthew Watson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to handle between-script messaging. <br/>
/// Objects with script components extending NotifiableObj class can send a message to all other like objects.
/// </summary>
public class Messenger
{
    /// <summary>
    /// The list of notifiable objects listening for notifications.
    /// </summary>
    private List<ManagedObject> listeners;

    /// <summary>
    /// A queue to which all notifications are added.
    /// </summary>
    private Queue<Notification> messageQueue;

    #region Constructor
    /// <summary>
    /// Constructor initializes the list of listeners.
    /// </summary>
    public Messenger()
    {
        //Initialize the objects list
        listeners = new List<ManagedObject>();

        //Initialize the message queue
        messageQueue = new Queue<Notification>();
    }
    #endregion

    #region Proclaim Message
    /// <summary>
    /// Send a message to all relevant listeners.
    /// </summary>
    /// <param name="category">Broad category of message. Message will only be sent to listeners whose category matches.</param>
    /// <param name="message">The actual message being sent. For instance, "PlayerDeath" or "Extend."<br/>If you need to include extra data, include it in this string after the event name, and seperate it by spaces.</param>
    /// <param name="senderData">Whatever the sender's GetLoggingData method returns.</param>
    public void Proclaim(Category category, string message, string senderData)
    {
        //Add the notification to the message queue
        messageQueue.Enqueue(new Notification(category, message, senderData));
    }
    #endregion

    #region Listener List Ops
    /// <summary>
    /// Add listener to the list.
    /// </summary>
    public void AddListener(ManagedObject listener)
    {
        listeners.Add(listener);
    }

    /// <summary>
    /// Remove listener from the list.
    /// </summary>
    public void RemoveListener(ManagedObject listener)
    {
        listeners.Remove(listener);
    }
    #endregion

    #region Update Method
    public void Update()
    {
        //Empty the queue
        while(messageQueue.Count != 0)
        {
            //create an instance of the struct
            Notification n = messageQueue.Dequeue();

            foreach (ManagedObject obj in listeners)
            {
                //Notify each object if the message pertains to them. GENERAL messages will be sent to every object.
                if (obj.relevantCategories.Contains(n.category) || n.category == Category.GENERAL)
                {
                    //Notify the object.
                    n.NotifyObject(obj);
                }
            }
        }
    }
    #endregion

    #region Notification Struct
    /// <summary>
    /// An struct which stores the notification data. <br/>
    /// The message queue is populated with these.
    /// </summary>
    private struct Notification
    {
        public Category category;
        public string message, senderData;
        public Notification(Category category, string message, string senderData)
        {
            this.category = category;
            this.message = message;
            this.senderData = senderData;
        }

        //Notify the object of the message.
        public void NotifyObject(ManagedObject recipient)
        {
            recipient.OnNotify(category, message, senderData);
        }
    }

    #endregion
}

public enum Category
{
    GENERAL, Score, Shooting, Audio
}
