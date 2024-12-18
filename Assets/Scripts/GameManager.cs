using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] Bookshelf[] bookshelves;
    [SerializeField] float booksToOrder = 5;
    [SerializeField] private Transform[] bookSpawnPoints;
    [Header("Prefabs")] [SerializeField] private GameObject[] books000General = new GameObject[0];
    [SerializeField] private GameObject[] books100Philosophy = new GameObject[0];
    [SerializeField] private GameObject[] books200Religion = new GameObject[0];
    [SerializeField] private GameObject[] books300SocialScience = new GameObject[0];
    [SerializeField] private GameObject[] books400Language = new GameObject[0];
    [SerializeField] private GameObject[] books500PureScience = new GameObject[0];
    [SerializeField] private GameObject[] books600Technology = new GameObject[0];
    [SerializeField] private GameObject[] books700Arts = new GameObject[0];
    [SerializeField] private GameObject[] books800Literature = new GameObject[0];
    [SerializeField] private GameObject[] books900History = new GameObject[0];

    private Dictionary<DeweyCategory, GameObject[]> booksByCategory = new Dictionary<DeweyCategory, GameObject[]>();

    private int currentSpawnPoint = 0;
    private float goodBooks = 0;
    private float badBooks = 0;
    private float totalBooks = 0;
    private List<Bookshelf> bookshelvesWithBooks = new List<Bookshelf>();

    public static GameManager Instance;
    public Bookshelf[] Bookshelfs => bookshelves;

    private void OnEnable()
    {
        Bookshelf.OnInsertedBook += TryEndGame;
    }

    private void OnDisable()
    {
        Bookshelf.OnInsertedBook -= TryEndGame;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    private void Start()
    {
        FillBooksByCategory();
        ActivateBookshelves();
    }

    private void ActivateBookshelves()
    {
        for (int i = 0; i < bookshelves.Length; i++)
        {
            totalBooks++;
            if (totalBooks > booksToOrder) break;
            bookshelvesWithBooks.Add(GetRandomBookshelf());
            bookshelvesWithBooks[i].ChooseRandomSocket();
            SpawnBookToOrder(bookshelvesWithBooks[i].Category);
        }
    }

    private Bookshelf GetRandomBookshelf()
    {
        Bookshelf randomBookshelf;
        do
        {
            randomBookshelf = bookshelves[Random.Range(0, bookshelves.Length)];
        } while (bookshelvesWithBooks.Contains(randomBookshelf));

        return randomBookshelf;
    }

    private void FillBooksByCategory()
    {
        booksByCategory.Add(DeweyCategory.General, books000General);
        booksByCategory.Add(DeweyCategory.Philosophy, books100Philosophy);
        booksByCategory.Add(DeweyCategory.Religion, books200Religion);
        booksByCategory.Add(DeweyCategory.SocialScience, books300SocialScience);
        booksByCategory.Add(DeweyCategory.Language, books400Language);
        booksByCategory.Add(DeweyCategory.PureScience, books500PureScience);
        booksByCategory.Add(DeweyCategory.Technology, books600Technology);
        booksByCategory.Add(DeweyCategory.Arts, books700Arts);
        booksByCategory.Add(DeweyCategory.Literature, books800Literature);
        booksByCategory.Add(DeweyCategory.History, books900History);
    }

    private void SpawnBookToOrder(DeweyCategory category)
    {
        if (currentSpawnPoint >= bookSpawnPoints.Length) currentSpawnPoint = 0;
        GameObject spawnBook = GetRandomBook(category);
        Vector3 spawnPoint = bookSpawnPoints[currentSpawnPoint].position;
        Quaternion spawnRotation = bookSpawnPoints[currentSpawnPoint].rotation;
        GameObject book = Instantiate(spawnBook, spawnPoint, spawnRotation);
        Book bookComponent = book.GetComponent<Book>();
        bookComponent.Category = category;
        currentSpawnPoint++;
    }

    private GameObject GetRandomBook(DeweyCategory category)
    {
        return booksByCategory[category][Random.Range(0, booksByCategory[category].Length)];
    }

    private bool IsGameFinished()
    {
        foreach (var bookshelf in bookshelves)
        {
            if (!bookshelf.IsActive) continue;
            if (!bookshelf.CurrentBook) return false;
            if (bookshelf.CheckBook()) goodBooks++;
            else badBooks++;
        }

        return true;
    }

    private void TryEndGame()
    {
        if (IsGameFinished())
            Debug.Log($"Game Fnished! Good books: {goodBooks}, Bad books: {badBooks}, Total books: {totalBooks}");
    }
}