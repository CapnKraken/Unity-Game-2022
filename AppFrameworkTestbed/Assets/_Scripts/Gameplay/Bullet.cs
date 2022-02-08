using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PoolableObject
{

    [SerializeField] private float speed;
    [SerializeField] private float direction;

    /// <summary>
    /// The bullet's collision circle
    /// </summary>
    private HitCircle hitCircle;

    //The bullet's sprite renderer
    private SpriteRenderer spriteRenderer;

    public void SetStats(float speed, float direction)
    {
        this.speed = speed;
        this.direction = direction;
    }
    public void SetType(int type)
    {
        hitCircle.radius = Global.bulletRadii[type];
        spriteRenderer.sprite = Global.bulletSprites[type];
    }

    #region Initialize
    protected override void Initialize()
    {
        //Bullets exist relative to the camera position
        transform.parent = GameManager.Instance.screenParent;

        hitCircle = GetComponent<HitCircle>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    #endregion

    //Empty
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
        #region Move
        Global.MoveObject(transform, direction, speed);

        Vector3 pos = transform.localPosition;
        bool outBounds = false;
        if (pos.x > upperBound.x || pos.x < lowerBound.x) outBounds = true;
        if (pos.y > upperBound.y || pos.y < lowerBound.y) outBounds = true;

        if (outBounds)
        {
            GameManager.Instance.objectPool.ReturnObjectToPool(this.tag, this);
        }
        #endregion

        #region Player Collision
        if (hitCircle.isTouching(GameManager.Instance.player.hitbox))
        {
            Notify(Category.GENERAL, "PlayerHit");
        }
        #endregion
    }

    public override void OnNotify(Category category, string message, string senderData)
    {
        if(message == "PlayerHit")
        {
            GameManager.Instance.objectPool.ReturnObjectToPool(this.tag, this);
        }
    }

    public override string GetLoggingData()
    {
        //Logging data includes the gameobject name, its speed, and its direction.
        return $"Bullet {name} {{Speed: {speed}, Direction: {direction}}}";
    }
}
