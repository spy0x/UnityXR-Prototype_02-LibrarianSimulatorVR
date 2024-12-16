using Sirenix.OdinInspector;
using UnityEngine;


[ExecuteInEditMode] 
public class BooksSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] bookPrefabs;
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
