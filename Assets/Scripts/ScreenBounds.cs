// ================================================================================================================================
// File:        ScreenBounds.cs
// Description:	Defines the locations of the screen borders so entities and projectiles know when to loop around to the other side
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public static class ScreenBounds
{
    //Min/Max position values inside the screen bounds
    private static Vector2 XPosRange = new Vector2(-8f, 8f);
    private static Vector2 YPosRange = new Vector2(-5f, 5f);

    //Returns a vector keeping it inside the screen bounds, going outside one edge re-enters from the opposite edge
    public static Vector3 KeepPosInside(Vector3 CurrentPos)
    {
        //Copy the current pos into a new vector
        Vector3 InsidePos = CurrentPos;

        //Make sure it stays inside the bounds of the screen
        if (InsidePos.x < XPosRange.x)
            InsidePos.x = InsidePos.y;
        if (InsidePos.x > XPosRange.y)
            InsidePos.x = XPosRange.x;
        if (InsidePos.y < YPosRange.x)
            InsidePos.y = YPosRange.y;
        if (InsidePos.y > YPosRange.y)
            InsidePos.y = YPosRange.x;

        //Return the final location
        return InsidePos;
    }
}
