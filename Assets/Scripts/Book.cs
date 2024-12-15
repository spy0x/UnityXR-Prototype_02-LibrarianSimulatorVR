using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] DeweyCategory category;
    public DeweyCategory Category => category;
    
}
