using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerWall : MonoBehaviour
{
    public List<DestructibleObject> wallParts;

    private System.Random rnd = new System.Random();

    public bool IsDestroyed()
    {
        foreach (var w in wallParts)
        {
            if (!w.IsDestroyed())
            {
                return false;
            }
        }
        return true;
    }

    public List<DestructibleObject> GetAliveParts()
    {
        List<DestructibleObject> result = new List<DestructibleObject>();

        foreach (var w in wallParts)
        {
            if (!w.IsDestroyed())
            {
                result.Add(w);
            }
        }

        return result;
    }

    public DestructibleObject GetRandomAlivePart()
    {
        List<DestructibleObject> parts = GetAliveParts();
        if (parts.Count == 0)
        {
            return null;
        }

        return parts[rnd.Next(parts.Count)];
    }
}
