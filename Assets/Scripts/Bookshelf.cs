using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public enum DeweyCategory
{
    General = 0,
    Philosophy = 100,
    Religion = 200,
    SocialScience = 300,
    Language = 400,
    PureScience = 500,
    Technology = 600,
    Arts = 700,
    Literature = 800,
    History = 900
}

public class Bookshelf : MonoBehaviour
{
    [SerializeField] DeweyCategory category;
    [SerializeField] private BookInteractor[] bookInteractors;
    [Header("DEVELOP")]
    [SerializeField] BooksSpawner[] booksSpawners;

    public static event Action OnInsertedBook;
    
    private XRSocketInteractor socketInteractor;
    private BookInteractor currentBookInteractor;
    public BookInteractor CurrentBookInteractor => currentBookInteractor;
    public XRSocketInteractor SocketInteractor => socketInteractor;
    public DeweyCategory Category => category;
    
    private bool isActive;
    public bool IsActive => isActive;
    private Book currentBook;
    public Book CurrentBook => currentBook;

    private void DisableAllBookInteractors()
    {
        foreach (var bookInteractor in bookInteractors)
        {
            bookInteractor.SocketInteractor.enabled = false;
        }
    }

    private void OnDisable()
    {
        if(!socketInteractor) return;
        socketInteractor.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectExited(SelectExitEventArgs arg0)
    {
        XRGrabInteractable grabInteractable = arg0.interactableObject as XRGrabInteractable;
        currentBook = arg0.interactableObject.transform.GetComponent<Book>();
        if (!grabInteractable) return;
        grabInteractable.enabled = false;
        XRInteractableAffordanceStateProvider affordanceStateProvider = grabInteractable.GetComponent<XRInteractableAffordanceStateProvider>();
        if (affordanceStateProvider) affordanceStateProvider.enabled = false;
        OnInsertedBook?.Invoke();
    }
    public void ChooseRandomSocket()
    {
        DisableAllBookInteractors();
        isActive = true;
        // int randomIndex = 0;
        int randomIndex = UnityEngine.Random.Range(0, bookInteractors.Length);
        currentBookInteractor = bookInteractors[randomIndex];
        currentBookInteractor.GetChildBook().SetActive(false);
        socketInteractor = bookInteractors[randomIndex].SocketInteractor;
        socketInteractor.enabled = true;
        socketInteractor.selectExited.AddListener(OnSelectExited);
    }
    
    [Button]
    public bool CheckBook()
    {
        Debug.Log(currentBook && currentBook.Category == category);
        return currentBook && currentBook.Category == category;
    }
    
    [Button]
    private void SpawnBooks()
    {
        foreach (var booksSpawner in booksSpawners)
        {
            booksSpawner.SpawnBooks();
        }
    }
    [Button("Spawn Books In Interactors")]
    private void SpawnBooksInInteractors()
    {
        foreach (var bookInteractor in bookInteractors)
        {
            bookInteractor.BookSpawner.SpawnBooks();
        }
    }
    [Button("Clear Interactor Books")]
    private void ClearBooks()
    {
        foreach (var bookInteractor in bookInteractors)
        {
            bookInteractor.BookSpawner.DestroySpawnedBooks();
        }
    }
    [Button("Clear Bookshelve Books")]
    private void ClearBookshelveBooks()
    {
        foreach (var booksSpawner in booksSpawners)
        {
            booksSpawner.DestroySpawnedBooks();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!socketInteractor) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(socketInteractor.transform.position, 0.2f);
    }
}
