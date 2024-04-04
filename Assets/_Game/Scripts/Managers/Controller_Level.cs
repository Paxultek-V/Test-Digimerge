using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Level : MonoBehaviour
{
    public static Action OnLevelStart;
    public static Action OnFinishedLevel;


    private void OnEnable()
    {
        PiggyBank.OnPiggyBankFinishedCollectingMoney += OnPiggyBankFinishedCollectingMoney;
    }

    private void OnDisable()
    {
        PiggyBank.OnPiggyBankFinishedCollectingMoney -= OnPiggyBankFinishedCollectingMoney;
    }


    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        OnLevelStart?.Invoke();
    }


    private void OnPiggyBankFinishedCollectingMoney(float amountCollected)
    {
        OnFinishedLevel?.Invoke();
    }
}