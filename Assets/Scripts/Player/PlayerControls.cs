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
    private float ForwardThrusterPower = 8f;   //Strength of the ships forward thrusters
    private float ReverseThrusterPower = -3.5f;  //Strength of the ships reverse thrusters
    private float FrictionStrength = 1.25f;  //Applied to slowly bring the ship to a stop

    //Shooting
    public GameObject ProjectilePrefab; //Prefab which the player shoots
    private float ProjectileSpawnDistance = 0.5f;  //How far in front of the ship to spawn projectiles

    //Thruster Sound
    public AudioSource SoundPlayer;
    private bool PlayingThrusterSound = false;

    private void Start()
    {
        //Get the initial rotation value
        CurrentRotation = transform.rotation.z;
        //Start and immediately pause the thruster sound
        SoundPlayer.Play();
        SoundPlayer.Pause();
    }

    private void Update()
    {
        //Process all actions giving control of the ship to the player
        MoveShip();
        SteerShip();
        FireProjectiles();
    }

    //Fires a projectile forwards whenever the player presses spacebar
    private void FireProjectiles()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Spawn a new projectile and give it its movement values
            Vector3 SpawnPos = transform.position + transform.up * ProjectileSpawnDistance;
            GameObject Projectile = Instantiate(ProjectilePrefab, SpawnPos, Quaternion.identity);
            Projectile.GetComponent<ProjectileMovement>().InitializeProjectile(transform.up, 8f + CurrentVelocity.magnitude);
            SoundEffectsPlayer.Instance.PlaySound("PlayerShoot");
        }
    }

    //Controls the thrusters and applys movement to the ship
    private void MoveShip()
    {
        //Get new velocity vector after applying user input
        bool ThrustForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        if (ThrustForward)
            CurrentVelocity += transform.up * ForwardThrusterPower * Time.deltaTime;
        bool ThrustReverse = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        if (ThrustReverse)
            CurrentVelocity += transform.up * ReverseThrusterPower * Time.deltaTime;

        //Start looping the thruster sound effect if we arent
        if(ThrustForward && !PlayingThrusterSound)
        {
            PlayingThrusterSound = true;
            SoundPlayer.UnPause();
        }
        //Otherwise stop looping if it shouldnt be anymore
        else if(!ThrustForward && PlayingThrusterSound)
        {
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
        NewPos = ScreenBounds.ClampPosInside(NewPos);

        //Move to the new location
        transform.position = NewPos;
    }

    //Steers the players ship
    private void SteerShip()
    {
        //Get new rotation value after reading user input
        bool SteerLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        if (SteerLeft)
            CurrentRotation += TurnSpeed * Time.deltaTime;
        bool SteerRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        if (SteerRight)
            CurrentRotation -= TurnSpeed * Time.deltaTime;

        //Clamp rotation value
        if (CurrentRotation >= 360f)
            CurrentRotation -= 360f;
        if (CurrentRotation <= -360f)
            CurrentRotation += 360f;

        //Create a new quaternion to give the player
        Quaternion NewRotation = Quaternion.Euler(0f, 0f, CurrentRotation);
        transform.rotation = NewRotation;
    }
}
