// ================================================================================================================================
// File:        AsteroidDrift.cs
// Description:	Makes asteroids slowly float and spin around the level
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class AsteroidDrift : MonoBehaviour
{
    //Drift
    private bool ExplosionForce = false;    //Tracks if there has been an explosion force that has blown this asteroid away
    private Vector3 ForceDirection; //Medium and Small asteroids are blown away from the explosion when the large asteroid is broken down
    private float MoveSpeed;    //Current movement speed
    public Vector2 MoveSpeedRange = new Vector2(0.15f, 0.5f);  //Available movement speeds
    private Vector3 DriftDirection; //Current direction of travel

    //Spin
    private float RotationSpeed;    //Current rotation speed
    private Vector2 RotationSpeedRange = new Vector2(-25f, 25f);    //Available rotation speeds
    private float CurrentRotation;  //Current degrees of rotation around the z axis

    private void Start()
    {
        //Grab and store current rotation
        CurrentRotation = transform.rotation.z;

        //Set random movement speed, travel direction and rotation speed
        MoveSpeed = Random.Range(MoveSpeedRange.x, MoveSpeedRange.y);
        DriftDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
        RotationSpeed = Random.Range(RotationSpeedRange.x, RotationSpeedRange.y);
    }

    private void Update()
    {
        //Do nothing while the game is paused
        if (GameState.Instance.GamePaused)
            return;

        //Use the explosion force, or the random drift direction if there isnt one to apply movement to the asteroid
        Vector3 NewPos = transform.position + (ExplosionForce ? ForceDirection : DriftDirection) * MoveSpeed * Time.deltaTime;
        NewPos = ScreenBounds.WrapPosInside(NewPos);
        transform.position = NewPos;

        //Spin a bit
        CurrentRotation += RotationSpeed * Time.deltaTime;
        if (CurrentRotation < -360f)
            CurrentRotation += 360f;
        if (CurrentRotation > 360f)
            CurrentRotation -= 360f;
        Quaternion NewRotation = Quaternion.Euler(0f, 0f, CurrentRotation);
        transform.rotation = NewRotation;
    }

    //Applies explosion force to the asteroid from its parent asteroid blowing up
    public void ApplyExplosionForce(Vector3 ForceDirection)
    {
        ExplosionForce = true;
        this.ForceDirection = ForceDirection;
    }
}
