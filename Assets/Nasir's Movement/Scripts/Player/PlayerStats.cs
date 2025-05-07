using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class PlayerStats : MonoBehaviour
{
    [Header("PlayerScripts")]
    [SerializeField] private CharacterMovement characterMoveScript;
    [SerializeField] private PlayerExperience playerXpScript;
    [SerializeField] private NewARScript arScript;
    [SerializeField] private NewPistolScript pistolScript;
    [SerializeField] private NewShotgunScript shotgunScript;
    [SerializeField] private KnifeAnimation knifeScript;

    [Header("Lists")]
    [SerializeField] private List<GameObject> itemScripts;
    [SerializeField] private List<WatchFunction> watchFuncScript;
    [SerializeField] private List<CrocsFunction> crocsFuncScript;
    [SerializeField] private List<SyringeFunction> syringeFuncScript;
    [SerializeField] private List<ClipFunction> clipFuncScript;

    [Header("Stats")]
    //Syringe
    private float attackSpeed;

    private float arAttack;
    private float minArAttackSpeed = 5f;

    private float pistolAttack;
    private float minPistolAttackSpeed = 2f;

    private float shotgunAttack;
    private float minShotAttackSpeed = 7.5f;

    private float setKnifeAttackSpeed = 1f;
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

    private float arReload;
    private float arDelay;
    private float maxArReload = 2f;
    private float minArDelayTime = 3.2f;

    private float pistolReload;
    private float pistolDelay;
    private float maxPistolReload = 2f;
    private float minPistolDelayTime = 1.6f;

    private float shotReload;
    private float shotDelay;
    private float maxShotReload = 2f;
    private float minShotDelay = 2.72f;

    private float setKnifeReload;
    private float setGrenadeReload;

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

    [SerializeField] private Image clipImage;
    [SerializeField] private TextMeshProUGUI clipCount;

    [Header("ItemLists")]
    [SerializeField] internal List<GameObject> watches;
    [SerializeField] internal List<GameObject> crocs;
    [SerializeField] internal List<GameObject> syringes;
    [SerializeField] internal List<GameObject> clips;

    [Header("ItemValue")]
    [SerializeField] private float watchValue;
    [SerializeField] private float crocValue;
    [SerializeField] private float syringeValue;
    [SerializeField] private float clipValue;

    private void Start()
    {
        watchImage.enabled = false;
        crocsImage.enabled = false;
        syringeImage.enabled = false;
        clipImage.enabled = false;

        walkSpeed = characterMoveScript.walkSpeed;
        runSpeed = characterMoveScript.runSpeed;
        wallRunSpeed = characterMoveScript.wallRunForce;
        dashPower = characterMoveScript.dashPower;

        dashCooldown = characterMoveScript.dashTime;

        arAttack = arScript.fireRate;
        arReload = arScript.sAnimSpeed;
        arDelay = arScript.reloadTime;

        pistolAttack = pistolScript.fireRate;
        pistolReload = pistolScript.animSpeed;
        pistolDelay = pistolScript.reloadTime;

        shotgunAttack = shotgunScript.fireRate;
        shotReload = shotgunScript.sAnimSpeed;
        shotDelay = shotgunScript.reloadTime;

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
        HashSet<ClipFunction> currentClips = new HashSet<ClipFunction>();

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

            if (item.TryGetComponent(out ClipFunction clip))
            {
                currentClips.Add(clip);
            }
        }

        syringeFuncScript = currentSyringes.ToList();
        crocsFuncScript = currentCrocs.ToList();
        watchFuncScript = currentWatches.ToList();
        clipFuncScript = currentClips.ToList();

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

        for (int i = clipFuncScript.Count - 1; i >= 0; i--)
        {
            if (clipFuncScript[i].canUpdateClipStat)
            {
                UpdateReloadSpeed();
                clipImage.enabled = true;
            }
        }
    }

    private void UpdateCurrentGun()
    {
        if (arScript.isActiveAndEnabled)
        {
            attackSpeed = arAttack;
            reloadSpeed = arReload;
        }
        else if (pistolScript.isActiveAndEnabled)
        {
            attackSpeed = pistolAttack;
            reloadSpeed = pistolReload;
        }
        else if (shotgunScript.isActiveAndEnabled)
        {
            attackSpeed = shotgunAttack;
            reloadSpeed = shotReload;
        }
        else if (knifeScript.isActiveAndEnabled)
        {
            attackSpeed = setKnifeAttackSpeed;
            reloadSpeed = setKnifeReload;
        }
        else
        {
            attackSpeed = setGrenadeAttackSpeed;
            reloadSpeed = setGrenadeReload;
        }
    }

    private void UpdateAttackSpeed()
    {
        float attackIncrease = syringeValue * syringes.Count;

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

    private void UpdateReloadSpeed()
    {
        float reloadIncrease = clipValue * clips.Count;

        if (arReload < maxArReload)
        {
            float delayDecrease = ((arReload * 2f) - .4f);
            float newArReload = arReload + reloadIncrease;
            arReload = newArReload;

            if (clipValue > 0)
            {
                float newDelay = arDelay - delayDecrease;
                arDelay = newDelay;
            }   
        }

        if (pistolReload < maxPistolReload)
        {
            float delayDecrease = ((pistolReload * 2f) - .4f);
            float newPistolReload = pistolReload + reloadIncrease;
            pistolReload = newPistolReload;

            if (clipValue > 0)
            {
                float newDelay = pistolDelay - delayDecrease;
                pistolDelay = newDelay;
            }
        }

        if (shotReload < maxShotReload)
        {
            float delayDecrease = ((shotReload * 2f) - .4f);
            float newShotgunReload = shotReload + reloadIncrease;
            shotReload = newShotgunReload;

            if (clipValue > 0)
            {
                float newDelay = shotDelay - delayDecrease;
                shotDelay = newDelay;
            }
        }

        arScript.sAnimSpeed = arReload;
        arScript.reloadTime = arDelay;

        pistolScript.animSpeed = pistolReload;
        pistolScript.reloadTime = pistolDelay;

        shotgunScript.sAnimSpeed = shotReload;
        shotgunScript.reloadTime = shotDelay;
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
        reloadSpeedTxt.text = string.Format("Reload speed: " + reloadSpeed);

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

        if (syringes.Count > 1)
        {
            syringeCount.text = string.Format("x" + syringes.Count);
        }

        if (clips.Count > 1)
        {
            clipCount.text = string.Format("x" + clips.Count);
        }
    }
}
