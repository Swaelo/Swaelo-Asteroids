// ================================================================================================================================
// File:        SmallSaucerAI.cs
// Description:	Small Saucers track the player around, firing projectiles slighty in front or slightly behind them.  They're accuracy
//              increases as the players score counter gets higher.
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class SmallSaucerAI : MonoBehaviour
{
    //Movement
    private float MoveSpeed = 0.75f;    //Saucers current movement speed
    private Vector3 TargetPos;  //Current seeking location nearby the player
    private float NextTargetUpdate = 0f;    //How long until a new target location is acquired
    private Vector2 TargetUpdateInterval = new Vector2(0.5f, 2.5f); //New target location is acquired periodically
    private Vector2 TargetOffsetRange = new Vector2(0.65f, 2.3f);  //Distance target position is offset from players location
    private float TargetUpdateDistance = 0.15f; //How close the saucer must be from its current target to force a new target

    //Firing
    public GameObject SaucerProjectilePrefab;   //Projectiles fired by the saucer
    private Vector2 FiringCooldownRange = new Vector2(1.5f, 5f);    //How long the saucer must wait between shots
    private float FiringCooldownLeft = 2.5f;    //How long until the saucer can fire again
    private float ProjectileSpawnDistance = 0.5f;   //How far away from the saucer to spawn in its projectiles
    private Vector2 AimOffsetRange = new Vector2(0.85f, 2.5f); //How far away from the player to aim the projectiles

    //Death
    public Animator AnimationController;    //Used to trigger playback of the death animation
    private bool IsDead = false;    //Tracks when the saucer is dead
    private float DeathAnimLeft = 0.333f;   //How long until the death animation finishes playing out

    private void Start()
    {
        TargetPos = GetOffsetPlayerPos(TargetOffsetRange);
    }

    private void Update()
    {
        //Do nothing while the game is paused
        if (GameState.Instance.GamePaused)
            return;

        //Perform normal behaviours while both the saucer and player are alive
        if(!IsDead && GameState.Instance.PlayerShip != null)
        {
            UpdateTarget();
            SeekPlayer();
            FireProjectiles();
        }
        //Otherwise playout death animation before the saucer destroys itself
        else
        {
            DeathAnimLeft -= Time.deltaTime;
            if (DeathAnimLeft <= 0.0f)
                Destroy(gameObject);
        }
    }

    //Periodically updates the target location offset from players location
    private void UpdateTarget()
    {
        //Update the target position whenever the timer expires, or we have arrived at the current target location
        float TargetDistance = Vector3.Distance(transform.position, TargetPos);
        NextTargetUpdate -= Time.deltaTime;
        if(TargetDistance <= TargetUpdateDistance || NextTargetUpdate <= 0.0f)
        {
            //Reset the timer and grab a new target location
            NextTargetUpdate = Random.Range(TargetUpdateInterval.x, TargetUpdateInterval.y);
            TargetPos = GetOffsetPlayerPos(TargetOffsetRange);
        }
    }

    //Seeks positions nearby to the players position
    private void SeekPlayer()
    {
        Vector3 TargetDirection = Vector3.Normalize(TargetPos - transform.position);
        Vector3 NewPos = transform.position + TargetDirection * MoveSpeed * Time.deltaTime;
        NewPos = ScreenBounds.WrapPosInside(NewPos);
        transform.position = NewPos;
    }

    //Fires projectiles nearby the players locaiton
    private void FireProjectiles()
    {
        //Fire a new projectile whenever the timer expires
        FiringCooldownLeft -= Time.deltaTime;
        if(FiringCooldownLeft <= 0.0f)
        {
            //Reset the timer
            FiringCooldownLeft = Random.Range(FiringCooldownRange.x, FiringCooldownRange.y);
            //Find a spot to aim the projectile, get the direction to that location, and find a location to spawn the projectile in at
            Vector3 ShotTarget = GetOffsetPlayerPos(AimOffsetRange);
            Vector3 ShotDirection = Vector3.Normalize(ShotTarget - transform.position);
            Vector3 SpawnPos = transform.position + ShotDirection * ProjectileSpawnDistance;
            //Play the firing sound effect, then spawn in the new projectile and tell it which way to go
            SoundEffectsPlayer.Instance.PlaySound("EnemyShoot");
            GameObject ProjectileSpawn = Instantiate(SaucerProjectilePrefab, SpawnPos, Quaternion.identity);
            ProjectileSpawn.GetComponent<ProjectileMovement>().InitializeProjectile(ShotDirection);
        }
    }

    //Returns a position offset from the players current location, within the range of the given vector values
    private Vector3 GetOffsetPlayerPos(Vector2 OffsetRange)
    {
        //Get 2 random values within the given offset range to offset from the player pos in each direction
        float XOffset = Random.Range(OffsetRange.x, OffsetRange.y);
        float YOffset = Random.Range(OffsetRange.x, OffsetRange.y);

        //Flip a coin to decide which direction these offsets will be applied
        bool PositiveXOffset = Random.value >= 0.5f;
        bool PositiveYOffset = Random.value >= 0.5f;

        //Apply these offsets to the players current location to get the new offset position
        Vector3 OffsetPos = GameState.Instance.PlayerShip.transform.position;
        OffsetPos.x += PositiveXOffset ? XOffset : -XOffset;
        OffsetPos.y += PositiveYOffset ? YOffset : -YOffset;
        OffsetPos = ScreenBounds.ClampPosInside(OffsetPos);
        return OffsetPos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Saucers die if they collide with any asteroids
        if(collision.transform.CompareTag("Asteroid"))
        {
            AnimationController.SetTrigger("Death");
            IsDead = true;
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<PolygonCollider2D>());
            SoundEffectsPlayer.Instance.PlaySound("EnemyDie");
            GameState.Instance.SaucerDestroyed();
        }
    }
}
