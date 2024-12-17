using System;
using UnityEngine;
using UnityEngine.AI;


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
    [Header("AI Settings")]
    [SerializeField] private PeopleState initialState;
    [SerializeField] private float angleView = 45f;
    [SerializeField] private float distanceView = 5f;
    [SerializeField] [Range(0,1)] private float chanceToGoIdle = 0.3f;
    [SerializeField] Vector2 randomIdleTime = new Vector2(2, 5);
    [Header("Locomotion Settings")] 
    [SerializeField] private float walkingSpeed = 1f;
    [SerializeField] private float runningSpeed = 3f;
    [SerializeField] private PatrolPoints patrolPoints;
    private PeopleState currentState;
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

    private void Start()
    {
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
                animator.SetFloat(Speed, 0);
                EnableMovement(false);
                Idleling();
                break;
            case PeopleState.Walking:
                animator.SetFloat(Speed, walkingSpeed);
                EnableMovement(true);
                navMeshAgent.speed = walkingSpeed;
                Walking();
                break;
            case PeopleState.Running:
                animator.SetFloat(Speed, runningSpeed);
                EnableMovement(true);
                navMeshAgent.speed = runningSpeed;
                break;
            case PeopleState.Reading:
                animator.SetFloat(Speed, 0);
                EnableMovement(false);
                break;
            case PeopleState.Talking:
                animator.SetFloat(Speed, 0);
                EnableMovement(false);
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
        if (IsPlayerInSight())
        {
            currentState = PeopleState.Running;
            playerLastPosition = player.position;
            navMeshAgent.SetDestination(playerLastPosition);
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
        if (patrolPoints)
        {
            if (Vector3.Distance(transform.position, patrolPoints.GetCurrentPatrolPoint().position) <= navMeshAgent.stoppingDistance)
            {
                patrolPoints.GetNextPatrolPoint();
                if (UnityEngine.Random.value <= chanceToGoIdle)
                {
                    isWaiting = true;
                    timer = 0;
                    idleTime = UnityEngine.Random.Range(randomIdleTime.x, randomIdleTime.y);
                    currentState = PeopleState.Idle;
                    return;
                }
            }
            navMeshAgent.SetDestination(patrolPoints.GetCurrentPatrolPoint().position);
        }
        if (IsPlayerInSight())
        {
            currentState = PeopleState.Running;
            playerLastPosition = player.position;
            navMeshAgent.SetDestination(playerLastPosition);
        }
    }
    
    private bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (directionToPlayer.magnitude <= distanceView && angleToPlayer <= angleView / 2)
        {
            if (Physics.Linecast(transform.position, player.position, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
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