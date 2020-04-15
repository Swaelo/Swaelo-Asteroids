// ================================================================================================================================
// File:        SoundEffectsPlayer.cs
// Description:	Allows any script to easily play sound effects when they need to
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class SoundEffectsPlayer : MonoBehaviour
{
    //Singleton instance
    public static SoundEffectsPlayer Instance = null;
    private void Awake() { Instance = this; }

    //Sound Effect Dictionary
    [System.Serializable]
    public struct Sound
    {
        public string Name; //Name used to access this sound in the dictionary
        public AudioClip[] SoundClips;  //When multiple files are included they are selected at random
    }
    public Sound[] SoundDictionary; //Contains all sound effects that can be played
    public AudioSource SoundPlayer; //Used to play the sound effects

    //Takes the name of a sound effect and plays it if it can be found in the dictionary
    public void PlaySound(string SoundName, float VolumeScale = 1.0f)
    {
        //Make sure the volume level is appropriate
        VolumeScale = Mathf.Clamp(VolumeScale, 0f, 1f);
        VolumeScale *= 0.35f;

        //Look through all the sounds in the dictionary
        for(int i = 0; i < SoundDictionary.Length; i++)
        {
            //Check each one to see if its the sound we want to play right now
            Sound CurrentSound = SoundDictionary[i];
            if(CurrentSound.Name == SoundName)
            {
                //Play the sound and exit out of the loop
                int SoundSelection = Random.Range(1, CurrentSound.SoundClips.Length);
                SoundPlayer.PlayOneShot(CurrentSound.SoundClips[SoundSelection], VolumeScale);
                return;
            }
        }

        //Print an error message if the specified sound couldnt be found in the dictionary
        Debug.Log("Error: Couldnt find sound effect " + SoundName + " in the dictionary.");
    }
}
