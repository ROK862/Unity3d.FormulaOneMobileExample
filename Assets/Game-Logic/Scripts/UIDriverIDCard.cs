using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UIDriverIDCard : MonoBehaviour
{
    [SerializeField]
    Text overallRating;
    [SerializeField]
    Text uiDriverName;
    [SerializeField]
    Text uiDriverExperience;
    [SerializeField]
    Text uiDriverRaceCraft;
    [SerializeField]
    Text uiDriverAwareness;
    [SerializeField]
    Text uidriverPace;
    [SerializeField]
    Text uiDriverPrice;
    [SerializeField]
    Image uiDriverImage;
    [SerializeField]
    DriverRegister register;

    [SerializeField]
    public int driverID = 0;
    [SerializeField]
    public string driverName = "";
    [SerializeField]
    public int driverExperience = 0;
    [SerializeField]
    public int driverRaceCraft = 0;
    [SerializeField]
    public int driverAwareness = 0;
    [SerializeField]
    public int driverPace = 0;
    [SerializeField]
    public string driverTeam = "";
    [SerializeField]
    public double driverPrice = 0;
    [SerializeField]
    public double driverSalary = 0;
    [SerializeField]
    public double driverBonus = 0;

    public UIDriverIDCard(int _driverID, string _driverName, int _driverExperience, int _driverRaceCraft, int _driverAwareness, int _driverPace, string _driverTeam, double _driverPrice, double _driverSalary, double _driverBonus) {
        driverID = _driverID;
        driverName = _driverName;
        driverExperience = _driverExperience;
        driverRaceCraft = _driverRaceCraft;
        driverAwareness = _driverAwareness;
        driverPace = _driverPace;
        driverTeam = _driverTeam;
        driverPrice = _driverPrice;
        driverSalary = _driverSalary;
        driverBonus = _driverBonus;
    }

    public void AddToSlot() {
        if (register != null) {
            register.AddToSlot(this);
        }
    }

    public void Clone(UIDriverIDCard clone, Sprite image) {
        driverID = clone.driverID;
        driverName = clone.driverName;
        driverExperience = clone.driverExperience;
        driverRaceCraft = clone.driverRaceCraft;
        driverAwareness = clone.driverAwareness;
        driverPace = clone.driverPace;
        driverTeam = clone.driverTeam;
        driverPrice = clone.driverPrice;
        driverSalary = clone.driverSalary;
        driverBonus = clone.driverBonus;

        if (uiDriverImage)
        {
            uiDriverImage.sprite = image;
        }
        if (uiDriverName)
        {
            uiDriverName.text = driverName.ToString();
        }
        if (uiDriverExperience)
        {
            uiDriverExperience.text = driverExperience.ToString();
        }
        if (uiDriverRaceCraft)
        {
            uiDriverRaceCraft.text = driverRaceCraft.ToString();
        }
        if (uiDriverAwareness)
        {
            uiDriverAwareness.text = driverAwareness.ToString();
        }
        if (uiDriverPrice)
        {
            uiDriverPrice.text = "M" + driverPrice.ToString("C", CultureInfo.CurrentCulture);
        }
        if (overallRating)
        {
            overallRating.text = ((int)((driverExperience + driverRaceCraft + driverAwareness + driverPace) / 4)).ToString();
        }
    }
}
