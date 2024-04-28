using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      if (other.TryGetComponent(out Car car))
      {
         car.StopCar();
         
         car.spawnPoint.gameObject.SetActive(false);
         car.targetPoint.gameObject.SetActive(false);
      }
   }
}
