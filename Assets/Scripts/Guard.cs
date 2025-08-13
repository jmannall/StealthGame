using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Guard : MonoBehaviour
{
    public NavigationPath path;
    private NavMeshAgent agent;

    private Transform player;
    private bool isChasingPlayer = false;
    static bool playerCaught = false;

    public float speed = 2.0f;
    [Min(0.0f)]
    public float distanceThreshold = 0.1f;

    public float sightDistance = 5.0f;

    private AudioSource audioSource;
    public AudioClip spottedSound;
    public AudioClip patrolSound;
    public AudioClip chaseSound;

    double timeLastSawPlayer = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCaught = false;
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        PlaySyncedAudio(patrolSound);
        agent.destination = path.GetNextWaypoint();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCaught)
        {
            audioSource.volume = 0.0f; // Mute the audio
            agent.speed = 0.0f; // Stop the guard
            return;
        }

        agent.speed = speed;
        SetNavigationTarget();
    }

    void SetNavigationTarget()
    {
        if (CanSeePlayer())
        {
            if (!isChasingPlayer)
                StartChase();
            timeLastSawPlayer = Time.realtimeSinceStartup;
            agent.destination = player.position;
            return;
        }

        if (isChasingPlayer && Time.realtimeSinceStartup - timeLastSawPlayer > 5.0f)
            StopChase();

        if (agent.remainingDistance <= distanceThreshold)
            agent.destination = path.GetNextWaypoint();
    }

    public static void OnPlayerCaught()
    {
        playerCaught = true;
    }

    void PlaySyncedAudio(AudioClip clip)
    {
        audioSource.clip = clip;

        double loopLength = audioSource.clip.samples / (double)audioSource.clip.frequency;
        double loopFraction = (AudioSettings.dspTime / loopLength) % 1.0;

        int newSample = Mathf.FloorToInt((float)(loopFraction * audioSource.clip.samples));
        audioSource.timeSamples = newSample;
        audioSource.Play();
    }
    void StartChase()
    {
        Debug.Log("Player spotted, chasing!");
        isChasingPlayer = true;
        sightDistance *= 2.0f; // Increase sight distance when chasing
        PlaySyncedAudio(chaseSound);
        audioSource.PlayOneShot(spottedSound);
    }

    void StopChase()
    {
        Debug.Log("Lost sight of player, returning to patrol.");
        isChasingPlayer = false;
        sightDistance /= 2.0f; // Reset sight distance when not chasing
        PlaySyncedAudio(patrolSound);
        agent.destination = path.GetCurrentWaypoint();
    }

    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > sightDistance)
            return false; // Player is too far away to be seen

        if (distanceToPlayer < 2.0f)
            return true; // Player is very close, consider it visible

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (!isChasingPlayer)
        {
            // Check if player is in front of the guard
            float inFrontOfGuard = Vector3.Dot(transform.forward, directionToPlayer);
            if (inFrontOfGuard < Mathf.Cos(Mathf.PI / 4)) // 45 degrees in front
                return false; // Player is behind the guard
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.Log("Player is visible");
                return true; // Player is visible
            }
        }
        return false; // Player is not visible
    }
}
