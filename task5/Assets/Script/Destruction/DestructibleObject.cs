using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public bool bUseDisruption = true;

    private List<GameObject> meshParts = new List<GameObject>();
    private bool bDestroyed = false;

    private void Start()
    {
        foreach (var mesh in GetComponentsInChildren<MeshFilter>())
        {
            meshParts.Add(mesh.gameObject);
        }
    }

    public void Explode(Vector3 pt, float force, float radius)
    {
        if (bDestroyed)
        {
            return;
        }

        bDestroyed = true;

        foreach (var part in meshParts)
        {
            Rigidbody partBody = part.GetComponent<Rigidbody>();

            if (partBody == null)
            {
                partBody = part.AddComponent<Rigidbody>();
            }

            partBody.useGravity = true;
            partBody.isKinematic = false;
            partBody.sleepThreshold = 0.001f;

            partBody.AddExplosionForce(force, pt, radius);

            part.AddComponent<DestructibleObjectPart>();
        }

        OnDestruct();
    }

    public bool IsDestroyed()
    {
        return bDestroyed;
    }

    protected virtual void OnDestruct()
    {

    }
}
