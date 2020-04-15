// ================================================================================================================================
// File:        ProjectileMovement.cs
// Description:	Moves a projectile around the level
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private bool Initialised = false;   //The projectile waits until its been told in which direction to travel before applying movement
    private Vector3 MovementDirection;  //Direction the projectile has been told to travel
    private float MoveSpeed = 8f;   //How fast this projectile travels

    //Called by whatever entity spawned this projectile into the level, to provide it with its direction of travel and optional movement speed override
    public void InitializeProjectile(Vector3 MovementDirection, float MoveSpeed = 8f)
    {
        //Set initialised flag and save values provided
        Initialised = true;
        this.MovementDirection = MovementDirection.normalized;
        this.MoveSpeed = MoveSpeed;
    }

    private void Update()
    {
        //Apply movement once initialised
        if(Initialised)
        {
            //Travel forward in current direction
            transform.position += MovementDirection * MoveSpeed * Time.deltaTime;

            //Self-destruct if the projectile goes outside the screen borders
            if (!ScreenBounds.IsPosInside(transform.position))
                Destroy(gameObject);
        }
    }
}
