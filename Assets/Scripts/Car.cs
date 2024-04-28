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
    [SerializeField] private bool canDrive;

    [SerializeField] private List<SplinePoint> carDrivePoints;
    public SplineComputer splineComputer;
    public SplineFollower splineFollower;
    public float direction = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        StartCoroutine(CreatePathPoints());
    }

    public void ResetPosition()
    {
        if (canDrive)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            carDrivePoints.Clear();
        }
        else
        {
            splineFollower.SetPercent(0);
        }
    }

    void Update()
    {
        if (canDrive)
        {
            MoveCar();
            TurnCar(direction);
        }
        else
        {
            CreatePath();
        }
    }

    private void MoveCar()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void StopCar()
    {
        rb.velocity = Vector3.zero;
        speed = 0;

        canDrive = false;

        ActionManager.OnCarReachedTarget?.Invoke();
    }

    private void TurnCar(float turnValue)
    {
        transform.Rotate(0, turnValue * turnSpeed * Time.deltaTime, 0);
    }

    private void CreatePath()
    {
        // splinePoints = splineComputer.GetPoints(SplineComputer.Space.World);
        splineComputer.SetPoints(carDrivePoints.ToArray());
        splineComputer.RebuildImmediate();

        splineFollower.followSpeed = 5;
        splineFollower.follow = true;
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

            yield return new WaitForSeconds(.5f);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Car car) || other.gameObject.CompareTag("Obstacle"))
        {
            for (int i = 0; i < GameManager.Instance.spawnedCars.Count; i++)
            {
                GameManager.Instance.spawnedCars[i].ResetPosition();
            }
        }
    }
}