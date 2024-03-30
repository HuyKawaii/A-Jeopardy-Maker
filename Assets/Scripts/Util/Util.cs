using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util 
{
    public static int CalculatePoint(int questionIndex, int numberOfCategory)
    {
        return (questionIndex / numberOfCategory + 1) * 100;
    }
}
