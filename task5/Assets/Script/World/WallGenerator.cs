using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{

    public GameObject wallStraight;
    public GameObject wallAngled;
    public GameObject wallEnding;

    public List<WallGenerator> neightborWalls = new List<WallGenerator>();
    public void GenerateWall()
    {
        if (neightborWalls.Count == 0)
        {
            return;
        }

        Vector3 neigh1Vector, neigh2Vector;

        neigh1Vector = neightborWalls[0].transform.position - transform.position;
        neigh1Vector.Normalize();

        float orientAngle;
        GameObject wallClass;

        if (neightborWalls.Count == 1)
        {
            wallClass = wallEnding;
            orientAngle = Mathf.Rad2Deg * Mathf.Atan2(neigh1Vector.x, neigh1Vector.z);
        }
        else
        {
            neigh2Vector = neightborWalls[1].transform.position - transform.position;
            neigh2Vector.Normalize();

            float wallAngle = Mathf.Acos(Vector3.Dot(neigh1Vector, neigh2Vector));
            bool bIsAngledWall = MathUtils.NearlyEqual(Mathf.Abs(wallAngle), Mathf.PI / 2f);

            wallClass = bIsAngledWall ? wallAngled : wallStraight;

            Vector3 orientVector = neigh1Vector + neigh2Vector;

            if (bIsAngledWall)
            {
                if (orientVector.x > 0f && orientVector.z > 0f)
                {
                    orientAngle = 0f;
                }
                else if (orientVector.x < 0f && orientVector.z > 0f)
                {
                    orientAngle = -90f;
                }
                else if (orientVector.x < 0f && orientVector.z < 0f)
                {
                    orientAngle = 180f;
                }
                else
                {
                    orientAngle = 90f;
                }
            }
            else
            {
                orientAngle = Mathf.Rad2Deg * Mathf.Atan2(neigh1Vector.x, neigh1Vector.z);
            }
        }

        Instantiate(wallClass, transform.position, Quaternion.Euler(0f, orientAngle, 0f));

        Destroy(gameObject);
    }
}
