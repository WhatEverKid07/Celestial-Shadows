using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KnifeAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private InputActionAsset controls;

    [SerializeField] private AnimationClip[] attackAnims;

    private InputAction inspect;
    private InputAction attack;

    private float timeThreshold = 0.2f; // Time window for rapid presses
    private float lastPressTime = -1f;
    private bool isRapidPressing = false;
    private bool isSpinning = false;
    private bool animCanPlay = true;

    public bool IsRapidPressing => isRapidPressing; // Public getter for the bool

    private void Awake()
    {
        inspect = controls.FindActionMap("Gun Controls").FindAction("Inspect");
        attack = controls.FindActionMap("Gun Controls").FindAction("Shoot");
    }
    void Start()
    {
        inspect.Enable();
        attack.Enable();
        inspect.performed += OnButtonPressed;
        inspect.canceled += OnButtonReleased;
        attack.performed += _ => StartAttackAnimation(); 
    }
    private void StartAttackAnimation()
    {
        if (animCanPlay && gameObject.activeInHierarchy)
        {
            StartCoroutine(SelectAttackAnim());
        }
    }
    private IEnumerator SelectAttackAnim()
    {
        if (!animCanPlay || attackAnims.Length == 0 || animator == null) yield break;

        animCanPlay = false;
        int randomIndex = Random.Range(0, attackAnims.Length);
        string selectedAnimation = attackAnims[randomIndex].name;
        float waitTime = attackAnims[randomIndex].length;

        animator.Play(selectedAnimation);
        yield return new WaitForSeconds(waitTime);
        animCanPlay = true;
    }

    private void OnDisable()
    {
        inspect.Disable();
        attack.Disable();
    }
    private void OnEnable()
    {
        inspect.Enable();
        attack.Enable();
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