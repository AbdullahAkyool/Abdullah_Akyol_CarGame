using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
    private Rigidbody rb;
    
    public Transform spawnPoint;
    public Transform targetPoint;
    public float speed = 5.0f;

    public bool canDrive;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ResetPosition()
    {
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    void Update()
    {
        if (canDrive)
        {
            MoveCar();
        }
        else
        {
            FollowPath();
        }
    }

    private void MoveCar()
    {
        if (Vector3.Distance(transform.position, targetPoint.position) > .5f)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector3.zero;
            speed = 0;
            
            canDrive = false;
            
            ActionManager.OnCarReachedTarget?.Invoke();
        }
    }

    public void TurnCar(float turnValue)
    {
        var carRotation = transform.localRotation;
        var targetRotation = Quaternion.Euler(new Vector3(carRotation.x,carRotation.y + turnValue,carRotation.z));
        
        transform.localRotation = Quaternion.Slerp(carRotation,targetRotation,.4f * Time.deltaTime);
       
       //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x,transform.rotation.y + 90,transform.rotation.z));
    }

    private void FollowPath()
    {
       // Debug.Log("car following path");
    }

    public void CreatePath()
    {
        
    }
}
