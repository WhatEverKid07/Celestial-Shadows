using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilOnClick : MonoBehaviour
{
    public float recoilForce;
    [Header("Make the curve domains from 0 to 1.")]
    [Header("Output value is degrees of deflection.")]
    [Header("Adjust time interval separately below.")]
    public AnimationCurve RecoilUp;
    public AnimationCurve RecoilRight;

    [Header("How long is entire recoil sequence?")]
    public float TimeInterval = 0.25f;

    [Header("How long is recovery sequence?")]
    public float RecoveryTime = 0.25f;

    [Header("Which object is having its .localRotation driven.")]
    public Transform RecoilPivot;

    private float recoiling;
    private float recovering;
    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = RecoilPivot.localRotation;
    }

    void DriveRecoil(float fraction)
    {
        float up = RecoilUp.Evaluate(fraction);
        float right = RecoilRight.Evaluate(fraction);

        if (fraction == 0)
        {
            up = 0;
            right = 0;
        }

        up *= recoilForce;
        up = -up;

        RecoilPivot.localRotation = Quaternion.Euler(up, right, 0);
    }

    void Update()
    {
        if (recoiling == 0 && recovering == 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                recoiling = Time.deltaTime;
            }
        }

        if (recoiling > 0)
        {
            float fraction = recoiling / TimeInterval;
            recoiling += Time.deltaTime;
            if (recoiling > TimeInterval)
            {
                recoiling = 0;
                fraction = 1;
                recovering = Time.deltaTime; // Start recovery phase
            }

            DriveRecoil(fraction);
        }
        else if (recovering > 0)
        {
            float fraction = recovering / RecoveryTime;
            recovering += Time.deltaTime;
            if (recovering > RecoveryTime)
            {
                recovering = 0;
                fraction = 1;
            }

            // Smoothly return to original rotation
            RecoilPivot.localRotation = Quaternion.Lerp(RecoilPivot.localRotation, originalRotation, fraction);
        }
    }
}
