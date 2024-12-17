using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;


[ExecuteInEditMode] 
public class BooksSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] bookPrefabs;
    
    private GameObject spawnedBook;
    public GameObject SpawnedBook => spawnedBook;

    private void Awake()
    {
        if (transform.childCount == 0) return;
        spawnedBook = transform.GetChild(0).gameObject;
    }

    [Button("Spawn Books")]
    public void SpawnBooks()
    {
        DestroySpawnedBooks();
        Instantiate(GetRandomBook(), transform.position, transform.localRotation, transform);
    }

    public void DestroySpawnedBooks()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    private GameObject GetRandomBook()
    {
        return bookPrefabs[Random.Range(0, bookPrefabs.Length)];
    }
    
}
