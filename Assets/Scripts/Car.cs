using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
    private Rigidbody rb;

    public Transform spawnPoint;
    public Transform targetPoint;
    public float speed;
    public float turnSpeed;
    public bool canDrive;
    
    public List<SplinePoint> carDrivePoints;
    public SplinePoint[] splinePoints;

    public SplineComputer splineComputer;
    public SplineFollower splineFollower;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        StartCoroutine(CreatePath());
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

        if (Input.GetKey(KeyCode.A))
        {
            TurnCar(-60);
        }

        if (Input.GetKey(KeyCode.D))
        {
            TurnCar(60);
        }
    }

    private void MoveCar()
    {
        if (Vector3.Distance(transform.position, targetPoint.position) > .5f)
        {
            // transform.position += transform.forward * speed * Time.deltaTime;

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
        if (canDrive)
        {
            // Vector3 currentRotation = transform.localEulerAngles;
            //
            // currentRotation.y += turnValue;
            //
            // Debug.Log(currentRotation);
            //
            // transform.localEulerAngles = currentRotation;

            transform.Rotate(0, turnValue * Time.deltaTime, 0);
        }
    }

    private void FollowPath()
    {
        splinePoints = splineComputer.GetPoints(SplineComputer.Space.World);
        splineComputer.SetPoints(carDrivePoints.ToArray(), SplineComputer.Space.World);
        splineComputer.RebuildImmediate();

        splineFollower.followSpeed = 5;
        splineFollower.follow = true;
    }

    IEnumerator CreatePath()
    {
        while (canDrive)
        {
            SplinePoint newSplinePoint = new SplinePoint(transform.position);
            
            if (!carDrivePoints.Contains(newSplinePoint))
            {
                carDrivePoints.Add(newSplinePoint);
            }
            
            yield return new WaitForSeconds(.5f);

        }
        
    }

    private void OnCollisionEnter(Collision other)
    {
        // if(canDrive) ResetPosition();
    }
}