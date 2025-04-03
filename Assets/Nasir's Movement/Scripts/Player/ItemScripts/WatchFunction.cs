using UnityEngine;

public class WatchFunction : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerStats playerStatsScript;

    [Header("Watch")]
    private GameObject clock;
    private GameObject chain;
    private GameObject time;

    private float rotationSpeed = 30f;
    private float bobbingAmplitude = 0.1f;  
    private float bobbingFrequency = 1f;   
    private float originalYPosition;

    internal bool canUpdateWatchStat;

    private void Start()
    {
        playerStatsScript = FindAnyObjectByType<PlayerStats>();

        clock = GameObject.Find("clock");
        chain = GameObject.Find("chain");
        time = GameObject.Find("time");

        originalYPosition = transform.position.y;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        float newY = originalYPosition + (Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameObject.Find("PlayerPhys"))
        {
            playerStatsScript.watches.Add(gameObject);
            canUpdateWatchStat = true;

            clock?.SetActive(false);
            chain?.SetActive(false);
            time?.SetActive(false);

            Invoke(nameof(NoMoreUpdates), .0001f);
        }
    }

    private void NoMoreUpdates()
    {
        canUpdateWatchStat = false;
        Destroy(this);
    }
}
