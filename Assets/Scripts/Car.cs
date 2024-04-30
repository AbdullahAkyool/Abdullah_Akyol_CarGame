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

    public void MakeDriveable()  //yeni level'a geciste arabayi surulebilir yap
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

    private void FollowPath()  //olusturulan path'i spline computer'a bas ve takip et
    {
        if (splineComputer.pointCount <= 0)
        {
            //splinePoints = splineComputer.GetPoints(SplineComputer.Space.World);
            splineComputer.SetPoints(carDrivePoints.ToArray());
            splineComputer.RebuildImmediate();
        
            splineFollower.followSpeed = followSpeed;
        }
    }

    IEnumerator CreatePathPoints()  //belirlenen sürede bir aracin konumunu listeye at ve path olustur
    {
        while (canDrive)
        {
            SplinePoint newSplinePoint = new SplinePoint(transform.position);
            
            if (!carDrivePoints.Contains(newSplinePoint))
            {
                carDrivePoints.Add(newSplinePoint);
            }

            yield return new WaitForSeconds(.2f);
        }
    }
    public void CheckCarCycle() //aracın hedefe ulasip ulasmadigini kontrol et
    {
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