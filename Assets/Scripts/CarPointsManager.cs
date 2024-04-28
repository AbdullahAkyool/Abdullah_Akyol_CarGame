using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class CarPointsManager : MonoBehaviour
{
    public static CarPointsManager Instance;
    
    [Serializable]
    public class TransformList
    {
        public List<Transform> carTransforms = new List<Transform>();
    }

    public List<TransformList> carPointsLists = new List<TransformList>();

    public List<SplineComputer> splineComputers;

    private void Awake()
    {
        Instance = this;
    }

    public void DeactivateAllPoints()
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
