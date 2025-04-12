using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.InputSystem;

public class KnifeAnimation : MonoBehaviour
{
    [SerializeField] private GunManagement gunManager;

    [SerializeField] private Animator animator;
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private AnimationClip[] attackAnims;
    //[SerializeField] private AudioSource knifeSlash;

    private InputAction inspect;
    private InputAction attack;

    private float timeThreshold = 0.2f;
    private float lastPressTime = -1f;
    private bool isRapidPressing = false;
    private bool isSpinning = false;
    private bool animCanPlay = true;
    private bool isAttacking = false;
    private bool isInspecting = false;

    public bool IsRapidPressing => isRapidPressing;

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
    private void StartAttackAnimation()
    {
        if (animCanPlay && gameObject.activeInHierarchy)
        {
            StartCoroutine(SelectAttackAnim());
        }
    }
    private IEnumerator SelectAttackAnim()
    {
        if (!animCanPlay || isAttacking || attackAnims.Length == 0 || animator == null || isInspecting) yield break;

        animCanPlay = false;
        isAttacking = true;
        gunManager.canSwitch = false;

        int randomIndex = Random.Range(0, attackAnims.Length);
        string selectedAnimation = attackAnims[randomIndex].name;
        animator.Play(selectedAnimation);
        AudioManager.instance.KnifeSlash();
        Debug.Log("attack");

        // Let animation event reset states
    }
    public void OnAttackAnimationEnd()
    {
        animCanPlay = true;
        isAttacking = false;
        gunManager.canSwitch = true;
    }

    private void OnButtonPressed(InputAction.CallbackContext context)
    {
        if (!isSpinning && !isAttacking)
        {
            animator.SetTrigger("StartSpin");
        }
        float currentTime = Time.time;
        if (lastPressTime > 0 && (currentTime - lastPressTime) <= timeThreshold && !isAttacking)
        {
            isRapidPressing = true;
            animator.SetBool("LoopSpin?", true);
        }

        lastPressTime = currentTime;
        isSpinning = true;
    }
    private void OnButtonReleased(InputAction.CallbackContext context)
    {
        StartCoroutine(CheckForStopPressing());
    }
    private IEnumerator CheckForStopPressing()
    {
        float stopTime = lastPressTime;
        yield return new WaitForSeconds(timeThreshold);

        if (lastPressTime == stopTime)
        {
            isRapidPressing = false;
            animator.SetBool("LoopSpin?", false);
            isSpinning = false;
        }
    }
    private void StartOfInspectAnim()
    {
        isInspecting = !isInspecting;
        gunManager.canSwitch = !gunManager.canSwitch;
        animCanPlay = !animCanPlay;
        //Debug.LogWarning(isInspecting);
    }
}