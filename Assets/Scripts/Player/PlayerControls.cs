// ================================================================================================================================
// File:        PlayerControls.cs
// Description:	Allows the player to fly their ship around and fire projectiles
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    //Rotation
    private float CurrentRotation = 0f;   //Rotation on Z axis to turn the ship around
    private float TurnSpeed = 200f; //How fast the ship turns

    //Movement
    private Vector3 CurrentVelocity = new Vector3(0f, 0f, 0f);  //Current direction of movement
    private Vector2 VelocityRange = new Vector2(-8.5f, 8.5f);   //Min/Max velocity limits
    private float ThrusterPower = 8f;   //Strength of the ships forward thrusters
    private float FrictionStrength = 1.25f;  //Applied to slowly bring the ship to a stop

    //Shooting
    public GameObject ProjectilePrefab; //Prefab which the player shoots
    private float ProjectileSpawnDistance = 0.5f;  //How far in front of the ship to spawn projectiles

    //Thruster Sound/Animation
    public AudioSource SoundPlayer;
    private bool PlayingThrusterSound = false;
    public SpriteRenderer ThrusterRenderer;

    //Hyperspace Teleporting
    private bool InTeleport = false;    //Is the ship currently in the middle of teleporting
    private float TeleportDuration = 1f;    //How long it takes to teleport
    private float TeleportLeft; //Time until the teleport finishes
    private Vector2 XPosRange = new Vector2(-7.5f, 7.5f);   //Where the ship may teleport on the x axis
    private Vector2 YPosRange = new Vector2(-4.5f, 4.5f);   //Where the ship may teleport on the y axis

    //Death Animation
    public Animator AnimationController;    //Used to trigger playback of the death animation
    public SpriteRenderer Renderer; //Used to hide the ship from view after the death animation finishes, before they respawn
    private bool IsDead = false;    //Tracks when the players ship is dead
    private float DeathAnimLeft = 0.333f;   //How long until the death animation finishes playing

    private void Start()
    {
        //Turn off thruster rendering
        ThrusterRenderer.forceRenderingOff = true;
        //Get the initial rotation value
        CurrentRotation = transform.rotation.z;
        //Start and immediately pause the thruster sound
        SoundPlayer.Play();
        SoundPlayer.Pause();
    }

    private void Update()
    {
        //Do nothing while the game is paused
        if (GameState.Instance.GamePaused)
            return;

        //Allow control while alive
        if (!IsDead)
            ControlShip();
        //Wait for death animation to finish if dead
        else
            PlayDeath();
    }

    //Allows full control of the players ship
    private void ControlShip()
    {
        //Process all actions giving control of the ship to the player
        if (!InTeleport)
        {
            MoveShip();
            SteerShip();
            FireProjectiles();
            StartTeleport();
        }
        else
            FinishTeleport();
    }

    //Fires a projectile forwards whenever the player presses spacebar
    private void FireProjectiles()
    {
        if(Input.GetButtonDown("Shoot"))
        {
            //Spawn a new projectile and give it its movement values
            Vector3 SpawnPos = transform.position + transform.up * ProjectileSpawnDistance;
            GameObject Projectile = Instantiate(ProjectilePrefab, SpawnPos, Quaternion.identity);
            Projectile.GetComponent<ProjectileMovement>().InitializeProjectile(transform.up, 8f + CurrentVelocity.magnitude);
            SoundEffectsPlayer.Instance.PlaySound("PlayerShoot", 1f);
        }
    }

    //Controls the thrusters and applys movement to the ship
    private void MoveShip()
    {
        //Get new velocity vector after applying user input
        if (Input.GetButton("Thrust"))
            CurrentVelocity += transform.up * ThrusterPower * Time.deltaTime;

        //Start looping the thruster sound effect if we arent
        if(Input.GetButton("Thrust") && !PlayingThrusterSound)
        {
            ThrusterRenderer.forceRenderingOff = false;
            PlayingThrusterSound = true;
            SoundPlayer.UnPause();
        }
        //Otherwise stop looping if it shouldnt be anymore
        else if(!Input.GetButton("Thrust") && PlayingThrusterSound)
        {
            ThrusterRenderer.forceRenderingOff = true;
            PlayingThrusterSound = false;
            SoundPlayer.Pause();
        }

        //Clamp velocity magnitude
        if (CurrentVelocity.magnitude < 0.0f)
            CurrentVelocity = Vector3.ClampMagnitude(CurrentVelocity, VelocityRange.x);
        else if (CurrentVelocity.magnitude > 0.0f)
            CurrentVelocity = Vector3.ClampMagnitude(CurrentVelocity, VelocityRange.y);

        //Apply friction
        if (CurrentVelocity.magnitude > 0.0f)
            CurrentVelocity -= CurrentVelocity.normalized * FrictionStrength * Time.deltaTime;
        if (CurrentVelocity.magnitude < 0.0f)
            CurrentVelocity += CurrentVelocity.normalized * FrictionStrength * Time.deltaTime;

        //Get new location value for the ship, kept inside the screen borders
        Vector3 NewPos = transform.position + CurrentVelocity * Time.deltaTime;
        NewPos = ScreenBounds.WrapPosInside(NewPos);

        //Move to the new location
        transform.position = NewPos;
    }

    //Steers the players ship
    private void SteerShip()
    {
        float Rotation = Input.GetAxis("Rotate");
        if (Rotation != 0f)
            CurrentRotation -= Rotation * TurnSpeed * Time.deltaTime;

        //Clamp rotation value
        if (CurrentRotation >= 360f)
            CurrentRotation -= 360f;
        if (CurrentRotation <= -360f)
            CurrentRotation += 360f;

        //Create a new quaternion to give the player
        Quaternion NewRotation = Quaternion.Euler(0f, 0f, CurrentRotation);
        transform.rotation = NewRotation;
    }

    //Allows the player to start the teleportation process by pressing the enter key
    private void StartTeleport()
    {
        if(Input.GetButtonDown("Teleport"))
        {
            //Start the teleport timer and move the ship away out of sight
            InTeleport = true;
            TeleportLeft = TeleportDuration;
            transform.position = new Vector3(1000, 1000, 0);
            CurrentVelocity = new Vector3(0f, 0f, 0f);
            SoundEffectsPlayer.Instance.PlaySound("TeleportAway");
        }
    }

    //Returns the ship back to the playing field once the teleport has completed
    private void FinishTeleport()
    {
        //Wait until the timer finishes before bringing the ship back into play
        TeleportLeft -= Time.deltaTime;
        if(TeleportLeft <= 0.0f)
        {
            //Place the ship back into the playing field at a random location somewhere
            transform.position = new Vector3(Random.Range(XPosRange.x, XPosRange.y), Random.Range(YPosRange.x, YPosRange.y), 0f);
            InTeleport = false;
            SoundEffectsPlayer.Instance.PlaySound("TeleportBack");
        }
    }

    //Plays out the death animation before destroying itself and asking the GameState class to respawn or go to gameover
    private void PlayDeath()
    {
        //Wait until the animation finishes playing
        DeathAnimLeft -= Time.deltaTime;
        if(DeathAnimLeft <= 0.0f)
        {
            //Tell the GameState class the player has died, then destroy it
            GameState.Instance.PlayerDead();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Collisions with Asteroids, Saucers, or Enemy Projectiles kills the player
        if(collision.transform.CompareTag("Asteroid") || collision.transform.CompareTag("Saucer") || collision.transform.CompareTag("SaucerProjectile"))
        {
            //Start the death animation and play the sound effect
            IsDead = true;
            AnimationController.SetTrigger("Death");
            SoundEffectsPlayer.Instance.PlaySound("PlayerDie");
        }
    }
}
