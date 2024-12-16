using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] Bookshelf[] bookshelves;
    [SerializeField] float booksToOrder = 5;
    [SerializeField] private Transform[] bookSpawnPoints;
    [Header("Prefabs")] 
    [SerializeField] private GameObject[] booksPrefab000 = new GameObject[0];
    [SerializeField] private GameObject[] booksPrefab100 = new GameObject[0];
    [SerializeField] private GameObject[] booksPrefab200 = new GameObject[0];
    [SerializeField] private GameObject[] booksPrefab300 = new GameObject[0];
    [SerializeField] private GameObject[] booksPrefab400 = new GameObject[0];
    [SerializeField] private GameObject[] booksPrefab500 = new GameObject[0];
    [SerializeField] private GameObject[] booksPrefab600 = new GameObject[0];
    [SerializeField] private GameObject[] booksPrefab700 = new GameObject[0];
    [SerializeField] private GameObject[] booksPrefab800 = new GameObject[0];
    [SerializeField] private GameObject[] booksPrefab900 = new GameObject[0];

    private Dictionary<DeweyCategory, GameObject[]> booksByCategory = new Dictionary<DeweyCategory, GameObject[]>();

    private int currentSpawnPoint = 0;
    private float goodBooks = 0;
    private float badBooks = 0;
    private float totalBooks = 0;

    private void OnEnable()
    {
        Bookshelf.OnInsertedBook += TryEndGame;
    }
    
    private void OnDisable()
    {
        Bookshelf.OnInsertedBook -= TryEndGame;
    }

    private void Start()
    {
        FillBooksByCategory();
        foreach (var bookshelf in bookshelves)
        {
            totalBooks++;
            if (totalBooks > booksToOrder) break;
            bookshelf.ChooseRandomSocket();
            SpawnBookToOrder(bookshelf.Category);
        }
    }

    private void FillBooksByCategory()
    {
        booksByCategory.Add(DeweyCategory.General, booksPrefab000);
        booksByCategory.Add(DeweyCategory.Philosophy, booksPrefab100);
        booksByCategory.Add(DeweyCategory.Religion, booksPrefab200);
        booksByCategory.Add(DeweyCategory.SocialScience, booksPrefab300);
        booksByCategory.Add(DeweyCategory.Language, booksPrefab400);
        booksByCategory.Add(DeweyCategory.PureScience, booksPrefab500);
        booksByCategory.Add(DeweyCategory.Technology, booksPrefab600);
        booksByCategory.Add(DeweyCategory.Arts, booksPrefab700);
        booksByCategory.Add(DeweyCategory.Literature, booksPrefab800);
        booksByCategory.Add(DeweyCategory.History, booksPrefab900);
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
        Debug.Log("Llegue aqui");
        return true;
    }

    private void TryEndGame()
    {
        if (IsGameFinished())
            Debug.Log($"Game Fnished! Good books: {goodBooks}, Bad books: {badBooks}, Total books: {totalBooks}");
    }
}