using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public List<Car> cars;
    [SerializeField]private int currentCarIndex = -1;
    private float timeLimit = 20f;
    private float timer;

    [System.Serializable]
    public class TransformList
    {
        public List<Transform> carTransforms = new List<Transform>();
    }

    public List<TransformList> carPointsLists = new List<TransformList>();

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
        cars[currentCarIndex].GetComponent<Car>().TurnCar(turnValue);
    }

    private void ActivateNextCar()
    {
        currentCarIndex++;
        
        var newCar = Instantiate(cars[currentCarIndex], transform);
        
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