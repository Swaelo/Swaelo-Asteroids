// ================================================================================================================================
// File:        AsteroidManager.cs
// Description:	Handles spawning and tracking of asteroids on the field
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class AsteroidManager : MonoBehaviour
{
    //Singleton Instance
    public static AsteroidManager Instance = null;
    private void Awake() { Instance = this; }

    //Asteroid Prefabs
    public GameObject[] LargePrefabs;
    public GameObject[] MediumPrefabs;
    public GameObject[] SmallPrefabs;

    //Active Asteroids
    public List<GameObject> ActiveAsteroids = new List<GameObject>();
    public int MaxAsteroidCount = 26;   //Maximum number of asteroids that can be active at any one time

    //Minimum spawning distance between asteroids
    private float HalfMediumSpaceNeeded = 0.85f;
    private float HalfSmallSpaceNeeded = 0.35f;

    //Spawns in a set number of large asteroids for the beginning of a new level
    public void PrepareNewLevel(int AsteroidCount)
    {

    }

    //Returns a random asteroid prefab of the specified size
    private GameObject GetAsteroidPrefab(AsteroidSizes Size)
    {
        int Selection = Random.Range(1,
            Size == AsteroidSizes.Small ? 6 :
            Size == AsteroidSizes.Medium ? 4 : 3);
        return Size == AsteroidSizes.Small ? SmallPrefabs[Selection] :
            Size == AsteroidSizes.Medium ? MediumPrefabs[Selection] :
            LargePrefabs[Selection];
    }

    //Spawns an asteroid at a specified location
    public void SpawnAsteroid(AsteroidSizes Size, Vector3 Location)
    {
        //Ignore this function call if there is already the maximum number of asteroids active
        if (ActiveAsteroids.Count > MaxAsteroidCount)
            return;

        //Select one of the prefabs of the given size at random, and spawn it in at the given location
        GameObject RandomPrefab = GetAsteroidPrefab(Size);
        Quaternion RandomRotation = Quaternion.Euler(0f, 0f, Random.Range(-360f, 360f));
        GameObject AsteroidSpawn = Instantiate(RandomPrefab, Location, RandomRotation);

        //Add it to the tracking list with all the others
        ActiveAsteroids.Add(AsteroidSpawn);
    }

    //Spawns 2 smaller sizes asteroids near the previous ones location
    private void SpawnFragments(AsteroidSizes Size, Vector3 SpawnPos)
    {
        //Ignore this function call if the maximum amount of asteroids already exist
        if (ActiveAsteroids.Count >= MaxAsteroidCount)
            return;

        //Get a random direction to offset the spawns from the destroyed asteroids previous location
        Vector3 SpawnDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;

        //Get two locations where each fragment will be spawned in at
        Vector3 FirstSpawnPos = SpawnPos + SpawnDirection * (Size == AsteroidSizes.Medium ? HalfMediumSpaceNeeded : HalfSmallSpaceNeeded);
        Vector3 SecondSpawnPos = SpawnPos - SpawnDirection * (Size == AsteroidSizes.Medium ? HalfMediumSpaceNeeded : HalfSmallSpaceNeeded);

        //Get two random rotations to apply to the asteroids when spawning them in
        Quaternion FirstSpawnRotation = Quaternion.Euler(0f, 0f, Random.Range(-360f, 360f));
        Quaternion SecondSpawnRotation = Quaternion.Euler(0f, 0f, Random.Range(-360f, 360f));

        //Grab random prefabs to spawn in for each fragment
        GameObject FirstPrefab = GetAsteroidPrefab(Size);
        GameObject SecondPrefab = GetAsteroidPrefab(Size);

        //Spawn in the first prefab and add it to the tracking list with all the others
        GameObject FirstSpawn = Instantiate(FirstPrefab, FirstSpawnPos, FirstSpawnRotation);
        ActiveAsteroids.Add(FirstSpawn);

        //Spawn and track the 2nd prefab if adding the first one didnt cause us to reach the max asteroid count
        if(ActiveAsteroids.Count < MaxAsteroidCount)
        {
            GameObject SecondSpawn = Instantiate(SecondPrefab, SecondSpawnPos, SecondSpawnRotation);
            ActiveAsteroids.Add(SecondSpawn);
        }
    }

    //Destroys an asteroid and removes it from the active list, continues to the next level if there are no asteroids left
    public void DestroyAsteroid(GameObject Asteroid)
    {
        //Remove the asteroid from the active tracking list
        if (ActiveAsteroids.Contains(Asteroid))
            ActiveAsteroids.Remove(Asteroid);

        //Store the asteroids size and position, then destroy it
        AsteroidSizes AsteroidSize = Asteroid.GetComponent<AsteroidSize>().MySize;
        Vector3 AsteroidPos = Asteroid.transform.position;
        Destroy(Asteroid);

        //If the asteroid was large or medium, spawn a couple smaller asteroids nearby the destroyed asteroids location
        if (AsteroidSize == AsteroidSizes.Large)
            SpawnFragments(AsteroidSizes.Medium, AsteroidPos);
        else if (AsteroidSize == AsteroidSizes.Medium)
            SpawnFragments(AsteroidSizes.Small, AsteroidPos);

        //Play sound effect and award points for destroying the asteroid
        SoundEffectsPlayer.Instance.PlaySound("AsteroidDestroy");
        GameState.Instance.IncreaseScore((int)(AsteroidSize == AsteroidSizes.Large ? ScoreValues.LargeAsteroid :
            AsteroidSize == AsteroidSizes.Medium ? ScoreValues.MediumAsteroid :
            ScoreValues.SmallAsteroid));

        //Continue onto the next level if they are no more asteroids left
        if (ActiveAsteroids.Count <= 0)
            GameState.Instance.NextLevel();
    }
}
