using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public enum PeopleState
{
    Idle,
    Walking,
    Running,
    Reading,
    Talking,
    Following
}

public class PeopleBehaviour : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsReading = Animator.StringToHash("IsReading");

    [Header("AI Settings")] [SerializeField]
    private Transform head;

    [SerializeField] private PeopleState initialState;

    [FormerlySerializedAs("angleView")] [SerializeField]
    private float defaultAngleView = 45f;

    [SerializeField] private float runningAngleView = 360f;

    [FormerlySerializedAs("distanceView")] [SerializeField]
    private float defaulDistanceView = 5f;

    [SerializeField] private float runningDistanceView = 15f;
    [SerializeField] [Range(0, 1)] private float chanceToGoIdle = 0.3f;
    [SerializeField] Vector2 randomIdleTime = new Vector2(2, 5);
    [SerializeField] private float readingTime = 20f;
    [SerializeField] private float playerPresenceDistance = 3f;

    [Header("Locomotion Settings")] [SerializeField]
    private float walkingSpeed = 1f;

    [SerializeField] private float runningSpeed = 3f;
    [SerializeField] private PatrolPoints patrolPoints;
    [SerializeField] private float defaultStoppingDistance;
    [SerializeField] private float followingStopDistance;

    [Header("Quest Settings")] [SerializeField]
    private GameObject canvas;

    [SerializeField] private TextMeshPro canvasText;


    private PeopleState currentState;
    private float currentDistanceView;
    private float currentAngleView;
    private Vector3 playerLastPosition;
    private Transform player; // Reference to the player
    private bool isWaiting = false;
    private static PlayerController playerController;

    public PeopleState CurrentState
    {
        get => currentState;
    }

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private float timer;
    private float idleTime;
    private static bool HasTarget;

    private DeweyCategory targetBookSection = DeweyCategory.None;

    private Dictionary<DeweyCategory, Bookshelf[]> bookSections = new Dictionary<DeweyCategory, Bookshelf[]>();
    private Collider[] bookSectionColliders;

    private void Start()
    {
        FillActiveBookSectionsMap();
        currentDistanceView = defaulDistanceView;
        currentAngleView = defaultAngleView;
        player = GameObject.FindGameObjectWithTag("MainCamera").transform;
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentState = initialState;
    }

    private void FillActiveBookSectionsMap()
    {
        List<DeweyCategory> categories = new List<DeweyCategory>();
        List<Bookshelf> bookshelves = new List<Bookshelf>();
        bookSections = GameManager.Instance.Bookshelfs
            .GroupBy(book => book.Category)
            .ToDictionary(group => group.Key, group => group.ToArray());
        // foreach (var bookshelf in GameManager.Instance.Bookshelfs)
        // {
        //     if (categories.Contains(bookshelf.Category)) continue;
        //     bookshelves.Add(bookshelf);
        //     categories.Add(bookshelf.Category);
        // }
        //
        // for (int i = 0; i < categories.Count; i++)
        // {
        //     bookSections.Add(categories[i], bookshelves[i]);
        // }
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
                animator.SetFloat(Speed, navMeshAgent.velocity.magnitude);
                Running();
                break;
            case PeopleState.Reading:
                Reading();
                break;
            case PeopleState.Talking:
                break;
            case PeopleState.Following:
                navMeshAgent.speed = runningSpeed;
                animator.SetFloat(Speed, navMeshAgent.velocity.magnitude);
                Following();
                break;
            default:
                break;
        }
    }

    private void Idleling()
    {
        if (IsPlayerInSight() && !HasTarget)
        {
            ChasePlayer();
        }
        else if (isWaiting)
        {
            timer += Time.deltaTime;
            if (timer >= idleTime)
            {
                isWaiting = false;
                timer = 0;
                SetState(PeopleState.Walking);
            }
        }
    }

    private void Walking()
    {
        if (IsPlayerInSight() && !HasTarget)
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
                GoToIdle();
            }
        }
        else if (playerLastPosition != Vector3.zero)
        {
            navMeshAgent.SetDestination(playerLastPosition);
        }
        else if (patrolPoints)
        {
            GoToIdle();
        }

        if (Vector3.Distance(transform.position, navMeshAgent.destination) <= navMeshAgent.stoppingDistance)
        {
            GoToIdle();
        }
    }

    private void GoToIdle()
    {
        AudioManager.Instance.PlayMusic(MusicType.StartGame);
        HasTarget = false;
        playerLastPosition = Vector3.zero;
        WaitRandomTime();
    }

    private void Following()
    {
        // TODO: disable climb features
        navMeshAgent.SetDestination(player.position);
    }

    private void Reading()
    {
        if (timer >= readingTime)
        {
            timer = 0;
            HasTarget = false;
            animator.SetBool(IsReading, false);
            if (canvasText) canvasText.text = "";
            if (canvas) canvas.SetActive(false);
            SetState(PeopleState.Walking);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentState == PeopleState.Running && other.transform == player)
        {
            Debug.Log("Player entered the trigger");
            if (!playerController) playerController = player.GetComponentInParent<PlayerController>();
            if (playerController) playerController.SetPlayerHandInteractors(false);
            currentDistanceView = defaulDistanceView;
            currentAngleView = defaultAngleView;
            navMeshAgent.speed = 0;
            animator.SetFloat(Speed, 0);
            isWaiting = false;
            ShowDialogCanvas();
        }
        else if (currentState == PeopleState.Following && ContainsSectionColliders(other))
        {
            AudioManager.Instance.PlayMusic(MusicType.StartGame);
            if (canvasText) canvasText.text = "Thank you!";
            if (playerController) playerController.SetPlayerHandInteractors(true);
            bookSectionColliders = null;
            animator.SetBool(IsReading, true);
            animator.SetFloat(Speed, 0);
            timer = 0;
            navMeshAgent.stoppingDistance = defaultStoppingDistance;
            SetState(PeopleState.Reading);
        }
    }

    private bool ContainsSectionColliders(Collider other)
    {
        foreach (var bookSectionCollider in bookSectionColliders)
        {
            if (bookSectionCollider == other)
            {
                return true;
            }
        }

        return false;
    }

    private void ShowDialogCanvas()
    {
        DeweyCategory category = GetRandomBookSection();
        bookSectionColliders = bookSections[category].Select(bookshelf => bookshelf.GetComponent<Collider>()).ToArray();
        targetBookSection = category;
        if (canvasText) canvasText.text = $"Can you take me to the {Bookshelf.signTexts[category]} section?";
        if (canvas) canvas.SetActive(true);
        Debug.Log($"Can you take me to the section {Bookshelf.signTexts[category]}?");
        navMeshAgent.stoppingDistance = followingStopDistance;
        SetState(PeopleState.Following);
    }

    private DeweyCategory GetRandomBookSection()
    {
        DeweyCategory category;
        do
        {
            category = (DeweyCategory)UnityEngine.Random.Range(0, Enum.GetValues(typeof(DeweyCategory)).Length);
        } while (!bookSections.ContainsKey(category));

        return category;
    }


    private bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (directionToPlayer.magnitude <= currentDistanceView && angleToPlayer <= defaultAngleView / 2)
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
        currentDistanceView = defaulDistanceView;
        currentAngleView = defaultAngleView;
        isWaiting = true;
        timer = 0;
        idleTime = UnityEngine.Random.Range(randomIdleTime.x, randomIdleTime.y);
        SetState(PeopleState.Idle);
    }

    private void ChasePlayer()
    {
        AudioManager.Instance.PlayMusic(MusicType.Running);
        HasTarget = true;
        currentDistanceView = runningDistanceView;
        currentAngleView = runningAngleView;
        navMeshAgent.speed = runningSpeed;
        SetState(PeopleState.Running);
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