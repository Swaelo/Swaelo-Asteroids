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
    public static Vector3 WrapPosInside(Vector3 CurrentPos)
    {
        //Start with the current location
        Vector3 WrappedPos = CurrentPos;

        //Wrap around to the other side when leaving out one side of the screen
        if (WrappedPos.x < XPosRange.x)
            WrappedPos.x = XPosRange.y;
        else if (WrappedPos.x > XPosRange.y)
            WrappedPos.x = XPosRange.x;
        if (WrappedPos.y < YPosRange.x)
            WrappedPos.y = YPosRange.y;
        else if (WrappedPos.y > YPosRange.y)
            WrappedPos.y = YPosRange.x;

        //Return the wrapped location
        return WrappedPos;
    }

    //Returns a vector keeping it inside the screen bounds
    public static Vector3 ClampPosInside(Vector3 CurrentPos)
    {
        //Take the current position
        Vector3 ClampedPos = CurrentPos;

        //Clamp all the values inside the max screen values
        ClampedPos.x = Mathf.Clamp(ClampedPos.x, XPosRange.x, XPosRange.y);
        ClampedPos.y = Mathf.Clamp(ClampedPos.y, YPosRange.x, YPosRange.y);

        //Return the clamped position vector
        return ClampedPos;
    }

    //Checks if a position lies within the screenbounds
    public static bool IsPosInside(Vector3 Pos)
    {
        if (Pos.x < XPosRange.x ||
            Pos.x > XPosRange.y ||
            Pos.y < YPosRange.x ||
            Pos.y > YPosRange.y)
            return false;
        return true;
    }

    //Returns a random location inside the screen bounds
    public static Vector3 GetInsidePos()
    {
        return new Vector3(
            Random.Range(XPosRange.x, XPosRange.y),
            Random.Range(YPosRange.x, YPosRange.y),
            0f);
    }
}
