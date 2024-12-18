using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
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
    History = 900,
    None = 666
}

public class Bookshelf : MonoBehaviour
{
    [SerializeField, OnValueChanged("SetSignTexts")]
    DeweyCategory category;

    [SerializeField] BookInteractor[] bookInteractors;
    [Header("DEVELOP")] [SerializeField] BooksSpawner[] booksSpawners;
    [SerializeField] Material activeSocketMaterial;
    [SerializeField] TextMeshProUGUI[] signTextsUI;

    public static event Action OnInsertedBook;

    private XRSocketInteractor socketInteractor;
    private BookInteractor currentBookInteractor;
    public BookInteractor CurrentBookInteractor => currentBookInteractor;
    public XRSocketInteractor SocketInteractor => socketInteractor;

    public DeweyCategory Category
    {
        get => category;
    }

    private bool isActive;
    public bool IsActive => isActive;
    private Book currentBook;
    public Book CurrentBook => currentBook;

    public static Dictionary<DeweyCategory, string> signTexts = new Dictionary<DeweyCategory, string>
    {
        { DeweyCategory.General, "General" },
        { DeweyCategory.Philosophy, "Philosophy" },
        { DeweyCategory.Religion, "Religion" },
        { DeweyCategory.SocialScience, "Social Science" },
        { DeweyCategory.Language, "Language" },
        { DeweyCategory.PureScience, "Science" },
        { DeweyCategory.Technology, "Technology" },
        { DeweyCategory.Arts, "Arts" },
        { DeweyCategory.Literature, "Literature" },
        { DeweyCategory.History, "History" },
        { DeweyCategory.None, "" }
    };


    private void Start()
    {
        SetSignTexts();
    }

    private void SetSignTexts()
    {
        foreach (var signTextUI in signTextsUI)
        {
            signTextUI.text = signTexts[category];
#if UNITY_EDITOR
            EditorUtility.SetDirty(signTextUI);
#endif
        }
    }

    private void DisableAllBookInteractors()
    {
        foreach (var bookInteractor in bookInteractors)
        {
            bookInteractor.SocketInteractor.enabled = false;
        }
    }

    private void OnDisable()
    {
        if (!socketInteractor) return;
        socketInteractor.selectEntered.RemoveListener(OnSelectEntered);
    }

    public void ChooseRandomSocket()
    {
        DisableAllBookInteractors();
        isActive = true;
        // int randomIndex = 0;
        int randomIndex = UnityEngine.Random.Range(0, bookInteractors.Length);
        currentBookInteractor = bookInteractors[randomIndex];
        if (activeSocketMaterial)
            currentBookInteractor.GetComponentInChildren<Renderer>().material = activeSocketMaterial;
        currentBookInteractor.transform.GetChild(0).GetComponentInChildren<Collider>().enabled = false;
        socketInteractor = bookInteractors[randomIndex].SocketInteractor;
        socketInteractor.enabled = true;
        socketInteractor.selectEntered.AddListener(OnSelectEntered);
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

    private void OnSelectEntered(SelectEnterEventArgs arg0)
    {
        currentBook = arg0.interactableObject.transform.GetComponent<Book>();
        OnInsertedBook?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (!socketInteractor) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(socketInteractor.transform.position, 0.2f);
    }
}