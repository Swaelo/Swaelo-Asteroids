// ================================================================================================================================
// File:        SaucerSize.cs
// Description:	Defines the different sizes of saucer enemies that exist
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public enum SaucerSizes
{
    Small = 1,
    Large = 2
}

public class SaucerSize : MonoBehaviour
{
    public SaucerSizes MySize;  //The size of this saucer enemy
}
