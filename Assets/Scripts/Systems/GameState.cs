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
    public GameObject PlayerShipPrefab; //Used to respawn them into the game after dying
    public GameObject PlayerShip;   //Reference to current instance of the players ship

    //Respawn Timer
    private bool WaitingToRespawn = false;  //When the player dies it takes a short moment before they will be respawned
    private float RespawnInterval = 2.5f;   //How long it takes for the player to respawn after dying
    private float RespawnTimer = 2.5f;  //Seconds left until the player respawns

    //Score/Lives
    public int Lives = 3;   //Extra lives remaining
    public int MaxLives = 5;    //Maximum number of extra lives attainable
    public int Score = 0;   //The players score
    private int NextFreeLife = 10000;   //How many points needed to get the next free life
    private int FreeLifeInterval = 10000;   //How far apart each score milestone for another free life

    //Level Progression
    public bool DebugMode = false;  //Game wont start automatically when this is true
    public int Level = 1;   //Current level
    private int AsteroidCount = 4;  //How many asteroids to spawn at the start of the next round, increases by 1 each round
    private int MaxAsteroidCount = 11;  //Maximum number of asteroids that can be spawned in at the start of a new level

    //GameOver / Restarting
    public bool GameOver = false;

    //Saucer spawning
    public GameObject SmallSaucerPrefab;    //Used to spawn small saucers into the level
    public GameObject LargeSaucerPrefab;    //Used to spawn large saucers into the level
    private bool SaucerActive = false;  //Tracks when there is currently a saucer in the level
    private GameObject ActiveSaucer = null; //Reference to current saucer enemy if there is one
    private Vector2 SaucerSpawnInterval = new Vector2(5f, 15f); //How often saucers will spawn into the level
    private float NextSaucerSpawn;  //Time left until the next saucer enemy spawns in
    private bool SaucerTimerActive = true; //Starts counting from the beginning of the round, then resets when the saucer has been destroyed
    private Vector2 SaucerSpawnXPosValues = new Vector2(-7.4f, 7.4f);   //XPos for spawning saucers on either side of the screen
    private Vector2 SaucerSpawnYPosRange = new Vector2(-4.3f, 4.3f);    //Range on y axis where saucers can be spawned in

    //Pausing
    public bool GamePaused = false;

    private void Start()
    {
        //Start the first level and set the initial saucer spawn timer
        if(!DebugMode)
        {
            NextLevel(true);
            NextSaucerSpawn = Random.Range(SaucerSpawnInterval.x, SaucerSpawnInterval.y);
            PlayerShip = Instantiate(PlayerShipPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    private void RestartGame()
    {
        AsteroidManager.Instance.PrepareGameRestart();
        Lives = 3;
        Score = 0;
        NextFreeLife = 10000;
        Level = 1;
        AsteroidCount = 4;
        GameOver = false;
        NextSaucerSpawn = Random.Range(SaucerSpawnInterval.x, SaucerSpawnInterval.y);
        PlayerShip = Instantiate(PlayerShipPrefab, Vector3.zero, Quaternion.identity);
        UIManager.Instance.GameOverText.SetActive(false);
        UIManager.Instance.UpdateScoreDisplay(Score);
        UIManager.Instance.UpdateLivesDisplay(Lives);
        UIManager.Instance.UpdateRoundDisplay(Level);
        NextLevel(true);
    }

    private void Update()
    {
        //Can just press Enter to start a new game during gameover
        if (GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                RestartGame();
        }
        //While dead, cant do anything except wait to be respawned
        else if (WaitingToRespawn)
            RespawnPlayer();
        //Otherwise just continue the game as normal
        else
        {
            //Spawn saucer enemies periodically while the game is active
            if (!DebugMode && !GamePaused)
                SpawnSaucers();

            //Allow player to toggle pause
            if(Input.GetButtonDown("Pause"))
            {
                GamePaused = !GamePaused;
                UIManager.Instance.GamePausedText.SetActive(GamePaused);
            }
        }
    }

    //Respawns the player after the timer expires
    private void RespawnPlayer()
    {
        RespawnTimer -= Time.deltaTime;
        if(RespawnTimer <= 0.0f)
        {
            WaitingToRespawn = false;
            PlayerShip = Instantiate(PlayerShipPrefab, Vector3.zero, Quaternion.identity);
            SoundEffectsPlayer.Instance.PlaySound("PlayerRespawn");
        }
    }

    //Awards points to the player
    public void IncreaseScore(int Amount)
    {
        //Increase the current score value
        Score += Amount;

        //Check if an extra life should be added
        if (Score >= NextFreeLife)
            GiveExtraLife();

        //Update the score display
        UIManager.Instance.UpdateScoreDisplay(Score);
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
        SoundEffectsPlayer.Instance.PlaySound(FirstLevel ? "GameStart" : "LevelComplete", 0.25f);

        //Make sure the extra lives are displayed correctly
        UIManager.Instance.UpdateLivesDisplay(Lives);

        //Destroy any saucer enemy still alive at the end of the level
        if(SaucerActive)
        {
            SaucerActive = false;
            Destroy(ActiveSaucer);
        }

        //Increase the level counter and asteroid spawn count if we are progressing from a previous level
        if(!FirstLevel)
        {
            Level++;
            if (AsteroidCount < MaxAsteroidCount)
                AsteroidCount++;
        }

        //Update the UI round number display
        UIManager.Instance.UpdateRoundDisplay(Level);

        //Spawn in all the new asteroids for this level, then increase the counter ready for next level
        AsteroidManager.Instance.PrepareNewLevel(AsteroidCount);
    }

    //Spawns a saucer enemy into the level once the timer has expired
    private void SpawnSaucers()
    {
        //Wait until the timer has been actived
        if (SaucerTimerActive)
        {
            //Wait for the timer
            NextSaucerSpawn -= Time.deltaTime;
            if (NextSaucerSpawn <= 0.0f)
            {
                //Disable the timer
                SaucerTimerActive = false;
                //Decide which type of saucer to spawn in
                bool SpawnSmall = Random.value >= 0.5f;
                GameObject SaucerPrefab = SpawnSmall ? SmallSaucerPrefab : LargeSaucerPrefab;
                //Decide which side to spawn the saucer, and their spawn location
                bool EnterLeft = Random.value >= 0.5f;
                Vector3 SpawnPos = new Vector3(EnterLeft ? SaucerSpawnXPosValues.x : SaucerSpawnXPosValues.y,
                    Random.Range(SaucerSpawnYPosRange.x, SaucerSpawnYPosRange.y), 0f);
                //Spawn the saucer into the level and keep track of them
                SaucerActive = true;
                ActiveSaucer = GameObject.Instantiate(SaucerPrefab, SpawnPos, Quaternion.identity);
            }
        }
    }

    //Removes the current saucer from tracking and starts the countdown timer before the next one will be spawned in
    public void SaucerDestroyed()
    {
        SaucerActive = false;
        SaucerTimerActive = true;
        NextSaucerSpawn = Random.Range(SaucerSpawnInterval.x, SaucerSpawnInterval.y);
    }

    //Respawns the player if they have lives left, otherwise goes to the gameover screen
    public void PlayerDead()
    {
        //If the player still has some extra lives remaining, start the respawn timer before letting them continue play
        if(Lives > 0)
        {
            Lives--;
            UIManager.Instance.UpdateLivesDisplay(Lives);
            WaitingToRespawn = true;
            RespawnTimer = RespawnInterval;
        }
        //If they havnt any lives left its a game over
        else
        {
            SoundEffectsPlayer.Instance.PlaySound("GameOver");
            UIManager.Instance.GameOverText.SetActive(true);
            GameOver = true;
        }
    }
}
