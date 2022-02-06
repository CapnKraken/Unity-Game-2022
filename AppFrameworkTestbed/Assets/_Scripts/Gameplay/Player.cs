//Matthew Watson

//Player object

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Player : ManagedObject
{
    /// <summary>
    /// How fast the player moves when unfocused and focused, respectively.
    /// </summary>
    public Vector2 speeds;

    /// <summary>
    /// The player's collision area
    /// </summary>
    public HitCircle hitbox;

    #region Initialize
    protected override void Initialize()
    {
        hitbox = GetComponent<HitCircle>();
    }
    #endregion

    #region Update
    public override void Tick()
    {
        #region Get Inputs
        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.UpArrow),
            Input.GetKey(KeyCode.DownArrow),
            Input.GetKey(KeyCode.LeftArrow),
            Input.GetKey(KeyCode.RightArrow),

            Input.GetKey(KeyCode.LeftShift),

            Input.GetKey(KeyCode.Escape)
        };

        //Temporary- if user presses escape, application quits.
        if(inputs[5] == true)
        {
            Application.Quit();
        }
        #endregion

        #region Handle Movement
        Vector3 movement = Vector3.zero;

        //up, down, left, right
        if (inputs[0]) movement.y++;
        if (inputs[1]) movement.y--;
        if (inputs[2]) movement.x--;
        if (inputs[3]) movement.x++;

        //Normalize movement diagonals
        movement.Normalize();

        //4 is focus
        if (inputs[4])
        {
            //apply focused movement
            movement *= speeds.y;
        }
        else
        {
            //apply unfocused movement
            movement *= speeds.x;
        }

        Global.MoveObject(transform, movement);
        #endregion

        #region Test for Enemy Hit
        if(GameManager.Instance.TestForHit(hitbox, "Enemy"))
        {
            Notify(Category.GENERAL, "PlayerHit");
        }
        #endregion
    }
    #endregion
    public override void OnNotify(Category category, string message, string senderData)
    {
        //Send a message to play the audio sound
        Notify(Category.Audio, "PlaySound PlayerHit");

        //respawn at 0, 0
        transform.localPosition = new Vector3(0, 0, 0);
    }

    public override string GetLoggingData()
    {
        return "Player";
    }
}
