// ================================================================================================================================
// File:        PlayerProjectileCollisionHandler.cs
// Description:	Handles collisions between the players projectiles and other objects in the world
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerProjectileCollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //The projectiles destroy themselves if they come into contact with the player character, or another one of themselves
        if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("PlayerProjectile"))
            Destroy(gameObject);
        //Asteroids are destroyed on contact
        else if(collision.transform.CompareTag("Asteroid"))
        {
            //Have the manager class destroy the asteroid, then destroy this projectile
            AsteroidManager.Instance.DestroyAsteroid(collision.gameObject);
            Destroy(gameObject);
        }
        //Saucer enemies are killed on contact
        else if(collision.transform.CompareTag("Saucer"))
        {
            //Play the killing sound effect, award points for the kill and destroy the saucer and projectile
            SoundEffectsPlayer.Instance.PlaySound("EnemyDie");
            SaucerSizes Size = collision.transform.GetComponent<SaucerSize>().MySize;
            GameState.Instance.IncreaseScore((int)(Size == SaucerSizes.Small ? ScoreValues.SmallSaucer : ScoreValues.LargeSaucer));
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
