using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : NotifiableObj, iTickable
{
    /// <summary>
    /// If it's in the object pool, activeInScene will be false. If it's out in the scene, it'll be true.
    /// </summary>
    private bool activeInScene;

    #region Initialize and DeInitialize
    protected override void Initialize()
    {

        GameManager.Instance.AddTicker(this);
    }

    protected override void DeInitialize()
    {
        GameManager.Instance.RemoveTicker(this);
    }
    #endregion

    #region Update
    public void Tick()
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
