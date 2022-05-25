using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject cameraObj;

    public float cameraDistanceMin = 1.5f;
    public float cameraDistanceMax = 4f;

    private Vector3 cameraDir;
    private float cameraDistance;

    void Start()
    {
        cameraDir = cameraObj.transform.localPosition.normalized;
        cameraDistance = cameraDistanceMax;
    }

    void Update()
    {
        CheckCollision();
    }

    private void CheckCollision()
    {
        Vector3 desiredCameraPos = transform.TransformPoint(cameraDir * cameraDistanceMax);

        RaycastHit hit;
        if (Physics.Linecast(transform.position, desiredCameraPos, out hit, ~(1 << 8)))
        {
            cameraDistance = Mathf.Clamp(hit.distance, cameraDistanceMin, cameraDistanceMax);
        }
        else
        {
            cameraDistance = cameraDistanceMax;
        }

        cameraObj.transform.localPosition = cameraDistance * cameraDir;
    }
}
