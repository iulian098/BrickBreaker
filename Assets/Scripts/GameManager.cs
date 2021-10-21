using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static Vector2 GetBounds()
    {
        Vector3 bound;
        bound = Camera.main.ViewportToWorldPoint(Vector3.zero);
        return bound * -1;
    }

    public static Dictionary<string, bool> CreatePowerList()
    {
        Dictionary<string, bool> powers = new Dictionary<string, bool>();
        powers.Add("BigBall", false);
        powers.Add("SmallBall", false);
        powers.Add("Magnet", false);

        return powers;

    }

    public static void setWalls(Transform leftWall, Transform rightWall, Transform topWall, Transform gameOverTrigger)
    {
        Vector2 bounds = GetBounds();
        Vector3 wallScale;
        Vector3 wallPos;

        //Set walls position
        wallPos = new Vector3(0, bounds.y, 0);
        topWall.transform.position = wallPos;

        wallPos = new Vector3(-bounds.x, 0, 0);
        leftWall.transform.position = wallPos;

        wallPos = new Vector3(bounds.x, 0, 0);
        rightWall.transform.position = wallPos;

        wallPos = new Vector3(0, -bounds.y, 0);
        gameOverTrigger.transform.position = wallPos;

        //Set walls scale
        wallScale = topWall.transform.localScale;
        wallScale.x = bounds.x * 2;
        topWall.transform.localScale = wallScale;

        wallScale = gameOverTrigger.transform.localScale;
        wallScale.x = bounds.x * 2;
        gameOverTrigger.transform.localScale = wallScale;

        wallScale = leftWall.transform.localScale;
        wallScale.y = bounds.y * 2;
        leftWall.transform.localScale = wallScale;

        wallScale = rightWall.transform.localScale;
        wallScale.y = bounds.y * 2;
        rightWall.transform.localScale = wallScale;


    }

    public static bool IsFirstRun()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("FirstRun", 1));
    }
}
