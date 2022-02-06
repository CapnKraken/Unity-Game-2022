using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Handles the spawning of enemy bullets.
/// </summary>
public class BulletSystem : ManagedObject
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
                    //sample notification
                    //Shoot 50 50 10 90
                    //Shoot at position 50, 50. Speed 10, direction: 90.

                    //attempt to convert the split message into values for the bullet
                    Vector3 bulletPosition = new Vector3(float.Parse(splitMessage[1]), float.Parse(splitMessage[2]), 0);

                    float bulletSpeed = float.Parse(splitMessage[3]);
                    float bulletDirection = float.Parse(splitMessage[4]);

                    Bullet b = GameManager.Instance.objectPool.GetObjectFromPool("Bullet").GetComponent<Bullet>();
                    b.transform.position = bulletPosition;
                    b.SetStats(bulletSpeed, bulletDirection);
                }
                catch(Exception e)
                {
                    Global.LogReport(e.ToString());
                }
            }
        }
    }
}
