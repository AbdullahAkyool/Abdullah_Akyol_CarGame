using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlGameTime : MonoBehaviour,IPointerDownHandler
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameManager.UnFreezeGame();
    }
}
