// ================================================================================================================================
// File:        AsteroidSize.cs
// Description:	Defines the different sizes of asteroids that can exist, so we know what to do when we destroy one
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public enum AsteroidSizes
{
    Small = 1,
    Medium = 2,
    Large = 3
}

public class AsteroidSize : MonoBehaviour
{
    public AsteroidSizes MySize;    //The size of this asteroid
}