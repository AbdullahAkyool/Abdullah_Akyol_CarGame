using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Car> carPrefabs;
    [SerializeField] private int currentCarIndex = -1;
    private float timeLimit = 20f;
    private float timer;
    private Car activeCar = null;
    public List<Car> spawnedCars;

    private void Awake()
    {
        Instance = this;

        ActionManager.OnCarReachedTarget += ActivateNextCar;
    }

    private void Start()
    {
        CarPointsManager.Instance.DeactivateAllPoints();
        ActivateNextCar();
    }

    public void SetDirectionOfCar(float direction)
    {
        activeCar.direction = direction;
    }

    public void IncreaseSpeed(float multiplier)
    {
        activeCar.speed *= multiplier;
        activeCar.turnSpeed *= multiplier;
    }

    public void DecreaseSpeed(float multiplier)
    {
        activeCar.speed /= multiplier;
        activeCar.turnSpeed /= multiplier;
    }

    private void ActivateNextCar()
    {
        if (currentCarIndex >= carPrefabs.Count) return;

        currentCarIndex++;

        var newCar = Instantiate(carPrefabs[currentCarIndex], transform);
        activeCar = newCar;
        spawnedCars.Add(activeCar);

        activeCar.splineComputer = CarPointsManager.Instance.splineComputers[currentCarIndex];
        activeCar.splineFollower.spline = CarPointsManager.Instance.splineComputers[currentCarIndex];

        if (CarPointsManager.Instance.carPointsLists.Count >= 1)
        {
            CarPointsManager.TransformList currentCarList = CarPointsManager.Instance.carPointsLists[currentCarIndex];

            if (currentCarList.carTransforms.Count >= 1)
            {
                activeCar.spawnPoint = currentCarList.carTransforms[0];
                activeCar.targetPoint = currentCarList.carTransforms[1];
                
                activeCar.spawnPoint.gameObject.SetActive(true);
                activeCar.targetPoint.gameObject.SetActive(true);

                activeCar.ResetPosition();
            }
        }
    }

    public void CheckLastCar()
    {
        if (currentCarIndex == carPrefabs.Count - 1)
        {
            
        }
    }
    
}