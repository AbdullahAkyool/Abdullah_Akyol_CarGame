using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      if (other.TryGetComponent(out Car car) && car.targetPoint.gameObject == gameObject)
      {
         GameManager.Instance.currentCarIndex++;
         car.StopCar();
         GameManager.Instance.CheckLastCar();
      }
   }
}
