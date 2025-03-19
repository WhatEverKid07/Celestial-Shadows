using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KnifeAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private InputActionAsset controls;

    private InputAction inspect;

    public float timeThreshold = 0.5f; // Time window for rapid presses
    private float lastPressTime = -1f;
    private bool isRapidPressing = false;
    private bool isSpinning = false;

    public bool IsRapidPressing => isRapidPressing; // Public getter for the bool
    void Start()
    {
        inspect = controls.FindActionMap("Gun Controls").FindAction("Inspect");
        inspect.Enable();
        inspect.performed += OnButtonPressed;
        inspect.canceled += OnButtonReleased;
    }
    private void OnEnable()
    {
        //inspect.Enable();
    }

    private void OnDisable()
    {
        inspect.Disable();
    }

    private void OnButtonPressed(InputAction.CallbackContext context)
    {
        if (!isSpinning)
        {
            animator.SetTrigger("StartSpin");
        }
        float currentTime = Time.time;

        // If the press is within the allowed timeframe, set bool to true
        if (lastPressTime > 0 && (currentTime - lastPressTime) <= timeThreshold)
        {
            isRapidPressing = true;
            animator.SetBool("LoopSpin?", true);
        }

        lastPressTime = currentTime;
        isSpinning = true;
    }

    private void OnButtonReleased(InputAction.CallbackContext context)
    {
        // Start a coroutine to check if no presses happen within the threshold
        StartCoroutine(CheckForStopPressing());
    }

    private IEnumerator CheckForStopPressing()
    {
        float stopTime = lastPressTime;
        yield return new WaitForSeconds(timeThreshold);

        // If no new press happened in the timeframe, reset the bool
        if (lastPressTime == stopTime)
        {
            isRapidPressing = false;
            animator.SetBool("LoopSpin?", false);
            isSpinning = false;
        }
    }
}