using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTestScript : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    // Start is called before the first frame update
    void Start()
    {
        inputReader.MoveEvent += HandleMove;
        inputReader.PrimaryFireEvent += HandleFire;
    }


    private void OnDestroy() 
    {
        inputReader.MoveEvent -= HandleMove;
        inputReader.PrimaryFireEvent -= HandleFire;
    }

    private void HandleMove(Vector2 vector)
    {
        Debug.Log("MoveEvent: " + vector);
    }

    private void HandleFire(bool obj)
    {
        if(obj)
        {
            Debug.Log("Fire!");
        }
    }
}
