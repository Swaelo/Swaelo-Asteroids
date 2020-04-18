// ================================================================================================================================
// File:        SaucerProjectileCollisionHandler.cs
// Description:	Handles collision events between enemy projectiles and other objects in the world
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class SaucerProjectileCollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Enemy projectiles destroy themselves if they come into contact with another enemy, or another one of themselves
        if (collision.transform.CompareTag("Saucer") || collision.transform.CompareTag("SaucerProjectile"))
            Destroy(gameObject);
        //Enemies shots can destroy the asteroids just like the player can
        else if(collision.transform.CompareTag("Asteroid"))
        {
            AsteroidManager.Instance.DestroyAsteroid(collision.gameObject);
            Destroy(gameObject);
        }
    }
}