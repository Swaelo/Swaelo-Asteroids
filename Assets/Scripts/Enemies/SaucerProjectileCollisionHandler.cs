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
        //Enemy projectiles destroy themselves if they come into contact with another enemy, or another one of themselves, or any of the asteroids
        if (collision.transform.CompareTag("Saucer") || collision.transform.CompareTag("SaucerProjectile") || collision.transform.CompareTag("Asteroid"))
            Destroy(gameObject);
    }
}
