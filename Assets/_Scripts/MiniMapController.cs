using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    [SerializeField] private Transform _player;

    void FixedUpdate()
    {
        transform.position = 
            new Vector3(_player.position.x, transform.position.y, _player.position.z);
    }
}
