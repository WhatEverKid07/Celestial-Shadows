using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("PlayerScripts")]
    [SerializeField] private CharacterMovement characterMoveScript;
    [SerializeField] private WatchFunction watchFunctionScript;
    [SerializeField] private CrocsFunction crocsFunctionScript;

    [Header("Stats")]
    //Syringe
    private float attackSpeed;
    private float minAttackSpeed;

    //Crocs
    private float walkSpeed;
    private float maxWalkSpeed = 17f;
    private float runSpeed;
    private float maxRunSpeed = 20f;
    private float wallRunSpeed;
    private float maxWallRunSpeed = 700f;
    private float dashPower;
    private float maxDashPower = 8f;

    //Watch
    private float dashCooldown;
    private float minDashCooldown = .2f;

    //Vial
    private float damage;
    private float maxDamage;

    //Target
    private float critChance;
    private float maxCritChance;

    //Sword
    private float critDamage;
    private float maxCritDamage;

    //Gun Clip
    private float reloadSpeed;
    private float maxReloadSpeed;

    //XP
    private float xpMulti;
    private float maxXpMulti;

    [Header("StatsDisplay")]
    [SerializeField] private TextMeshProUGUI attackSpeedTxt;
    [SerializeField] private TextMeshProUGUI walkSpeedTxt;
    [SerializeField] private TextMeshProUGUI runSpeedTxt;
    [SerializeField] private TextMeshProUGUI wallRunSpeedTxt;
    [SerializeField] private TextMeshProUGUI dashPowerTxt;
    [SerializeField] private TextMeshProUGUI dashCooldownTxt;
    [SerializeField] private TextMeshProUGUI damageTxt;
    [SerializeField] private TextMeshProUGUI critChanceTxt;
    [SerializeField] private TextMeshProUGUI critDamageTxt;
    [SerializeField] private TextMeshProUGUI reloadSpeedTxt;
    [SerializeField] private TextMeshProUGUI xpMultiTxt;

    [Header("ItemLists")]
    [SerializeField] internal List<GameObject> watches;
    [SerializeField] internal List<GameObject> crocs;

    [Header("ItemValue")]
    [SerializeField] private float watchValue;
    [SerializeField] private float crocValue;

    private void Start()
    {
        crocsFunctionScript = FindAnyObjectByType<CrocsFunction>();
        watchFunctionScript = FindAnyObjectByType<WatchFunction>();

        walkSpeed = characterMoveScript.walkSpeed;
        runSpeed = characterMoveScript.runSpeed;
        wallRunSpeed = characterMoveScript.wallRunForce;
        dashPower = characterMoveScript.dashPower;

        dashCooldown = characterMoveScript.dashTime;
    }
    private void Update()
    {
        UpdateStatText();

        if (crocs.Count > 0 && crocsFunctionScript.canUpdateCrocStat)
        {
            UpdateSpeedStat();
        }

        if (watches.Count > 0 && watchFunctionScript.canUpdateWatchStat)
        {
            UpdateDashStat();
        }
    }

    private void UpdateSpeedStat()
    {
        float walkIncrease = crocValue * crocs.Count;
        float runIncrease = walkIncrease + .5f;
        float wallRunIncrease = (crocValue * crocs.Count) * 100;
        float dashIncrease = walkIncrease - .1f;

        if (characterMoveScript.walkSpeed <= maxWalkSpeed)
        {
            float newWalkSpeed = walkSpeed + walkIncrease;
            characterMoveScript.walkSpeed = newWalkSpeed;
        }
        else
        {
            characterMoveScript.walkSpeed = maxWalkSpeed;
        }

        if (characterMoveScript.runSpeed <= maxRunSpeed)
        {
            float newRunSpeed = runSpeed + runIncrease;
            characterMoveScript.runSpeed = newRunSpeed;
        }
        else
        {
            characterMoveScript.runSpeed = maxRunSpeed;
        }

        if(characterMoveScript.wallRunForce <= maxWallRunSpeed)
        {
            float newWallRunSpeed = wallRunSpeed + wallRunIncrease;
            characterMoveScript.wallRunForce = newWallRunSpeed;
        }
        else
        {
            characterMoveScript.wallRunForce = maxWallRunSpeed;
        }

        if(characterMoveScript.dashPower <= maxDashPower)
        {
            float newDashPower = dashPower + dashIncrease;
            characterMoveScript.dashPower = newDashPower;
        }
        else
        {
            characterMoveScript.dashPower = maxDashPower;
        }

        walkSpeed = characterMoveScript.walkSpeed;
        runSpeed = characterMoveScript.runSpeed;
        wallRunSpeed = characterMoveScript.wallRunForce;
        dashPower = characterMoveScript.dashPower;
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
        wallRunSpeedTxt.text = string.Format("Wall Run Speed: " + wallRunSpeed);
        dashPowerTxt.text = string.Format("Dash Power: " + dashPower);

        dashCooldownTxt.text = string.Format("Dash Cooldown: " + dashCooldown);

        attackSpeedTxt.text = string.Format("Attack speed: " + attackSpeed);
    }
}
