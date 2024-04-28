using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public List<Car> carPrefabs;
    [SerializeField] private int currentCarIndex = -1;
    private float timeLimit = 20f;
    private float timer;

    [System.Serializable]
    public class TransformList
    {
        public List<Transform> carTransforms = new List<Transform>();
    }

    public List<TransformList> carPointsLists = new List<TransformList>();

    public List<SplineComputer> SplineComputers;

    private void Awake()
    {
        Instance = this;

        ActionManager.OnCarReachedTarget += ActivateNextCar;
    }

    private void Start()
    {
        ActivateNextCar();
    }

    public void TurnCar(float turnValue)
    {
        carPrefabs[currentCarIndex].TurnCar(turnValue);
    }

    private void ActivateNextCar()
    {
        if (currentCarIndex >= carPrefabs.Count) return;

        currentCarIndex++;

        var newCar = Instantiate(carPrefabs[currentCarIndex], transform);

        newCar.splineComputer = SplineComputers[currentCarIndex];
        newCar.splineFollower.spline = SplineComputers[currentCarIndex];


        if (carPointsLists.Count >= 1)
        {
            TransformList currentCarList = carPointsLists[currentCarIndex];

            if (currentCarList.carTransforms.Count >= 1)
            {
                newCar.spawnPoint = currentCarList.carTransforms[0];
                newCar.targetPoint = currentCarList.carTransforms[1];

                newCar.ResetPosition();
            }
        }
    }
}