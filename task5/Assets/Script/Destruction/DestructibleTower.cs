using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleTower : DestructibleObject
{
    private GameObject towerObject;
    private GameObject debrisObject;

    private ParticleSystem fallDustFx;
    private ParticleSystem smokeFx;

    private bool bIsFalling = false;
    private Vector3 fallStartRotation;

    private float fallRotMin = -2.5f;
    private float fallRotMax = 2.5f;

    private float fallYPos = -2f;
    private float fallYVel = 1f;

    private float debrisYPos = 0f;
    private float debrisYVel;

    private void Start()
    {
        towerObject = transform.Find("ForestCastle_Blue").gameObject;
        debrisObject = transform.Find("TowerDebris").gameObject;

        fallDustFx = transform.Find("FX_TowerFallDust").gameObject.GetComponentInChildren<ParticleSystem>();
        smokeFx = transform.Find("FX_TowerSmoke").gameObject.GetComponentInChildren<ParticleSystem>();

        Vector3 debrisPos = debrisObject.transform.localPosition;

        float fallingTime = Mathf.Abs(fallYPos) / fallYVel;
        debrisYVel = Mathf.Abs(debrisYPos - debrisPos.y) / fallingTime;

        // Don't register meshes
    }

    void Update()
    {
        if (bIsFalling)
        {
            Vector3 targetRotation = fallStartRotation;
            targetRotation.x += Random.Range(fallRotMin, fallRotMax);
            targetRotation.z += Random.Range(fallRotMin, fallRotMax);

            towerObject.transform.localEulerAngles = targetRotation;

            Vector3 targetPos = towerObject.transform.localPosition;
            targetPos.y = MathUtils.InterpConstantTo(targetPos.y, fallYPos, Time.deltaTime, fallYVel);
            towerObject.transform.localPosition = targetPos;

            Vector3 targetDebrisPos = debrisObject.transform.localPosition;
            targetDebrisPos.y = MathUtils.InterpConstantTo(targetDebrisPos.y, debrisYPos, Time.deltaTime, debrisYVel);
            debrisObject.transform.localPosition = targetDebrisPos;

            if (targetPos.y == fallYPos)
            {
                bIsFalling = false;
                fallDustFx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    public GameObject GetCastle()
    {
        return towerObject;
    }

    protected override void OnDestruct()
    {
        bIsFalling = true;
        fallStartRotation = towerObject.transform.localEulerAngles;
        FMODUnity.RuntimeManager.PlayOneShot("event:/TowerFall", transform.position);
        smokeFx.Play();
        fallDustFx.Play();

        GameObject.FindGameObjectWithTag("WorldGenerator").GetComponent<WorldGenerator>().OnTowerDestroyed();
    }
}
