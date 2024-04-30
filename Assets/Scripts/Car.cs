using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
    private Rigidbody rb;
    public Transform spawnPoint;
    public Transform targetPoint;
    
    public float driveSpeed;
    public float turnSpeed;
    public float followSpeed;
    
    public bool canDrive;
    public bool canFollow;
    public bool isCycle;

    [SerializeField] private List<SplinePoint> carDrivePoints;
    //[SerializeField] private List<Vector3> carDrivePoints;
    //[SerializeField] private int pathPointIndex;
    
    //private SplinePoint[] splinePoints;
    public SplineComputer splineComputer;
    public SplineFollower splineFollower;
    public float direction = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        StartCoroutine(CreatePathPoints());
    }

    void Update()
    {
        if (canDrive)
        {
            MoveCar();
            TurnCar(direction);
        }

        if (canFollow)
        {
            FollowPath();
        }
    }

    private void MoveCar()
    {
        transform.Translate(Vector3.forward * driveSpeed * Time.deltaTime);
    }

    public void StopCar()
    {
        driveSpeed = 0;

        spawnPoint.gameObject.SetActive(false);
        targetPoint.gameObject.SetActive(false);

        SplinePoint lastSplinePoint = new SplinePoint( targetPoint.position);
        carDrivePoints.Add(lastSplinePoint);

        canDrive = false;
        canFollow = true;
        splineFollower.follow = true;

        ActionManager.OnCarReachedTarget?.Invoke();
    }

    public void MakeDriveable()
    {
        canDrive = true;
        canFollow = false;

        driveSpeed = 15;

        spawnPoint = null;
        targetPoint = null;

        splineFollower.follow = false;

        carDrivePoints.Clear();
    }

    private void TurnCar(float turnValue)
    {
        transform.Rotate(0, turnValue * turnSpeed * Time.deltaTime, 0);
    }

    private void FollowPath()
    {
        if (splineComputer.pointCount <= 0)
        {
            //splinePoints = splineComputer.GetPoints(SplineComputer.Space.World);
            splineComputer.SetPoints(carDrivePoints.ToArray());
            splineComputer.RebuildImmediate();
        
            splineFollower.followSpeed = followSpeed;
        }

        // speed = 15;
        //
        // if (pathPointIndex < carDrivePoints.Count)
        // {
        //     Vector3 targetPathPoint = carDrivePoints[pathPointIndex];
        //     float distance = Vector3.Distance(targetPathPoint, transform.localPosition);
        //     
        //     Quaternion targetRotation = Quaternion.LookRotation(targetPathPoint - transform.localPosition);
        //     transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 20);
        //     
        //     transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPathPoint, speed * Time.deltaTime);
        //     
        //     if (distance <= .5)
        //     {
        //         pathPointIndex++;
        //     }
        //     
        // }
    }

    IEnumerator CreatePathPoints()
    {
        while (canDrive)
        {
            SplinePoint newSplinePoint = new SplinePoint(transform.position);
            
            if (!carDrivePoints.Contains(newSplinePoint))
            {
                carDrivePoints.Add(newSplinePoint);
            }

            // var newPathPoint = transform.localPosition;
            // newPathPoint.y = 0;
            //
            // if (!carDrivePoints.Contains(newPathPoint))
            // {
            //     carDrivePoints.Add(newPathPoint);
            // }

            yield return new WaitForSeconds(.2f);
        }
    }

    // void CheckCarCycle()
    // {
    //     float distance = Vector3.Distance(transform.localPosition, targetPoint.localPosition);
    //     
    //     if (distance <= 1f)
    //     {
    //         isCycle = true;
    //     }
    // }
    //
    public void CheckPreviousCarCycle()
    {
        // if(GameManager.Instance.currentCarIndex <= 0) return;
        //
        // if (GameManager.Instance.spawnedCars[GameManager.Instance.currentCarIndex - 1].isCycle)
        // {
        //     isCompletedMission = true;
        // }
        
        float distance = Vector3.Distance(transform.localPosition, targetPoint.localPosition);
        
        if (distance <= 1f)
        {
            isCycle = true;
        }
    }

    public void ResetPosition()
    {
        if (canDrive)
        {
            isCycle = false;
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            carDrivePoints.Clear();
        }

        if (canFollow)
        {
            isCycle = false;
            splineFollower.SetPercent(0);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Car car) || other.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.ResetAllCars();
        }
    }
}