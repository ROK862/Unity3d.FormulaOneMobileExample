using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    [SerializeField] public static List<DriverDashBoard> racers = new List<DriverDashBoard>();
    [SerializeField] private AIPathCreator raceCurcuit;
    [SerializeField] private CheckerFlag checkerFlag;
    [SerializeField] private List<Transform> Racers;

    [SerializeField] private int UpdateMargin = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private int nextUpdate = 0;
    void Update()
    {
        if (nextUpdate < UpdateMargin) {
            nextUpdate++;
            return;
        }
        nextUpdate = 0;
        
        ////////////////////////////////////////////////////////////////////////////////////////////// 
        List<DriverDashBoard> lstOrdered = racers.OrderBy(item => (-1 *item.GetCircuitProgress())).ToList();
        racers = lstOrdered;
    }

    public static int GetPosition (DriverDashBoard player) {
        if (player != null) {
            return racers.IndexOf(player) + 1;
        }
        return 0;
    }
}
