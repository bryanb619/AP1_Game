using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("References"), SerializeField]
    private Transform orientation, playerCam;
    private Rigidbody rb;
    private Player_test pm;

    [SerializeField]
    private Animator Cam_anim;

    [Header("Dashing"), SerializeField]
    private float dashForce, dashUpwardForce, dashDuration;

    [Header("Cooldown"), SerializeField]
    private float dashCd, dashCdTimer;

    [Header("Input"), SerializeField]
    private KeyCode dashKey = KeyCode.LeftShift;

    [Header("Settings"), SerializeField]
    private bool useCameraFoward = true, allowAllDirections = true, disableGravity = false, resetVel = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<Player_test>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(dashKey))
        {
            Dash();
        }

        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }

    private void Dash()
    {
        if (dashCdTimer > 0) return;
        else
        {
            dashCdTimer = dashCd;
            Cam_anim.SetTrigger("dashed");
        }

        pm.dashing = true;

        Transform forwardT;

        if (useCameraFoward)
            forwardT = playerCam;
        else
            forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        if (resetVel)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        pm.dashing = false;

        if (disableGravity)
            rb.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else
            direction = forwardT.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }

}
