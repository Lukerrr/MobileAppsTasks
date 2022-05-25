using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjectPart : MonoBehaviour
{
    private float disruptDelayMin = 0.25f;
    private float disruptDelayMax = 1.0f;

    private float disruptionRate = 2f;

    private float disrutionStartTime = 0f;
    private bool bIsDisrupting = false;

    private Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (body.IsSleeping() && !bIsDisrupting && CanDisrupt())
        {
            bIsDisrupting = true;
            disrutionStartTime = Random.Range(disruptDelayMin, disruptDelayMax) + Time.time;
        }

        if (bIsDisrupting)
        {
            ProcessDisruption();
        }
    }

    private bool CanDisrupt()
    {
        return GetComponentInParent<DestructibleObject>().bUseDisruption;
    }

    private void ProcessDisruption()
    {
        if (Time.time >= disrutionStartTime)
        {
            float scale = transform.localScale.magnitude;
            float newScale = MathUtils.InterpConstantTo(scale, 0f, Time.deltaTime, disruptionRate);
            transform.localScale = transform.localScale / scale * newScale;

            if (newScale == 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
