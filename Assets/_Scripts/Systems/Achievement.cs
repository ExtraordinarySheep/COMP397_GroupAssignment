using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievement
{
    public string id;
    public string name;
    public string description;
    public bool unlocked;

    // Constructor
    public Achievement(string id, string name, string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.unlocked = false;
    }
}
