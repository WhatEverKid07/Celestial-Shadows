using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchFunction : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerStats playerStatsScript;

    private bool canUpdateWatchStat;
    private void Start()
    {
        playerStatsScript.watches.Add(gameObject);
        canUpdateWatchStat = true;

        Invoke(nameof(NoMoreUpdates), .1f);
    }

    private void NoMoreUpdates()
    {
        canUpdateWatchStat = false;
    }
}
