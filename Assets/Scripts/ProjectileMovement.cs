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
    private float TravelQuota = 1.25f; //How long before the projectiles lifetime expires

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
        //Do nothing while the game is paused
        if (GameState.Instance.GamePaused)
            return;

        //Apply movement once initialised
        if (Initialised)
        {
            //Travel forward in current direction
            Vector3 NewPos = transform.position + MovementDirection * MoveSpeed * Time.deltaTime;
            NewPos = ScreenBounds.WrapPosInside(NewPos);
            transform.position = NewPos;
        }

        //Auto-destroy projectile after maximum lifetime expires
        TravelQuota -= Time.deltaTime;
        if (TravelQuota <= 0.0f)
            Destroy(gameObject);
    }
}
