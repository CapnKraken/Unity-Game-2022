using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if(message == "Shoot")
        {
            Global.LogReport("Shooting, requested by " + senderData);
        }
    }
}
