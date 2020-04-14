// ================================================================================================================================
// File:        PlayerProjectileMovement.cs
// Description:	Moves the players projectile
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerProjectileMovement : MonoBehaviour
{
    private bool Initialised = false;   //Waits until values movement values have been provided by the player
    private Vector3 MovementDirection;  //Direction this projectile should be travelling
    private float MoveSpeed = 8f;  //How fast this projectile travels
    private float LifetimeLeft = 8f;    //Seconds remaining until the projectile destroys itself

    //Called from the PlayerControls when it spawns the projectile into the game
    public void InitializeProjectile(Vector3 MovementDirection, Vector3 ShipVelocity)
    {
        Initialised = true;
        this.MovementDirection = MovementDirection;
        MoveSpeed += ShipVelocity.magnitude;
    }

    private void Update()
    {
        //Keep the projectile moving once its been initialised
        if (Initialised)
        {
            //Move the projectile forward
            Travel();

            //Destroy the projectile when its lifetime expires
            LifetimeLeft -= Time.deltaTime;
            if (LifetimeLeft <= 0.0f)
                Destroy(gameObject);
        }
    }

    //Keeps the projectile moving forward
    private void Travel()
    {
        //Create a new movement vector to apply to the projectile
        Vector3 MovementVector = MovementDirection.normalized * MoveSpeed;

        //Figure out a new location for the projectile based on this movement vector
        Vector3 NewPos = transform.position + MovementVector * Time.deltaTime;
        NewPos = ScreenBounds.KeepPosInside(NewPos);

        //Move the projectile to its new location
        transform.position = NewPos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Projectiles destroy themself if they come into contact with the player character, or another one of themselves
        if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("PlayerProjectile"))
            Destroy(gameObject);
    }
}
