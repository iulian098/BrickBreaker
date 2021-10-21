using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BrickData
{
    public enum BrickType
    {
        normal,
        exploding,
        metal,
        stronger
    }

    public Vector2 position;
    public BrickType brickType;

    public BrickData(Vector2 _position, BrickType _type)
    {
        position = _position;
        brickType = _type;
    }

    public override string ToString()
    {
        return position + ", " + brickType.ToString();
    }
}
