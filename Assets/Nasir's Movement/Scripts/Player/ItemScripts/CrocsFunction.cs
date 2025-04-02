using UnityEngine;

public class CrocsFunction : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerStats playerStatsScript;

    [Header("Croc")]
    private GameObject croc1;
    private GameObject croc2;

    private float rotationSpeed = 30f;
    private float bobbingAmplitude = 0.1f;
    private float bobbingFrequency = 1f;
    private float originalYPosition;

    internal bool canUpdateCrocStat;

    private void Start()
    {
        playerStatsScript = FindAnyObjectByType<PlayerStats>();

        croc1 = GameObject.Find("Croc1");
        croc2 = GameObject.Find("Croc2");

        originalYPosition = transform.position.y;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        float newY = originalYPosition + (Mathf.Cos(Time.time * bobbingFrequency) * bobbingAmplitude);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        Debug.Log(transform.position.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameObject.Find("PlayerPhys"))
        {
            playerStatsScript.crocs.Add(gameObject);
            canUpdateCrocStat = true;

            croc1?.SetActive(false);
            croc2?.SetActive(false);

            //Invoke(nameof(NoMoreUpdates), .0001f);
        }
    }

    private void NoMoreUpdates()
    {
        canUpdateCrocStat = false;
        Destroy(this);
    }
}
