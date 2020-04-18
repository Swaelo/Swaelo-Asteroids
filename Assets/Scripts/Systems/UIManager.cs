// ================================================================================================================================
// File:        UIManager.cs
// Description:	Handles everything UI releated
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Singleton Instance
    public static UIManager Instance = null;
    private void Awake() { Instance = this; }

    //UI Elements
    public GameObject GamePausedText;
    public GameObject GameOverText;
    public Text ScoreText;
    public Text RoundText;
    public Image[] ExtraLives;

    public void UpdateScoreDisplay(int ScoreValue)
    {
        ScoreText.text = "Score: " + ScoreValue.ToString();
    }

    public void UpdateRoundDisplay(int RoundNumber)
    {
        RoundText.text = "Round: " + RoundNumber.ToString();
    }

    public void UpdateLivesDisplay(int ExtraLives)
    {
        for(int i = 1; i < 6; i++)
            this.ExtraLives[i - 1].gameObject.SetActive(ExtraLives >= i);
    }
}
