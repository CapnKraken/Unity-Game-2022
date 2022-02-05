using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NotifiableObj, iTickable
{

    [SerializeField] private float speed {get;set;}
    [SerializeField] private float direction {get;set;}



    protected override void Initialize()
    {

    }
    public void Tick()
    {
        Global.MoveObject(transform, direction, speed);
    }

    public override void OnNotify(Category category, string message, string senderData)
    {
        
    }

    public override string GetLoggingData()
    {
        //Logging data includes the gameobject name, its speed, and its direction.
        return $"Bullet {name} {{Speed: {speed}, Direction: {direction}}}";
    }
}
