//Matthew Watson

/// <summary>
/// Interface to implement the Tick update method. Tick tock. <br/>
/// Make sure to add/remove the object to the list in gamemanager.
/// </summary>
public interface iTickable
{
    /// <summary>
    /// To replace Update in objects that implement iTickable.
    /// </summary>
    public void Tick();
}
