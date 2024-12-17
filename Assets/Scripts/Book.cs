using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] DeweyCategory category;

    Dictionary<DeweyCategory, string> deweyCodes = new Dictionary<DeweyCategory, string>
    {
        {DeweyCategory.General, "000"},
        {DeweyCategory.Philosophy, "100"},
        {DeweyCategory.Religion, "200"},
        {DeweyCategory.SocialScience, "300"},
        {DeweyCategory.Language, "400"},
        {DeweyCategory.PureScience, "500"},
        {DeweyCategory.Technology, "600"},
        {DeweyCategory.Arts, "700"},
        {DeweyCategory.Literature, "800"},
        {DeweyCategory.History, "900"},
        {DeweyCategory.None, "666"}
    };
    public DeweyCategory Category
    {
        get => category;
        set => category = value;
    }

    private void Start()
    {
        TextMeshPro deweyCodeText = GetComponentInChildren<TextMeshPro>();
        if (deweyCodeText) deweyCodeText.text = deweyCodes[category];
    }
}