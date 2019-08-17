using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour {


    protected const int zero = 0;
    protected const int selectionCounter = 6;
    protected const int hexagonSlideDistance = 5;
    protected const int hexagonRotateConstant = 9;
    protected const int hexagonLerpingdownConstant = 2;
    protected const float rotateThreshold = 0.05f;
    protected const string hexagonTag = "Hexagon";

    //Boolean variables for checking game situation
    public static bool gameOver = false;
    public static bool bombPermission = false;
    public static bool bombIsOn = false;
    public static bool rotating = false;
    public static bool exploding = false;
    public static bool checkingExplosion = false;
    public static bool reassigning = false;
    public static bool producing = false;
}
