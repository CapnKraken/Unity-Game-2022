using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Handles the spawning of enemy bullets.
/// </summary>
public class BulletSystem : NotifiableObj
{

    protected override void Initialize()
    {
        //BulletSystem responds to shooting events
        relevantCategories.Add(Category.Shooting);
    }

    public override string GetLoggingData()
    {
        return name;
    }

    public override void OnNotify(Category category, string message, string senderData)
    {
        if (category == Category.Shooting) {
            string[] splitMessage = message.Split(' ');

            if (splitMessage[0] == "Shoot")
            {
                Global.LogReport("Shooting, requested by " + senderData);

                try
                {
                    //attempt to convert the split message into values for the bullet
                    float bulletSpeed = float.Parse(splitMessage[1]);
                    float bulletDirection = float.Parse(splitMessage[2]);
                }
                catch(Exception e)
                {
                    Global.LogReport(e.ToString());
                }
            }
        }
    }
}
