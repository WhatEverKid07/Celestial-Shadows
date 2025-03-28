using UnityEngine;

public class WatchFunction : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerStats playerStatsScript;

    [Header("Watch")]
    private GameObject clock;
    private GameObject chain;
    private GameObject time;

    internal bool canUpdateWatchStat;

    private void Start()
    {
        playerStatsScript = FindAnyObjectByType<PlayerStats>();

        clock = GameObject.Find("clock");
        chain = GameObject.Find("chain");
        time = GameObject.Find("time");
    }

    private void Update()
    {
        //transform.Rotate(transform.position, 180);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameObject.Find("PlayerPhys"))
        {
            playerStatsScript.watches.Add(gameObject);
            canUpdateWatchStat = true;

            clock.SetActive(false);
            chain.SetActive(false);
            time.SetActive(false);

            Invoke(nameof(NoMoreUpdates), .001f);
        }
    }

    private void NoMoreUpdates()
    {
        canUpdateWatchStat = false;
        Destroy(this);
    }
}
