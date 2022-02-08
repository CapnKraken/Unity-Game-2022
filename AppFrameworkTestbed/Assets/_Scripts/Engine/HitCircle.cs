//Matthew Watson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A deterministic alternative to unity's circle collider.
/// </summary>
public class HitCircle : MonoBehaviour
{
    //The radius of the collision area
    public float radius;

    //Offset from the original transform
    public Vector3 offset;

    #region Start and Destroy
    private void Start()
    {
        GameManager.Instance.AddHitCircle(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance.RemoveHitCircle(this);
    }
    #endregion

    /// <summary>
    /// Determines whether the HitCircle is intersecting with another specified HitCircle.
    /// </summary>
    /// <param name="other">The HitCircle to test collision with</param>
    /// <returns>True: The HitCircles are colliding. <br/>False: they are not.</returns>
    public bool isTouching(HitCircle other)
    {
        //get the opposite and adjacent sides for our triangle
        Vector3 difference = (other.transform.position + other.GetRotatedOffset()) - (transform.position + GetRotatedOffset());
        float hyp = this.radius + other.radius;

        //use pythagorean theorem to determine whether or not the two circles are touching
        bool result = hyp * hyp > (difference.x * difference.x + difference.y * difference.y);

        return result;
    }

    /// <summary>
    /// Get the correct offset for the HitCircle depending on the GameObject's rotation
    /// </summary>
    /// <returns>A Vector3 (with z value of 0) which represents the corrected offset from the origin.</returns>
    public Vector3 GetRotatedOffset()
    {
        //Obtain the object's rotation and convert it to radians.
        float rotation = transform.rotation.eulerAngles.z;
        rotation *= Mathf.Deg2Rad;

        //Initialize the new offset variable
        Vector3 rotatedOffset = Vector3.zero;

        //Store the cos and sin of rotation so we only have to perform each operation once per call
        float cosRotation = Mathf.Cos(rotation);
        float sinRotation = Mathf.Sin(rotation);

        //Determine the new offset
        rotatedOffset.x = cosRotation * offset.x - sinRotation * offset.y;
        rotatedOffset.y = sinRotation * offset.x + cosRotation * offset.y;

        return rotatedOffset;
    }

    //Draw a visual for the HitCircle in the editor.
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + GetRotatedOffset(), radius);
    }
}
