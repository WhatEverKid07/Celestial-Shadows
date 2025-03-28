using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchFunction : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerStats playerStatsScript;

    internal bool canUpdateWatchStat;
    private void Start()
    {
        playerStatsScript.watches.Add(gameObject);
        canUpdateWatchStat = true;

        Invoke(nameof(NoMoreUpdates), .01f);
    }

    private void NoMoreUpdates()
    {
        canUpdateWatchStat = false;
        Destroy(this);
    }
}
