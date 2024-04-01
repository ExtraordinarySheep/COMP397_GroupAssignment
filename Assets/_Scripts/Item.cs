using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public string itemDescription;
    [SerializeField] Transform itemLocation;
    [SerializeField] Icon itemIcon;
    [SerializeField] Mesh itemMesh;

    public delegate void ItemAction();
    public ItemAction pickup;
    public ItemAction drop;
}
