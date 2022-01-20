//Matthew Watson

//Camera Assembly

//FOR EDITING, NOT GAMEPLAY
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAssemblyController : MonoBehaviour, iTickable
{
    [Tooltip("X = Movement speed\nY = Rotation speed")]
    /// <summary>
    /// Movement, rotation.
    /// </summary>
    public Vector2 speeds;

    private void Start()
    {
        GameManager.Instance.AddTicker(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance.RemoveTicker(this);
    }
    public void Tick()
    {
        #region Get Inputs
        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),

            Input.GetKey(KeyCode.Q),
            Input.GetKey(KeyCode.E)
        };

        #endregion
        
        #region Handle Movement

        Vector3 movement = Vector3.zero;
        float rotation = 0;

        //up, down, left, right
        if (inputs[0]) movement.y--;
        if (inputs[1]) movement.y++;
        if (inputs[2]) movement.x--;
        if (inputs[3]) movement.x++;

        movement.Normalize();
        float direction = Mathf.Rad2Deg * Mathf.Atan2(movement.y, movement.x);
        //Global.LogReport("direction " + direction + "\nEuler z " + transform.rotation.eulerAngles.z);

        //Set up rotation
        if (inputs[4]) rotation += speeds.y;
        if (inputs[5]) rotation -= speeds.y;

        //Move and rotate the camera assembly accordingly
        Global.MoveObject(transform, transform.rotation.eulerAngles.z - direction, movement.magnitude * speeds.x);
        transform.Rotate(new Vector3(0, 0, rotation));

        #endregion
        
    }
}
