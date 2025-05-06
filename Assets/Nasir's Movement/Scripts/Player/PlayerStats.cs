using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class PlayerStats : MonoBehaviour
{
    [Header("PlayerScripts")]
    [SerializeField] private CharacterMovement characterMoveScript;
    [SerializeField] private NewARScript arScript;
    [SerializeField] private NewPistolScript pistolScript;
    [SerializeField] private NewShotgunScript shotgunScript;
    [SerializeField] private KnifeAnimation knifeScript;
    [SerializeField] private List<GameObject> itemScripts;
    [SerializeField] private List<WatchFunction> watchFuncScript;
    [SerializeField] private List<CrocsFunction> crocsFuncScript;
    [SerializeField] private List<SyringeFunction> syringeFuncScript;

    [SerializeField] private PlayerExperience playerXpScript;

    [Header("Stats")]
    //Syringe
    private GameObject currentGun;

    private float attackSpeed;

    private float arAttack;
    private float minArAttackSpeed = 5f;

    private float pistolAttack;
    private float minPistolAttackSpeed = 2f;

    private float shotgunAttack;
    private float minShotAttackSpeed = 7.5f;

    private float knifeAttack;
    private float setKnifeAttackSpeed = 1f;

    private float grenadeAttack;
    private float setGrenadeAttackSpeed = 1.2f;

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
    private float xp;
    private int xpLvl;
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
    [SerializeField] private TextMeshProUGUI xpTxt;
    [SerializeField] private TextMeshProUGUI xpLvlTxt;
    [SerializeField] private TextMeshProUGUI xpMultiTxt;

    [Header("ItemUI")]
    [SerializeField] private Image watchImage;
    [SerializeField] private TextMeshProUGUI watchCount;

    [SerializeField] private Image crocsImage;
    [SerializeField] private TextMeshProUGUI crocsCount;

    [SerializeField] private Image syringeImage;
    [SerializeField] private TextMeshProUGUI syringeCount;

    [Header("ItemLists")]
    [SerializeField] internal List<GameObject> watches;
    [SerializeField] internal List<GameObject> crocs;
    [SerializeField] internal List<GameObject> syringe;

    [Header("ItemValue")]
    [SerializeField] private float watchValue;
    [SerializeField] private float crocValue;
    [SerializeField] private float syringeValue;

    private void Start()
    {
        watchImage.enabled = false;
        crocsImage.enabled = false;
        syringeImage.enabled = false;

        walkSpeed = characterMoveScript.walkSpeed;
        runSpeed = characterMoveScript.runSpeed;
        wallRunSpeed = characterMoveScript.wallRunForce;
        dashPower = characterMoveScript.dashPower;

        dashCooldown = characterMoveScript.dashTime;

        arAttack = arScript.fireRate;
        pistolAttack = pistolScript.fireRate;
        shotgunAttack = shotgunScript.fireRate;

        xp = playerXpScript.currentXp;
        xpLvl = playerXpScript.xpLvl;

    }
    private void Update()
    {
        UpdateItemDrops();
    }

    private void LateUpdate()
    {
        UpdateStatText();
        UpdateItemUIText();
        UpdateCurrentGun();

        xp = playerXpScript.currentXp;
        xpLvl = playerXpScript.xpLvl;

    }

    private void UpdateItemDrops()
    {
        itemScripts = GameObject.FindGameObjectsWithTag("Item").ToList();

        HashSet<SyringeFunction> currentSyringes = new HashSet<SyringeFunction>();
        HashSet<CrocsFunction> currentCrocs = new HashSet<CrocsFunction>();
        HashSet<WatchFunction> currentWatches = new HashSet<WatchFunction>();

        foreach (GameObject item in itemScripts)
        {
            if (item.TryGetComponent(out SyringeFunction syringe))
            {
                currentSyringes.Add(syringe);
            }

            if (item.TryGetComponent(out CrocsFunction crocs))
            {
                currentCrocs.Add(crocs);
            }

            if (item.TryGetComponent(out WatchFunction watch))
            {
                currentWatches.Add(watch);
            }
        }

        syringeFuncScript = currentSyringes.ToList();
        crocsFuncScript = currentCrocs.ToList();
        watchFuncScript = currentWatches.ToList();

        for (int i = syringeFuncScript.Count - 1; i >= 0; i--)
        {
            if (syringeFuncScript[i].canUpdateSyringeStat)
            {
                UpdateAttackSpeed();
                syringeImage.enabled = true;
            }
        }

        for (int i = crocsFuncScript.Count - 1; i >= 0; i--)
        {
            if (crocsFuncScript[i].canUpdateCrocStat)
            {
                UpdateSpeedStat();
                crocsImage.enabled = true;
            }
        }

        for (int i = watchFuncScript.Count - 1; i >= 0; i--)
        {
            if (watchFuncScript[i].canUpdateWatchStat)
            {
                UpdateDashStat();
                watchImage.enabled = true;
            }
        }
    }

    private void UpdateCurrentGun()
    {
        if (arScript.isActiveAndEnabled)
        {
            attackSpeed = arAttack;
        }
        else if (pistolScript.isActiveAndEnabled)
        {
            attackSpeed = pistolAttack;
        }
        else if (shotgunScript.isActiveAndEnabled)
        {
            attackSpeed = shotgunAttack;
        }
        else if (knifeScript.isActiveAndEnabled)
        {
            attackSpeed = setKnifeAttackSpeed;
        }
        else
        {
            attackSpeed = setGrenadeAttackSpeed;
        }
    }

    private void UpdateAttackSpeed()
    {
        float attackIncrease = syringeValue * syringe.Count;

        if (arAttack > minArAttackSpeed)
        {
            float newArAttack = arAttack - attackIncrease;
            arAttack = newArAttack;
        }

        if (pistolAttack > minPistolAttackSpeed)
        {
            float newPistolAttack = pistolAttack - attackIncrease;
            pistolAttack = newPistolAttack;
        }

        if (shotgunAttack > minShotAttackSpeed)
        {
            float newShotgunAttack = shotgunAttack - attackIncrease;
            shotgunAttack = newShotgunAttack;
        }
    }

    private void UpdateSpeedStat()
    {
        float walkIncrease = crocValue * crocs.Count;
        float wallRunIncrease = (crocValue * crocs.Count) * 100;

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
            if (crocValue != 0)
            {
                float runIncrease = walkIncrease + .5f;
                float newRunSpeed = runSpeed + runIncrease;
                characterMoveScript.runSpeed = newRunSpeed;
            }
        }
        else
        {
            characterMoveScript.runSpeed = maxRunSpeed;
        }

        if (characterMoveScript.wallRunForce <= maxWallRunSpeed)
        {
            float newWallRunSpeed = wallRunSpeed + wallRunIncrease;
            characterMoveScript.wallRunForce = newWallRunSpeed;
        }
        else
        {
            characterMoveScript.wallRunForce = maxWallRunSpeed;
        }

        if (characterMoveScript.dashPower <= maxDashPower)
        {
            if (crocValue != 0)
            {
                float dashIncrease = walkIncrease - .1f;
                float newDashPower = dashPower + dashIncrease;
                characterMoveScript.dashPower = newDashPower;
            }
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

        xpTxt.text = string.Format("Xp: " +  xp);
        xpLvlTxt.text = string.Format("XpLvl: " + xpLvl);
    }

    private void UpdateItemUIText()
    {
        if (watches.Count > 1)
        {
            watchCount.text = string.Format("x" + watches.Count);
        }
        
        if (crocs.Count > 1)
        {
            crocsCount.text = string.Format("x" + crocs.Count);
        }

        if (syringe.Count > 1)
        {
            syringeCount.text = string.Format("x" + syringe.Count);
        }
    }
}
