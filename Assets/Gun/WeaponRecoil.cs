using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRecoil : MonoBehaviour
{
    public Vector3 upRecoil;
    Vector3 originalRotation;

    private void Start()
    {
        originalRotation = transform.localEulerAngles;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddRecoil();
        }
    }

    public void AddRecoil()
    {
        transform.localEulerAngles += upRecoil;
    }

    public void StopRecoil()
    {
        transform.localEulerAngles = originalRotation;
    }
}