using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlButton : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private enum CarDirection
    {
        Left,
        Right
    }

    [SerializeField] private CarDirection direction;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (direction.Equals(CarDirection.Left))
        {
            gameManager.SetDirectionOfCar(-1F);
        }
        else if(direction.Equals(CarDirection.Right))
        {
            gameManager.SetDirectionOfCar(1F);
        }
        else
        {
            throw new Exception();
        }
        
        gameManager.IncreaseSpeed(1.5F);
        GameManager.Instance.UnFreezeGame();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       gameManager.SetDirectionOfCar(0F);
       gameManager.DecreaseSpeed(1.5F);
    }
}
