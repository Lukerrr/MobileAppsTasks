using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    enum EState
    {
        STATE_IDLE = 0,
        STATE_ATTACK_WALL,
        STATE_ATTACK_TOWER,
        STATE_ATTACK_PLAYER,
    }

    public GameObject sightPos;
    public TankActor tank;

    private float aimFireDelay = 2f;

    private float fireDelayStart = -1f;

    private NavMeshAgent navMeshAgent;
    private bool bIsActive = true;

    private EState state = EState.STATE_ATTACK_WALL;

    private TowerWall targetTowerWall;
    private DestructibleObject targetWallPart;
    private DestructibleTower targetTower;
    private TankActor playerTank;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = true;

        // Choose the nearest tower wall
        float minDist = float.MaxValue;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("TowerWall"))
        {
            float dist = (go.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                targetTowerWall = go.GetComponent<TowerWall>();
                minDist = dist;
            }
        }

        targetTower = GameObject.FindGameObjectWithTag("Tower").GetComponent<DestructibleTower>();
    }

    void Update()
    {
        if (!bIsActive)
        {
            return;
        }

        playerTank = GameObject.FindGameObjectWithTag("Player").GetComponent<TankActor>();

        if (tank.IsDead())
        {
            StopMovement();

            GameObject.FindGameObjectWithTag("WorldGenerator").GetComponent<WorldGenerator>().OnEnemyKilled();

            bIsActive = false;
            return;
        }

        switch (state)
        {
            case EState.STATE_ATTACK_WALL:
                ProcessStateAttackWall();
                break;
            case EState.STATE_ATTACK_TOWER:
                ProcessStateAttackTower();
                break;
            case EState.STATE_ATTACK_PLAYER:
                ProcessStateAttackPlayer();
                break;
            case EState.STATE_IDLE:
                StopMovement();
                ResetCannonOrient();
                break;
            default:
                break;
        }
    }

    private void ProcessStateAttackWall()
    {
        if (targetTowerWall.IsDestroyed())
        {
            state = EState.STATE_ATTACK_TOWER;
            return;
        }

        if (!playerTank.IsDead() && CanSee(playerTank.gameObject))
        {
            state = EState.STATE_ATTACK_PLAYER;
            return;
        }

        if (targetWallPart == null || targetWallPart.IsDestroyed())
        {
            targetWallPart = targetTowerWall.GetRandomAlivePart();
        }

        ProcessTarget(targetWallPart.gameObject);
    }

    private void ProcessStateAttackTower()
    {
        if (targetTower.IsDestroyed())
        {
            state = EState.STATE_IDLE;
            return;
        }

        if (!playerTank.IsDead() && CanSee(playerTank.gameObject))
        {
            state = EState.STATE_ATTACK_PLAYER;
            return;
        }

        ProcessTarget(targetTower.GetCastle());
    }

    private void ProcessStateAttackPlayer()
    {
        if (playerTank.IsDead())
        {
            state = EState.STATE_ATTACK_WALL;
            return;
        }

        ProcessTarget(playerTank.gameObject);
    }

    private void ProcessTarget(GameObject go)
    {
        if (CanSee(go))
        {
            StopMovement();

            if (AimAt(go))
            {
                if (fireDelayStart < 0f)
                {
                    fireDelayStart = Time.time;
                }
                else if (Time.time - fireDelayStart >= aimFireDelay)
                {
                    tank.Fire();
                    fireDelayStart = -1f;
                }
            }
        }
        else
        {
            if (HasReachedDestination())
            {
                MoveTo(go.transform.position);
            }

            ResetCannonOrient();
        }
    }

    private bool CanSee(GameObject go)
    {
        Vector3 start = sightPos.transform.position;
        Vector3 dir = GetObjectCenter(go) - start;
        float radius = 0.2f;
        int tankLayerMask = (1 << 6);

        RaycastHit[] tankHits = Physics.SphereCastAll(start, radius, dir, float.PositiveInfinity, tankLayerMask);
        foreach (var tankHit in tankHits)
        {
            if (!(tankHit.transform == gameObject || tankHit.transform.IsChildOf(gameObject.transform)))
            {
                // Ray hits another tank
                return false;
            }
        }

        RaycastHit hit;
        if (Physics.SphereCast(start, radius, dir, out hit, float.PositiveInfinity, ~(tankLayerMask)))
        {
            if (hit.transform == go || hit.transform.IsChildOf(go.transform))
            {
                return true;
            }
            else
            {
                // Ray hits obstacle
                return false;
            }
        }

        return true;
    }

    private void MoveTo(Vector3 pos)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(pos);
        tank.Move(1, true);
    }

    private bool AimAt(GameObject go)
    {
        Vector3 aimPos = GetObjectCenter(go);
        Vector3 shootDir = aimPos - sightPos.transform.position;
        shootDir.Normalize();

        Quaternion dirQuat = Quaternion.FromToRotation(new Vector3(0f, 0f, 1f), shootDir);
        tank.towerYaw = dirQuat.eulerAngles.y - tank.transform.eulerAngles.y;
        tank.cannonPitch = dirQuat.eulerAngles.x;

        if ((shootDir - tank.GetFireDir()).magnitude <= 0.1f)
        {
            return true;
        }

        return false;
    }

    private Vector3 GetObjectCenter(GameObject go)
    {
        Vector3 sumVector = new Vector3(0f, 0f, 0f);

        foreach (Transform child in go.transform)
        {
            sumVector += child.position;
        }

        return sumVector / go.transform.childCount;
    }

    private void ResetCannonOrient()
    {
        tank.towerYaw = 0;
        tank.cannonPitch = 0;
    }

    private void StopMovement()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        tank.Move(0, true);
    }

    private bool HasReachedDestination()
    {
        return navMeshAgent.isStopped || navMeshAgent.isPathStale || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }
}
