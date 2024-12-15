using Sirenix.OdinInspector;
using UnityEngine;


[ExecuteInEditMode] 
public class BooksSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] bookPrefabs;
    [Button("Spawn Books")]
    private void SpawnBooks()
    {
        Instantiate(GetRandomBook(), transform.position, Quaternion.identity, transform);
    }
    
    private GameObject GetRandomBook()
    {
        return bookPrefabs[Random.Range(0, bookPrefabs.Length)];
    }
    
}
