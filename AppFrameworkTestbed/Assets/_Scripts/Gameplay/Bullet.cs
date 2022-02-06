using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PoolableObject
{

    [SerializeField] private float speed;
    [SerializeField] private float direction;

    public void SetStats(float speed, float direction)
    {
        this.speed = speed;
        this.direction = direction;
    }

    #region Initialize
    protected override void Initialize()
    {
    }
    #endregion

    #region Activate and Deactivate from pool
    protected override void OnActivate()
    {
        
    }

    protected override void OnDeactivate()
    {
        
    }
    #endregion

    /// <summary>
    /// The bounds of the bullet.
    /// </summary>
    public Vector2 upperBound, lowerBound;
    public override void Tick()
    {
        Global.MoveObject(transform, direction, speed);

        Vector3 pos = transform.position;
        bool outBounds = false;
        if (pos.x > upperBound.x || pos.x < lowerBound.x) outBounds = true;
        if (pos.y > upperBound.y || pos.y < lowerBound.y) outBounds = true;

        if (outBounds)
        {
            GameManager.Instance.objectPool.ReturnObjectToPool(this.tag, this);
        }
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
