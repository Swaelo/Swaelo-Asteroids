// ================================================================================================================================
// File:        LargeSaucerAI.cs
// Description:	Large saucers wander around randomly, firing shots in completely random direction
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class LargeSaucerAI : MonoBehaviour
{
    //Movement
    private float MoveSpeed = 1.25f;    //How fast the saucer moves around
    private Vector3 TargetPos;  //Current location the saucer is moving towards
    private Vector2 TargetUpdateInterval = new Vector2(1.5f, 5f);   //How often the saucer forces a new target selection if it cant reach it
    private float NextTargetUpdate = 3.5f;  //Time left until the saucer selects a new target to move toward
    private float TargetUpdateDistance = 0.25f; //How close the saucer must be from its target location to force it to get a new target

    //Firing
    public GameObject SaucerProjectilePrefab;   //Projectiles fired by the saucer
    private Vector2 FiringCooldownRange = new Vector2(2.15f, 6.5f); //How long the saucer must wait between firing shots
    private float FiringCooldownLeft = 2.5f;    //Seconds left until the saucer can fire another projectile
    private float ProjectileSpawnDistance = 0.75f;  //How far away from the saucer to spawn in its projectiles

    //Death
    public Animator AnimationController;
    private bool IsDead = false;
    private float DeathAnimLeft = 0.333f;

    private void Start()
    {
        //Get a random target location to wander towards
        TargetPos = ScreenBounds.GetInsidePos();
    }

    private void Update()
    {
        //Do nothing while the game is paused
        if (GameState.Instance.GamePaused)
            return;

        if(!IsDead && GameState.Instance.PlayerShip != null)
        {
            //Update target location periodically
            UpdateTarget();

            //Seek towards current target
            SeekTarget();

            //Fire random projectiles around
            FireProjectiles();
        }
        else
        {
            DeathAnimLeft -= Time.deltaTime;
            if (DeathAnimLeft <= 0.0f)
                Destroy(gameObject);
        }
    }

    //Causes a new target to be acquired when the current target has been reached, or the timer has expired
    private void UpdateTarget()
    {
        //Get a new target if we are close enough to the current target
        float TargetDistance = Vector3.Distance(transform.position, TargetPos);
        if(TargetDistance <= TargetUpdateDistance)
        {
            TargetPos = ScreenBounds.GetInsidePos();
            NextTargetUpdate = Random.Range(TargetUpdateInterval.x, TargetUpdateInterval.y);
        }

        //Get a new target if the update timer expires
        NextTargetUpdate -= Time.deltaTime;
        if(NextTargetUpdate <= 0.0f)
        {
            TargetPos = ScreenBounds.GetInsidePos();
            NextTargetUpdate = Random.Range(TargetUpdateInterval.x, TargetUpdateInterval.y);
        }
    }

    //Moves towards the current target location
    private void SeekTarget()
    {
        Vector3 TargetDirection = Vector3.Normalize(TargetPos - transform.position);
        Vector3 NewPos = transform.position + TargetDirection * MoveSpeed * Time.deltaTime;
        NewPos = ScreenBounds.WrapPosInside(NewPos);
        transform.position = NewPos;
    }

    //Periodically fires projectiles in random direction
    private void FireProjectiles()
    {
        //Fire a new projectile whenever the timer expires
        FiringCooldownLeft -= Time.deltaTime;
        if(FiringCooldownLeft <= 0.0f)
        {
            //Reset the firing cooldown timer
            FiringCooldownLeft = Random.Range(FiringCooldownRange.x, FiringCooldownRange.y);

            //Get a random direction to fire the projectile, and find the position where it should be spawned
            Vector3 FireDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
            Vector3 SpawnPos = transform.position + FireDirection * ProjectileSpawnDistance;

            //Spawn in the new projectile and give it its movement direction
            GameObject ProjectileSpawn = Instantiate(SaucerProjectilePrefab, SpawnPos, Quaternion.identity);
            ProjectileSpawn.GetComponent<ProjectileMovement>().InitializeProjectile(FireDirection);

            //Play the firing sound effect
            SoundEffectsPlayer.Instance.PlaySound("EnemyShoot");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
