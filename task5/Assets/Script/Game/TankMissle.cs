using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMissle : MonoBehaviour
{

    public float launchForce = 6000f;
    public GameObject explosionFx;

    FMOD.Studio.EventInstance flySound;

    private Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        Launch();
    }

    private void Launch()
    {
        Vector3 localForce = new Vector3(0, 0, launchForce);
        Vector3 force = body.transform.localToWorldMatrix.MultiplyVector(localForce);
        body.AddForce(force);

        flySound = FMODUnity.RuntimeManager.CreateInstance("event:/MissleLoop");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(flySound, body.transform, body);
        flySound.start();
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPos = collision.GetContact(0).point;

        foreach (ContactPoint pt in collision.contacts)
        {
            TankActor tank = pt.otherCollider.GetComponentInParent<TankActor>();
            if (tank)
            {
                tank.Destroy();
            }

            DestructibleObject destructible = pt.otherCollider.GetComponentInParent<DestructibleObject>();
            if (destructible)
            {
                destructible.Explode(pt.point, 400f, 5f);
            }
        }

        Instantiate(explosionFx, hitPos, Quaternion.identity);
        FMODUnity.RuntimeManager.PlayOneShot("event:/MissleHit", hitPos);

        flySound.release();
        flySound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        Destroy(gameObject);
    }
}
