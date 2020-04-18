// ================================================================================================================================
// File:        StarAnimator.cs
// Description:	Controls animation of all the stars in the background so they're all random and differently animated
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class StarAnimator : MonoBehaviour
{
    public Animator AnimationController;    //Used to change between the animation states
    private int AnimationState; //The stars current animation state
    private Vector2 StateChangeRate = new Vector2(1f, 3f);   //How often the star changes animation states
    private float NextStateChange;  //How long until the next animation state change
    public Vector2 ScaleRange = new Vector2(0.25f, 1.25f); //How much the star can be scaled

    private void Start()
    {
        //Select a state at random and set the change timer
        AnimationState = (int)Random.Range(1, 5);
        AnimationController.SetTrigger("State" + AnimationState.ToString());
        NextStateChange = Random.Range(StateChangeRate.x, StateChangeRate.y);
        //Apply some random scaling to the star to differentiate them from one another
        float NewScale = Random.Range(ScaleRange.x, ScaleRange.y);
        transform.localScale = new Vector3(NewScale, NewScale, NewScale);
    }

    private void Update()
    {
        //Do nothing while the game is paused
        if (GameState.Instance.GamePaused)
            return;

        //Wait for the timer to expire
        NextStateChange -= Time.deltaTime;
        if(NextStateChange <= 0.0f)
        {
            //Reset the change timer
            NextStateChange = Random.Range(StateChangeRate.x, StateChangeRate.y); 
            //Change to one of the other states at random
            List<int> OtherStates = new List<int>();
            for (int i = 1; i < 5; i++)
                if (i != AnimationState)
                    OtherStates.Add(i);
            int StateSelection = (int)Random.Range(0, 3);
            AnimationState = OtherStates[StateSelection];
            //Trigger this state change in the animation controller
            AnimationController.SetTrigger("State" + AnimationState.ToString());
        }
    }
}
