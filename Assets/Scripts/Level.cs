using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Serializable]
    public class TransformList
    {
        public List<Transform> carTransforms = new List<Transform>();
    }

    public List<TransformList> carPointsLists = new List<TransformList>();

    public List<SplineComputer> splineComputers;  //pool'da sorun cikardigi icin diziden spline computer cekme islemi iptal edildi

    public void DeactivateAllPoints() //baslangicta level'daki tum pointlerin aktifligini kapat
    {
        for (int i = 0; i < carPointsLists.Count; i++)
        {
            foreach (var point in carPointsLists[i].carTransforms)
            {
                point.gameObject.SetActive(false);
            }
        }
    }
}
