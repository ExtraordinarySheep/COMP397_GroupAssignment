using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image buttonImage;
    [SerializeField] TextMeshProUGUI buttonText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 1f);
        buttonText.color = new Color(0f, 0f, 0f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0f);
        buttonText.color = new Color(1f, 1f, 1f, 1f);
    }

}
