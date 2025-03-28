using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("PlayerScripts")]
    [SerializeField] private CharacterMovement characterMoveScript;
    [SerializeField] private WatchFunction watchFunctionScript;

    [Header("Stats")]
    //Syringe
    private float attackSpeed;

    //Crocs
    private float walkSpeed;
    private float runSpeed;
    private float dashPower;

    //Watch
    private float dashCooldown;

    //Vial
    private float damage;

    //Target
    private float critChance;

    //Sword
    private float critDamage;

    //Gun Clip
    private float reloadSpeed;

    //XP
    private float xpMulti;

    [Header("StatsDisplay")]
    [SerializeField] private TextMeshProUGUI attackSpeedTxt;
    [SerializeField] private TextMeshProUGUI walkSpeedTxt;
    [SerializeField] private TextMeshProUGUI runSpeedTxt;
    [SerializeField] private TextMeshProUGUI dashPowerTxt;
    [SerializeField] private TextMeshProUGUI dashCooldownTxt;
    [SerializeField] private TextMeshProUGUI damageTxt;
    [SerializeField] private TextMeshProUGUI critChanceTxt;
    [SerializeField] private TextMeshProUGUI critDamageTxt;
    [SerializeField] private TextMeshProUGUI reloadSpeedTxt;
    [SerializeField] private TextMeshProUGUI xpMultiTxt;

    [Header("ItemLists")]
    [SerializeField] internal List<GameObject> watches;

    [Header("ItemValue")]
    [SerializeField] private float watchValue;

    private void Start()
    {
        watchFunctionScript = FindAnyObjectByType<WatchFunction>();
        walkSpeed = characterMoveScript.walkSpeed;
        runSpeed = characterMoveScript.runSpeed;
        dashCooldown = characterMoveScript.dashTime;
    }
    private void Update()
    {
        UpdateStatText();

        if (watches.Count > 0 && watchFunctionScript.canUpdateWatchStat)
        {
            UpdateDashStat();
        }
    }

    private void UpdateDashStat()
    {
        float dashReduction = watchValue * watches.Count;
        float newDashCooldown = dashCooldown - dashReduction;
        characterMoveScript.dashTime = newDashCooldown;
        dashCooldown = characterMoveScript.dashTime;
    }

    private void UpdateStatText()
    {
        walkSpeedTxt.text = string.Format("Walk speed: " + walkSpeed);
        runSpeedTxt.text = string.Format("Run Speed: " + runSpeed);
        dashCooldownTxt.text = string.Format("Dash Cooldown: " + dashCooldown);
        attackSpeedTxt.text = string.Format("Attack speed: " + attackSpeed);
    }
}
