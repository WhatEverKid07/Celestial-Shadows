using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLightRechargingScript : MonoBehaviour
{
    [SerializeField] private Target playerTargetScript;
    [SerializeField] private Slider lightRecharger;
    [SerializeField] private int maxLight;
    [SerializeField] private float lightLose;
    [SerializeField] private float lightGain;
    [SerializeField] private string tagOfLightRecharger;

    private float currentLight;
    private bool inLight;

    void Start()
    {
        inLight = false;
        currentLight = 0;
        lightRecharger.value = currentLight;
        lightRecharger.maxValue = maxLight;
    }

    private void FixedUpdate()
    {
        if (lightRecharger != null) { UpdateLightBar(); }
        if (inLight && currentLight < maxLight) { currentLight += lightGain; }
        if (!inLight && currentLight > 0) { currentLight -= lightLose; playerTargetScript.canLoseDamage = false; }
        if (!inLight && currentLight <= 0) { playerTargetScript.canLoseDamage = true; }
    }
    private void UpdateLightBar() { lightRecharger.value = currentLight; }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("Collision");
        if (other.CompareTag(tagOfLightRecharger))
        {
            inLight = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagOfLightRecharger))
        {
            inLight = false;
        }
    }
}