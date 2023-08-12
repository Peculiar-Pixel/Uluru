using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BirdTypes
{
    white = 0,
    pink = 1,
    yellow = 2,
    orange = 3,
    red = 4,
    green = 5,
    blue = 6,
    black = 7
}

public class Bird
{
    //requirement on easy mode
    public PreferenceType easyBirdPreference;
    public Bird? easyOtherBird;

    //extra requirement on hard mode
    public PreferenceType hardBirdPreference;
    public Bird? hardOtherBird;

    //Bird descriptions
    public int position;
    public BirdTypes type;

    public Bird(BirdTypes birdType)
    {
        this.type = birdType;
    }
}