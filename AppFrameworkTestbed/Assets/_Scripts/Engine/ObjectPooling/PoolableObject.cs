using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : ManagedObject
{
    #region Active in scene?
    /// <summary>
    /// If it's in the object pool, activeInScene will be false. If it's out in the scene, it'll be true.
    /// </summary>
    private bool activeInScene;

    public void SetActiveInScene(bool isActive)
    {
        activeInScene = isActive;

        if (activeInScene)
        {
            OnActivate();
        }
        else
        {
            OnDeactivate();
        }
    }

    public bool GetActiveInScene()
    {
        return activeInScene;
    }

    /// <summary>
    /// What the object does when it's brought out from the pool.
    /// </summary>
    protected virtual void OnActivate()
    {

    }

    /// <summary>
    /// What the object does when it's enqueued back into the pool.
    /// </summary>
    protected virtual void OnDeactivate()
    {

    }
    #endregion

    #region Initialize
    protected override void Initialize()
    {
        //When a poolable object is spawned, exclude its update from the GameManager
        excludeTick = true;
    }
    #endregion

    #region Update
    public override void Tick()
    {

    }
    #endregion

    public override void OnNotify(Category category, string message, string senderData)
    {
        
    }
    public override string GetLoggingData()
    {
        return name;
    }
}
