using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float xSens = 0.25f;
    public float ySens = 0.25f;

    public float cameraPitchMin = -10f;
    public float cameraPitchMax = 60f;

    public GameObject cameraRoot;
    public TankActor tank;

    private Camera playerCamera;
    private PlayerUi playerUi;
    private PlayerInput playerInput;

    private bool bDead = false;
    private bool bCanFire = false;
    private bool bInputDisabled = false;

    private void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        playerCamera = GetComponentInChildren<Camera>();
        playerUi = GetComponentInChildren<PlayerUi>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        cameraRoot.transform.Rotate(GetLookInput());
        Quaternion r = cameraRoot.transform.rotation;
        Vector3 cameraRotation = new Vector3(0, 0, 0);
        cameraRotation.x = MathUtils.ClampAngle(r.eulerAngles.x, cameraPitchMin, cameraPitchMax);
        cameraRotation.y = r.eulerAngles.y;
        cameraRoot.transform.rotation = Quaternion.Euler(cameraRotation);
        
        tank.towerYaw = cameraRotation.y;
        tank.cannonPitch = cameraRotation.x;

        tank.Move(GetMovementInput());
        tank.Rotate(GetRotationInput());

        cameraRoot.transform.position = tank.transform.position;

        if (tank.IsDead())
        {
            if (!bDead)
            {
                GameObject.FindGameObjectWithTag("WorldGenerator").GetComponent<WorldGenerator>().OnPlayerKilled();
                bDead = true;
            }
            playerUi.SetCrossairVisible(false);
        }
        else
        {
            Vector3 fireHitPos;
            bool bHitValid = tank.GetFireHitPos(out fireHitPos);

            playerUi.SetCrossairVisible(bHitValid);

            if (bHitValid)
            {
                playerUi.SetCrossairPos(playerCamera.WorldToScreenPoint(fireHitPos));
            }

            if (tank.CanFire())
            {
                if (!bCanFire)
                {
                    playerUi.SetReloading(false);
                    bCanFire = true;
                }
            }
            else
            {
                if (bCanFire)
                {
                    playerUi.SetReloading(true);
                    bCanFire = false;
                }
            }
        }
    }

    public void DisableInput()
    {
        bInputDisabled = true;
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (bInputDisabled)
        {
            return;
        }

        tank.Fire();
    }

    private Vector3 GetLookInput()
    {
        if (bInputDisabled)
        {
            return new Vector3(0, 0, 0);
        }

        float x = playerInput.actions["LookRight"].ReadValue<float>() * xSens;
        float y = -playerInput.actions["LookUp"].ReadValue<float>() * ySens;
        return new Vector3(y, x, 0);
    }

    private float GetMovementInput()
    {
        if (bInputDisabled)
        {
            return 0;
        }

        return playerInput.actions["MoveForward"].ReadValue<float>();
    }

    private float GetRotationInput()
    {
        if (bInputDisabled)
        {
            return 0;
        }

        return playerInput.actions["RotateRight"].ReadValue<float>();
    }
}
