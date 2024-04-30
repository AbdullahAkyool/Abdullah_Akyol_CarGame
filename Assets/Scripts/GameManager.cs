using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentCarIndex = -1;
    [SerializeField] private Car activeCar = null;
    [SerializeField] private List<Car> spawnedCars;

    [SerializeField] private Level activeLevel;

    [SerializeField] private SplineComputer splineComputerPrefab;
    [SerializeField] private List<SplineComputer> allSplineComputers;

    [SerializeField] private bool gameFrozen;

    private void Awake()
    {
        Instance = this;

        ActionManager.OnCarReachedTarget += ActivateNextCar;
    }

    private void Start()
    {
        CreateLevel();
    }

    private void Update()
    {
        if(currentCarIndex<= 0) return;
        
        if (!spawnedCars[currentCarIndex - 1].isCycle)
        {
            spawnedCars[currentCarIndex - 1].CheckPreviousCarCycle();
        }
        
        if (!spawnedCars[currentCarIndex].isCycle)
        {
            spawnedCars[currentCarIndex].CheckPreviousCarCycle();
        }

        if (spawnedCars[currentCarIndex - 1].isCycle && !spawnedCars[currentCarIndex].isCycle)
        {
            ResetAllCars();
        }
    }

    public void SetDirectionOfCar(float direction)
    {
        activeCar.direction = direction;
    }

    public void IncreaseSpeed(float multiplier)
    {
        activeCar.driveSpeed *= multiplier;
        activeCar.turnSpeed *= multiplier;
    }

    public void DecreaseSpeed(float multiplier)
    {
        activeCar.driveSpeed /= multiplier;
        activeCar.turnSpeed /= multiplier;
    }

    private void ActivateNextCar()
    {
        if (currentCarIndex >= LevelManager.Instance.allCars.Count) return;

        //activeCar = LevelManager.Instance.SpawnCar();

        var newCar = LevelManager.Instance.InstantiateNewCar(currentCarIndex);
        activeCar = newCar;
        
        spawnedCars.Add(activeCar);
        

        var newSplineComputer = Instantiate(splineComputerPrefab);
        allSplineComputers.Add(newSplineComputer);

        activeCar.splineComputer = newSplineComputer;
        activeCar.splineFollower.spline = newSplineComputer;

        if (activeLevel.carPointsLists.Count >= 1)
        {
            Level.TransformList currentCarList = activeLevel.carPointsLists[currentCarIndex];

            if (currentCarList.carTransforms.Count >= 1)
            {
                activeCar.spawnPoint = currentCarList.carTransforms[0];
                activeCar.targetPoint = currentCarList.carTransforms[1];
                
                activeCar.spawnPoint.gameObject.SetActive(true);
                activeCar.targetPoint.gameObject.SetActive(true);

                //activeCar.ResetPosition();
                ResetAllCars();
            }
        }
        
        FreezeGame();

    }

    private void FreezeGame()
    {
        Time.timeScale = 0;
        
        gameFrozen = true;
        
        // for (int i = 0; i < spawnedCars.Count; i++)
        // {
        //     var frozenCar = spawnedCars[i];
        //     
        //     frozenCar.FreezeCar();
        // }
    }

    public void UnFreezeGame()
    {
        // for (int i = 0; i < spawnedCars.Count; i++)
        // {
        //     var frozenCar = spawnedCars[i];
        //     
        //     frozenCar.UnFreezeCar();
        // }
        
        Time.timeScale = 1;
        
        gameFrozen = false;
    }

    public void ResetAllCars()
    {
        for (int i = 0; i < spawnedCars.Count; i++)
        {
            var listCar = spawnedCars[i];

            listCar.ResetPosition();
        }
    }

    private void CreateLevel()
    {
        currentCarIndex = 0;

        if (spawnedCars.Count >= 1)
        {
            for (int i = 0; i < spawnedCars.Count; i++)
            {
                //LevelManager.Instance.DespawnCar(spawnedCars[i]);
                //spawnedCars[i].MakeDriveable();

                Destroy(spawnedCars[i].gameObject);
                Destroy(allSplineComputers[i].gameObject);
            }
        }

        spawnedCars.Clear();
        allSplineComputers.Clear();

        activeLevel = LevelManager.Instance.SpawnLevel();
        activeLevel.DeactivateAllPoints();

        ActivateNextCar();
    }

    public void CheckLastCar()
    {
        if (currentCarIndex == LevelManager.Instance.allCars.Count)
        {
            LevelManager.Instance.DespawnLevel(activeLevel);
            CreateLevel();
        }
    }

    public void RestartGame()
    {
        currentCarIndex = 0;

        if (spawnedCars.Count >= 1)
        {
            for (int i = 0; i < spawnedCars.Count; i++)
            {
                //LevelManager.Instance.DespawnCar(spawnedCars[i]);
                //spawnedCars[i].MakeDriveable();

                Destroy(spawnedCars[i].gameObject);
                Destroy(allSplineComputers[i].gameObject);
            }
        }

        spawnedCars.Clear();
        allSplineComputers.Clear();
        
        activeLevel.DeactivateAllPoints();
        
        ActivateNextCar();
    }

    private void OnDestroy()
    {
        ActionManager.OnCarReachedTarget -= ActivateNextCar;
    }
}