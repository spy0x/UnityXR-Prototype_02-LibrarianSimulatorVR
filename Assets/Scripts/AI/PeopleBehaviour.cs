using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public enum PeopleState
{
    Idle,
    Walking,
    Running,
    Reading,
    Talking,
    Searching
}

public class PeopleBehaviour : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");

    [Header("AI Settings")] [SerializeField]
    private Transform head;

    [SerializeField] private PeopleState initialState;
    [SerializeField] private float angleView = 45f;
    [SerializeField] private float runningAngleView = 360f;
    [SerializeField] private float distanceView = 5f;
    [SerializeField] private float runningDistanceView = 15f;
    [SerializeField] [Range(0, 1)] private float chanceToGoIdle = 0.3f;
    [SerializeField] Vector2 randomIdleTime = new Vector2(2, 5);
    [SerializeField] private float playerPresenceDistance = 3f;

    [Header("Locomotion Settings")] [SerializeField]
    private float walkingSpeed = 1f;

    [SerializeField] private float runningSpeed = 3f;
    [SerializeField] private PatrolPoints patrolPoints;
    private PeopleState currentState;
    private float currentDistanceView;
    private float currentAngleView;
    private Vector3 playerLastPosition;
    private Transform player; // Reference to the player
    private bool isWaiting = false;

    public PeopleState CurrentState
    {
        get => currentState;
    }

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private float timer;
    private float idleTime;
    private static bool hasTarget;

    private void Start()
    {
        currentDistanceView = distanceView;
        currentAngleView = angleView;
        player = GameObject.FindGameObjectWithTag("MainCamera").transform;
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentState = initialState;
    }

    private void Update()
    {
        switch (currentState)
        {
            case PeopleState.Idle:
                navMeshAgent.speed = 0;
                animator.SetFloat(Speed, 0);
                Idleling();
                break;
            case PeopleState.Walking:
                navMeshAgent.speed = walkingSpeed;
                animator.SetFloat(Speed, walkingSpeed);
                Walking();
                break;
            case PeopleState.Running:
                navMeshAgent.speed = runningSpeed;
                animator.SetFloat(Speed, runningSpeed);
                Running();
                break;
            case PeopleState.Reading:
                animator.SetFloat(Speed, 0);
                EnableMovement(false);
                break;
            case PeopleState.Talking:
                navMeshAgent.speed = 0;
                animator.SetFloat(Speed, 0);
                break;
            case PeopleState.Searching:
                animator.SetFloat(Speed, 0);
                EnableMovement(false);
                break;
            default:
                break;
        }
    }

    private void Idleling()
    {
        if (IsPlayerInSight() && !hasTarget)
        {
            ChasePlayer();
        }
        else if (isWaiting)
        {
            timer += Time.deltaTime;
            if (timer >= idleTime)
            {
                isWaiting = false;
                currentState = PeopleState.Walking;
            }
        }
    }

    private void Walking()
    {
        if (IsPlayerInSight() && !hasTarget)
        {
            ChasePlayer();
        }
        else if (patrolPoints)
        {
            if (Vector3.Distance(transform.position, patrolPoints.GetCurrentPatrolPoint().position) <=
                navMeshAgent.stoppingDistance)
            {
                patrolPoints.GetNextPatrolPoint();
                if (UnityEngine.Random.value <= chanceToGoIdle)
                {
                    WaitRandomTime();
                    return;
                }
            }

            navMeshAgent.SetDestination(patrolPoints.GetCurrentPatrolPoint().position);
        }
    }



    private void Running()
    {
        if (Vector3.Distance(transform.position, player.position) <= playerPresenceDistance || IsPlayerInSight())
        {
            NavMeshPath path = new NavMeshPath();
            if (navMeshAgent.CalculatePath(player.position, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    playerLastPosition = player.position;
                    navMeshAgent.SetDestination(player.position);
                }
                else if (playerLastPosition != Vector3.zero)
                {
                    navMeshAgent.SetDestination(playerLastPosition);
                }
            }
            else if (playerLastPosition != Vector3.zero)
            {
                navMeshAgent.SetDestination(playerLastPosition);
            }
            else if (patrolPoints)
            {
                WaitRandomTime();
            }
        }
        else if (playerLastPosition != Vector3.zero)
        {
            navMeshAgent.SetDestination(playerLastPosition);
        }
        else if (patrolPoints)
        {
            WaitRandomTime();
        }
        if (Vector3.Distance(transform.position, navMeshAgent.destination) <= navMeshAgent.stoppingDistance)
        {
            WaitRandomTime();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentState == PeopleState.Running && other.transform == player)
        {
            Debug.Log("Player entered the trigger");
            isWaiting = false;
            SetState(PeopleState.Talking);
        }
    }

    private bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (directionToPlayer.magnitude <= currentDistanceView && angleToPlayer <= angleView / 2)
        {
            if (Physics.Linecast(head.position, player.position, out RaycastHit hit))
            {
                return hit.transform == player;
            }
        }

        return false;
    }

    private void WaitRandomTime()
    {
        currentDistanceView = distanceView;
        currentAngleView = angleView;
        isWaiting = true;
        timer = 0;
        idleTime = UnityEngine.Random.Range(randomIdleTime.x, randomIdleTime.y);
        currentState = PeopleState.Idle;
    }
    private void ChasePlayer()
    {
        currentDistanceView = runningDistanceView;
        currentAngleView = runningAngleView;
        currentState = PeopleState.Running;
    }

    public void EnableMovement(bool state)
    {
        navMeshAgent.isStopped = !state;
    }

    public void SetState(PeopleState newState)
    {
        currentState = newState;
    }
}