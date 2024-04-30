using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
   // levels pool
   // cars pool

   public static LevelManager Instance;

   public List<Car> allCars;
   public List<Level> allLevels;

   public GameObject carsParent;
   [SerializeField] private GameObject levelsParent;

   private Queue<Car> carsPool;
   private Queue<Level> levelsPool;
   private void Awake()
   {
      Instance = this;
      
      carsPool = new Queue<Car>();
      levelsPool = new Queue<Level>();
   }

   private void Start()
   {
      GrowLevelPool();
      //GrowCarPool();
   }

   private void GrowCarPool()
   {
      for (int i = 0; i < allCars.Count; i++)
      {
         var car = Instantiate(allCars[i], carsParent.transform);
         car.gameObject.SetActive(false);
         carsPool.Enqueue(car);
      }
   }

   private void GrowLevelPool()
   {
      for (int i = 0; i < allLevels.Count; i++)
      {
         var level = Instantiate(allLevels[i], levelsParent.transform);
         level.gameObject.SetActive(false);
         levelsPool.Enqueue(level);
      }
   }

   public Car SpawnCar()
   {
      if (carsPool.Count <= 0)
      {
         GrowCarPool();
      }

      var car = carsPool.Dequeue();
      
      car.gameObject.SetActive(true);

      return car;
   }

   public Level SpawnLevel()
   {
      if (levelsPool.Count <= 0)
      {
         GrowLevelPool();
      }

      var level = levelsPool.Dequeue();
      level.gameObject.SetActive(true);
      return level;
   }

   public void DespawnCar(Car car)
   {
      car.gameObject.SetActive(false);
      carsPool.Enqueue(car);
   }

   public void DespawnLevel(Level level)
   {
      level.gameObject.SetActive(false);
      levelsPool.Enqueue(level);
   }

   public Car InstantiateNewCar(int index)
   {
      Car newCar = Instantiate(allCars[index], carsParent.transform);

      return newCar;
   }
}
