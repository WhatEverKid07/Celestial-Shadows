using UnityEngine;

public class CrocsFunction : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerStats playerStatsScript;

    private float rotationSpeed = 30f;
    private float bobbingAmplitude = 0.1f;
    private float bobbingFrequency = 1f;
    private float originalYPosition;

    internal bool canUpdateCrocStat;

    private void Start()
    {
        playerStatsScript = FindAnyObjectByType<PlayerStats>();

        originalYPosition = transform.position.y;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        float newY = originalYPosition + (Mathf.Cos(Time.time * bobbingFrequency) * bobbingAmplitude);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameObject.Find("PlayerPhys"))
        {
            playerStatsScript.crocs.Add(gameObject);
            canUpdateCrocStat = true;

            Invoke(nameof(NoMoreUpdates), .0001f);
        }
    }

    private void NoMoreUpdates()
    {
        canUpdateCrocStat = false;
        Destroy(gameObject);
    }
}
