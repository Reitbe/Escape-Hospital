﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockControl0247 : MonoBehaviour
{
    private int[] result, correctCombination;
    public bool isOpened;

    private SoundManager soundManager;

    private void Start()
    {
        soundManager = SoundManager.Instance;
        result = new int[]{0,0,0,0};
        correctCombination = new int[] { 0, 2, 4, 7 };
        isOpened = false;
        Rotate.Rotated += CheckResults;
    }

    private void CheckResults(string wheelName, int number)
    {
        switch (wheelName)
        {
            case "WheelOne":
                result[0] = number;
                break;

            case "WheelTwo":
                result[1] = number;
                break;

            case "WheelThree":
                result[2] = number;
                break;

            case "WheelFour":
                result[3] = number;
                break;
        }

        if (result[0] == correctCombination[0] && result[1] == correctCombination[1]
            && result[2] == correctCombination[2] && result[3] == correctCombination[3] && !isOpened)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.06f, transform.position.z);
            isOpened = true;
            soundManager.PlaySound(soundManager.SfxBasicPlayers, soundManager.SfxBasics, "LockOpen");
        }
    }

    private void OnDestroy()
    {
        Rotate.Rotated -= CheckResults;
    }
}
