using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BookInteractor : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor socketInteractor;
    [SerializeField] private BooksSpawner bookSpawner;
    public BooksSpawner BookSpawner => bookSpawner;
    public XRSocketInteractor SocketInteractor => socketInteractor;
    
    public GameObject GetChildBook()
    {
        if (transform.childCount == 0) return null;
        return transform.GetChild(0).gameObject;
    }
}
