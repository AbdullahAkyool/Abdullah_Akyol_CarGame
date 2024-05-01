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
        
        if (!spawnedCars[currentCarIndex - 1].isCycle)   //önceki araba targeta ulasti mi kontrolü
        {
            spawnedCars[currentCarIndex - 1].CheckCarCycle();
        }
        
        if (!spawnedCars[currentCarIndex].isCycle)
        {
            spawnedCars[currentCarIndex].CheckCarCycle(); 
        }

        if (spawnedCars[currentCarIndex - 1].isCycle && !spawnedCars[currentCarIndex].isCycle)
        {
            ResetAllCars();
        }
    }

    public void SetDirectionOfCar(float direction)  //aktif araba sag sol donusu
    {
        activeCar.direction = direction;
    }

    public void IncreaseSpeed(float multiplier) //donuslerde hizi arttır
    {
        activeCar.driveSpeed *= multiplier;
        activeCar.turnSpeed *= multiplier;
    }

    public void DecreaseSpeed(float multiplier)  //donus bittiginde eski hiza don
    {
        activeCar.driveSpeed /= multiplier;
        activeCar.turnSpeed /= multiplier;
    }

    private void ActivateNextCar()  //siradaki arabaya gec
    {
        if (currentCarIndex >= LevelManager.Instance.allCars.Count) return;

        activeCar = LevelManager.Instance.SpawnCar();      //pool'da bazi sorunlar ciktigi icin gecici olarak surekli instantiate denendi. ikisi de calisiyor

        //var newCar = LevelManager.Instance.InstantiateNewCar(currentCarIndex);
        //activeCar = newCar;
        
        spawnedCars.Add(activeCar);
        

        var newSplineComputer = Instantiate(splineComputerPrefab);  //dreamteck sorunlarından dolayi her araba icin yeni bir spline computer instantiate edildi
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
                ResetAllCars();  // her yeni arabada oyundaki tüm arabalar spawn konumlarına donuyor
            }
        }
        
        FreezeGame(); //her yeni arabada oyunu duraklat
    }

    private void FreezeGame()
    {
        Time.timeScale = 0;
        
        gameFrozen = true;
    }

    public void UnFreezeGame()
    {
        if(!gameFrozen) return;
        
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
                LevelManager.Instance.DespawnCar(spawnedCars[i]); //pool içerisinden sıradaki level'i cekme
                spawnedCars[i].MakeDriveable();

                //Destroy(spawnedCars[i].gameObject);
                Destroy(allSplineComputers[i].gameObject);
            }
        }

        spawnedCars.Clear();
        allSplineComputers.Clear();

        activeLevel = LevelManager.Instance.SpawnLevel();
        activeLevel.DeactivateAllPoints();

        ActivateNextCar();
    }

    public void CheckLastCar()  //son araba da target'a ulasirsa siradaki level'a gec
    {
        if (currentCarIndex == LevelManager.Instance.allCars.Count)
        {
            LevelManager.Instance.DespawnLevel(activeLevel);
            CreateLevel();
        }
    }

    public void RestartGame() //oyun kitlendiginde yeniden baslatmak icin
    {
        currentCarIndex = 0;

        if (spawnedCars.Count >= 1)
        {
            for (int i = 0; i < spawnedCars.Count; i++)
            {
                LevelManager.Instance.DespawnCar(spawnedCars[i]);
                spawnedCars[i].MakeDriveable();

                //Destroy(spawnedCars[i].gameObject);
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