//Matthew Watson
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A class to store useful methods that many objects need to use.
/// </summary>
public static class Global
{

    //Ticks upward each frame, 60 times per second.
    //GameManager resets it to zero whenever the game scene is loaded.
    public static int gameTime;

    /// <summary>
    /// Calls Debug.Log if the game is running in the editor.
    /// </summary>
    /// <param name="report">The report to log.</param>
    public static void LogReport(string report)
    {
        if (Application.isEditor)
        {
            Debug.Log(report);
        }
    }

    #region Movement and Rotation

    /// <summary>
    /// Moves a transform a certain distance.
    /// </summary>
    /// <param name="t">The transform to move</param>
    /// <param name="distance">The distance to move t</param>
    public static void MoveObject(Transform t, Vector3 distance)
    {
        //Divide the distance by this number
        const float ADJUSTMENT = 50f;

        Vector3 pos = t.localPosition;

        //add the proper distance
        pos += distance / ADJUSTMENT;

        //update position
        t.localPosition = pos;
    }

    public static void MoveObject(Transform t, float direction, float magnitude)
    {
        //Divide the distance by this number
        const float ADJUSTMENT = 50f;

        Vector3 temp = new Vector3();

        //moves the object based on trigonometry
        direction *= Mathf.Deg2Rad;

        temp.x = Mathf.Cos(direction) * magnitude;
        temp.y = Mathf.Sin(direction) * magnitude;

        temp /= ADJUSTMENT;

        t.localPosition += temp;
    }

    /// <summary>
    /// Returns the euler angle pointing towards the target position, from the origin position.
    /// </summary>
    /// <param name="origin">The position to point from.</param>
    /// <param name="target">The position to point towards.</param>
    /// <returns></returns>
    public static float PointTowards(Vector3 origin, Vector3 target)
    {
        if (origin != null && target != null)
        {
            float originX = origin.x;
            float originY = origin.y;

            float targetX = target.x;
            float targetY = target.y;

            //calculate direction
            if (targetX != originX)
            {
                //if targetX is less than origin, add 180 to correct the result.
                if (targetX > originX)
                {
                    return 0 + (180 / Mathf.PI) * Mathf.Atan((targetY - originY) / (targetX - originX));
                }
                else
                {
                    return 180 + (180 / Mathf.PI) * Mathf.Atan((originY - targetY) / (originX - targetX));
                }
            }
            else
            {
                //If the x values are identical, determine direction based on y relation
                if (targetY > originY)
                {
                    return 90;
                }
                else
                {
                    return 270;
                }
            }
        }
        else
        {
            //If either object is null, return 0
            return 0;
        }
    }
    #endregion

    #region Bullet Pattern Storage
    public static Dictionary<string, List<Action>> patternDictionary;

    public static List<Sprite> bulletSprites;
    public static List<float> bulletRadii;
    #endregion
}

