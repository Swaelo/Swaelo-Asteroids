// ================================================================================================================================
// File:        GameState.cs
// Description:	Tracks the players score and lives, what level they're on, handles level transitions
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

//Point values awarded for different gameplay events
public enum ScoreValues
{
    LargeAsteroid = 20,
    MediumAsteroid = 50,
    SmallAsteroid = 100,
    LargeSaucer = 200,
    SmallSaucer = 1000
}

public class GameState : MonoBehaviour
{
    //Singleton Instance
    public static GameState Instance = null;
    private void Awake() { Instance = this; }

    //Player reference
    public GameObject PlayerShip;

    //Score/Lives
    public int Lives = 3;   //Extra lives remaining
    public int MaxLives = 5;    //Maximum number of extra lives attainable
    public int Score = 0;   //The players score
    private int NextFreeLife = 10000;   //How many points needed to get the next free life
    private int FreeLifeInterval = 10000;   //How far apart each score milestone for another free life

    //Level Progression
    public int Level = 1;   //Current level
    private int AsteroidCount = 4;  //How many asteroids to spawn at the start of the next round, increases by 1 each round
    private int MaxAsteroidCount = 11;  //Maximum number of asteroids that can be spawned in at the start of a new level

    private void Start()
    {
        //Start the first level
        NextLevel(true);
    }

    //Awards points to the player
    public void IncreaseScore(int Amount)
    {
        //Increase the current score value
        Score += Amount;

        //Check if an extra life should be added
        if (Score >= NextFreeLife)
            GiveExtraLife();
    }

    //Awards an extra life to the player
    private void GiveExtraLife()
    {
        //Ignore this function call if the they have the max amount of lives
        if (Lives >= MaxLives)
            return;

        //Set the score milestone for the next free life
        NextFreeLife += FreeLifeInterval;

        //Play sound and increase the players life counter
        SoundEffectsPlayer.Instance.PlaySound("ExtraLife");
        Lives++;
    }

    //Starts the next level once the current level has been cleared of all asteroids
    public void NextLevel(bool FirstLevel = false)
    {
        //Play start of game / new level sound effect
        SoundEffectsPlayer.Instance.PlaySound(FirstLevel ? "GameStart" : "LevelComplete");
        //Increase the level counter and asteroid spawn count if we are progressing from a previous level
        if(!FirstLevel)
        {
            Level++;
            if (AsteroidCount < MaxAsteroidCount)
                AsteroidCount++;
        }
        //Spawn in all the new asteroids for this level, then increase the counter ready for next level
        AsteroidManager.Instance.PrepareNewLevel(AsteroidCount);
    }
}
