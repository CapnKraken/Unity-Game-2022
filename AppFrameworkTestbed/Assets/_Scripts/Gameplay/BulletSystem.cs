using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Handles the spawning of enemy bullets.
/// </summary>
public class BulletSystem : ManagedObject
{
    [System.Serializable]
    public struct SpriteRadius
    {
        public Sprite sprite;
        public float radius;
    }

    public List<SpriteRadius> bulletData;
    protected override void Initialize()
    {
        //BulletSystem responds to shooting events
        relevantCategories.Add(Category.Shooting);

        Global.bulletSprites = new List<Sprite>();
        Global.bulletRadii = new List<float>();
        foreach(SpriteRadius s in bulletData)
        {
            Global.bulletSprites.Add(s.sprite);
            Global.bulletRadii.Add(s.radius);
        }
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
                //Global.LogReport("Shooting, requested by " + senderData);
                Vector3 bulletPosition = new Vector3(float.Parse(splitMessage[1]), float.Parse(splitMessage[2]), 0);

                float bulletSpeed = float.Parse(splitMessage[3]);
                float bulletDirection = float.Parse(splitMessage[4]);

                int bulletType = int.Parse(splitMessage[5]);

                PoolableObject p = GameManager.Instance.objectPool.GetObjectFromPool("Bullet");

                if (p != null)
                {
                    Bullet b = p.GetComponent<Bullet>();
                    b.transform.position = bulletPosition;
                    b.SetStats(bulletSpeed, bulletDirection);
                    b.SetType(bulletType);
                }
                try
                {
                    //sample notification
                    //Shoot 50 50 10 90
                    //Shoot at position 50, 50. Speed 10, direction: 90.

                    //attempt to convert the split message into values for the bullet
                   
                }
                catch(Exception e)
                {
                    Global.LogReport(e.ToString());
                }
            }
        }
    }
}
