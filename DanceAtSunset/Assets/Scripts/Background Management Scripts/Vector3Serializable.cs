using UnityEngine;

[System.Serializable]
public struct Vector3Serializable
{
    public float x;
    public float y;
    public float z;

    public Vector3Serializable(Vector3 pos)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}