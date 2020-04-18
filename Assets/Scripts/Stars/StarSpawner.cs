// ================================================================================================================================
// File:        StarSpawner.cs
// Description:	Places a bunch of stars at random into the background at the start of the game
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    public GameObject StarPrefab;   //Star object to spawn in all over the place
    public int StarAmount = 25;    //Amount of stars to spawn in
    private Vector3 SpawnDirection = new Vector3(0f, 1f, 0f);   //Current direction to spawn stars in, from the middle of the screen
    private float MaxSpawnDistance = 9.5f;  //Maximum distance from the center of the screen where stars can be spawned
    private float RotationPerSpawn; //Spawn direction vector rotation after each spawn

    private void Start()
    {
        //Spawn all the stars into place
        RotationPerSpawn = 360f / StarAmount;
        for(int i = 0; i < StarAmount; i++)
        {
            //Get the furthest distance possible spawn location in the current spawn direction, then calculate its distance
            Vector3 MaxDistanceSpawnPos = SpawnDirection * MaxSpawnDistance;
            MaxDistanceSpawnPos = ScreenBounds.ClampPosInside(MaxDistanceSpawnPos);
            float MaxValidSpawnDistance = Vector3.Distance(Vector3.zero, MaxDistanceSpawnPos);
            //Spawn a new star in a random distance between 0 and this max distance
            float RandomSpawnDistance = Random.Range(0f, MaxValidSpawnDistance);
            Vector3 SpawnPos = SpawnDirection * RandomSpawnDistance;
            Instantiate(StarPrefab, SpawnPos, Quaternion.identity);
            //Rotate the spawn direction vector, ready for the next star to be spawned
            SpawnDirection = Quaternion.AngleAxis(-RotationPerSpawn, Vector3.forward) * SpawnDirection;
        }
    }
}
