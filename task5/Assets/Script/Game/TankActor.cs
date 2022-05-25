using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankActor : MonoBehaviour
{

    public float towerRotationSpeed = 100f;
    public float cannonRotationSpeed = 50f;

    public float cannonPitchMin = -10f;
    public float cannonPitchMax = 10f;

    public float movementVelocity = 2.5f;
    public float rotationVelocity = 25f;

    public float fireDelay = 0.01f;

    public bool bGodmode = false;
    public bool bNoRecoil = false;

    public float towerYaw { get; set; }
    public float cannonPitch { get; set; }

    public GameObject shotFx;
    public GameObject explosionFx;
    public GameObject towerFireFx;

    public List<ParticleSystem> moveFx;
    public List<ParticleSystem> dragFx;

    public GameObject ammoObject;

    private GameObject tower;
    private GameObject cannon;
    private GameObject fireLocator;
    private Rigidbody body;
    private Rigidbody towerBody;

    private GameObject wheelFLeft;
    private GameObject wheelFRight;
    private GameObject wheelBLeft;
    private GameObject wheelBRight;

    FMOD.Studio.EventInstance movementSound;
    FMOD.Studio.EventInstance tracksSound;
    FMOD.Studio.EventInstance towerSound;

    private bool bIsDead = false;
    private Vector3 linearVelocity = new Vector3();
    private Vector3 angularVelocity = new Vector3();

    private Vector3 lastPosition = new Vector3();
    private Vector3 lastRotation = new Vector3();

    private float lastMoveInput = 0f;
    private float lastMoveInputVel = 0f;
    private float lastDragTime = 0f;
    private float dragDelay = 0.45f;

    private float movementInputAlpha = 0f;
    private float tracksVelocityAlpha = 0f;
    private float towerVelocityAlpha = 0f;

    private float reloadPercent = 1f;

    void Start()
    {
        tower = transform.Find("TankFree_Tower").gameObject;
        cannon = tower.transform.Find("TankFree_Canon").gameObject;
        fireLocator = cannon.transform.Find("TankFree_FireLocator").gameObject;

        wheelFLeft = transform.Find("TankFree_Wheel_f_left").gameObject;
        wheelFRight = transform.Find("TankFree_Wheel_f_right").gameObject;
        wheelBLeft = transform.Find("TankFree_Wheel_b_left").gameObject;
        wheelBRight = transform.Find("TankFree_Wheel_b_right").gameObject;

        body = GetComponent<Rigidbody>();
        towerBody = tower.GetComponent<Rigidbody>();

        movementSound = FMODUnity.RuntimeManager.CreateInstance("event:/TankMovement");
        tracksSound = FMODUnity.RuntimeManager.CreateInstance("event:/TankTracksMovement");
        towerSound = FMODUnity.RuntimeManager.CreateInstance("event:/TankTowerRotation");

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(movementSound, body.transform, body);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(tracksSound, body.transform, body);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(towerSound, towerBody.transform, body);

        movementSound.start();
        tracksSound.start();
        towerSound.start();

        UpdateKinematics();
    }

    void Update()
    {
        UpdateKinematics();
        UpdateCannonPosition();
        UpdateWheelsPosition();
        UpdateMovementSound();
        UpdateReload();
    }

    public void Move(float Value, bool bInputOnly = false)
    {
        if (bIsDead)
        {
            return;
        }

        float moveInputVel = (Mathf.Abs(Value) - Mathf.Abs(lastMoveInput)) / Time.deltaTime;

        float dragThr = 5.0f;
        if (lastMoveInputVel <= dragThr && moveInputVel > dragThr)
        {
            Drag();
        }

        if (lastMoveInput == 0f && Value != 0f)
        {
            StartMoveFx();
        }
        else if (lastMoveInput != 0f && Value == 0f)
        {
            StopMoveFx();
        }

        lastMoveInputVel = moveInputVel;
        lastMoveInput = Value;

        if (!bInputOnly)
        {
            float fwdVel = Value * movementVelocity;
            transform.position += transform.forward * fwdVel * Time.deltaTime;
        }
    }

    public void Rotate(float Value)
    {
        if (bIsDead)
        {
            return;
        }

        float angularVel = Value * rotationVelocity;
        transform.eulerAngles += new Vector3(0, angularVel * Time.deltaTime, 0);
    }

    public bool IsDead()
    {
        return bIsDead;
    }

    public void Fire()
    {
        if (!CanFire() || bIsDead)
        {
            return;
        }

        Vector3 firePos = GetFirePos();

        Instantiate(shotFx, firePos, fireLocator.transform.rotation);
        Instantiate(ammoObject, firePos, fireLocator.transform.rotation);
        FMODUnity.RuntimeManager.PlayOneShot("event:/TankShot", firePos);

        if (!bNoRecoil)
        {
            Vector3 fireImpulse = body.transform.up * 50f * body.mass;
            body.AddForceAtPosition(fireImpulse, firePos);
        }

        if (fireDelay >= 1f)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/TankReloadStart", body.gameObject);
        }

        reloadPercent = 0f;
    }

    public bool CanFire()
    {
        return reloadPercent >= 1f;
    }

    public void Destroy()
    {
        if (bGodmode || bIsDead)
        {
            return;
        }

        bIsDead = true;
        lastMoveInput = 0f;
        lastMoveInputVel = 0f;

        towerBody.isKinematic = false;

        Vector3 explosionCenter = body.transform.position;
        Vector3 forceShift = new Vector3();
        forceShift.x = Random.Range(-1.75f, 1.75f);
        forceShift.z = Random.Range(-1.75f, 1.75f);

        towerBody.AddExplosionForce(1000f * towerBody.mass, explosionCenter + forceShift, 2f);

        Instantiate(explosionFx, tower.transform.position, Quaternion.identity);
        Instantiate(towerFireFx, tower.transform);

        FMODUnity.RuntimeManager.PlayOneShot("event:/MissleHit", body.position);

        movementSound.release();
        tracksSound.release();
        towerSound.release();

        movementSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        tracksSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        towerSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        StopMoveFx();
    }

    public bool GetFireHitPos(out Vector3 pos)
    {
        RaycastHit hit;
        bool bRaycastRes = Physics.Raycast(GetFirePos(), GetFireDir(), out hit, Mathf.Infinity, ~(1 << 3));

        if (bRaycastRes)
        {
            pos = hit.point;
        }
        else
        {
            pos = new Vector3();
        }

        return bRaycastRes;
    }

    private void Drag()
    {
        if (Time.time - lastDragTime < dragDelay)
        {
            return;
        }

        lastDragTime = Time.time;

        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/TankDragEvent", body.gameObject);

        foreach (var fx in dragFx)
        {
            fx.Play(true);
        }
    }

    private void StartMoveFx()
    {
        foreach (var fx in moveFx)
        {
            fx.Play(true);
        }
    }

    private void StopMoveFx()
    {
        foreach (var fx in moveFx)
        {
            fx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private Vector3 GetFirePos()
    {
        return fireLocator.transform.position + body.velocity * Time.deltaTime * 2;
    }

    public Vector3 GetFireDir()
    {
        return fireLocator.transform.forward;
    }

    private void UpdateKinematics()
    {
        linearVelocity = (transform.position - lastPosition) / Time.deltaTime;
        angularVelocity = (transform.eulerAngles - lastRotation) / Time.deltaTime;
        lastPosition = transform.position;
        lastRotation = transform.eulerAngles;
    }

    private void UpdateWheelsPosition()
    {
        const float wheelRadius = 0.4f * 0.75f;
        const float tankWidth = 1.5f * 0.75f;

        float linVel = body.transform.worldToLocalMatrix.MultiplyVector(linearVelocity).z;
        float angVel = Mathf.Deg2Rad * angularVelocity.y;

        float linVelComponent = linVel / wheelRadius;
        float angVelComponent = (tankWidth * angVel) / wheelRadius;

        float vel1 = (linVelComponent + angVelComponent);
        float vel2 = (linVelComponent - angVelComponent);

        float wheelsVelocity = Mathf.Max(Mathf.Abs(vel1), Mathf.Abs(vel2));
        tracksVelocityAlpha = Mathf.Clamp(wheelsVelocity, 0f, 1f);

        float delta1 = Mathf.Rad2Deg * vel1 * Time.deltaTime;
        float delta2 = Mathf.Rad2Deg * vel2 * Time.deltaTime;

        wheelFLeft.transform.Rotate(new Vector3(1,0,0), delta1);
        wheelFRight.transform.Rotate(new Vector3(1,0,0), delta2);
        wheelBLeft.transform.Rotate(new Vector3(1,0,0), delta1);
        wheelBRight.transform.Rotate(new Vector3(1,0,0), delta2);
    }

    private void UpdateMovementSound()
    {
        float movementInputAlphaRaw = Mathf.Clamp(Mathf.Abs(lastMoveInput), 0f, 1f);
        movementInputAlpha = MathUtils.InterpConstantTo(movementInputAlpha, movementInputAlphaRaw, Time.deltaTime, 2f);

        movementSound.setParameterByName("IsMoving", movementInputAlpha);
        tracksSound.setParameterByName("TracksMoving", tracksVelocityAlpha);
        towerSound.setParameterByName("TowerMoving", towerVelocityAlpha);
    }

    private void UpdateReload()
    {
        if (reloadPercent < 1f)
        {
            reloadPercent += Time.deltaTime / fireDelay;

            if (reloadPercent >= 1f)
            {
                reloadPercent = 1f;
                if (fireDelay >= 1f)
                {
                    FMODUnity.RuntimeManager.PlayOneShotAttached("event:/TankReloadEnd", body.gameObject);
                }
            }
        }
    }

    private void UpdateCannonPosition()
    {
        if (bIsDead)
        {
            return;
        }

        float towerYawRequired = towerYaw - body.transform.localRotation.eulerAngles.y;

        Vector3 towerR = tower.transform.localRotation.eulerAngles;

        float yawError = Mathf.Abs(towerR.y - towerYawRequired);
        while (yawError >= 360f)
        {
            yawError -= 360f;
        }

        if (yawError > 0.001f)
        {
            towerVelocityAlpha = 1f;
        }
        else if (towerVelocityAlpha > 0f)
        {
            towerVelocityAlpha -= 2.5f / towerVelocityAlpha * Time.deltaTime;

            if (towerVelocityAlpha < 0)
            {
                towerVelocityAlpha = 0f;
            }
        }

        towerR.y = MathUtils.InterpAngleConstantTo(towerR.y, towerYawRequired, Time.deltaTime, towerRotationSpeed);
        tower.transform.localRotation = Quaternion.Euler(towerR);

        Vector3 cannonR = cannon.transform.localRotation.eulerAngles;
        float cannonPitchCl = MathUtils.ClampAngle(cannonPitch, cannonPitchMin, cannonPitchMax);
        cannonR.x = MathUtils.InterpAngleConstantTo(cannonR.x, cannonPitchCl, Time.deltaTime, cannonRotationSpeed);
        cannon.transform.localRotation = Quaternion.Euler(cannonR);
    }
}
