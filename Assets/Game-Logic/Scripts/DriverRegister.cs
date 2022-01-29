using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriverRegister : MonoBehaviour
{
    [SerializeField]
    Sprite[] DriverImages;
    [SerializeField]
    public int activeSlot = 0;

    [SerializeField]
    UIDriverIDCard UICardTemplate;
    public List<UIDriverIDCard> driverIDCards;
    // Start is called before the first frame update
    void Start()
    {
        driverIDCards = DBDriver.GetDrivers();

        if (UICardTemplate != null) {
            foreach (UIDriverIDCard card in driverIDCards) {
                GameObject obj = GameObject.Instantiate(UICardTemplate.gameObject);

                obj.transform.parent = UICardTemplate.transform.parent;

                obj.GetComponent<UIDriverIDCard>().Clone(card, DriverImages[card.driverID-1]);
                obj.SetActive(true);
            }
        }
    }

    public void SetActiveSlot(int slotID)
    {
        activeSlot = slotID;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void AddToSlot(UIDriverIDCard uIDriverIDCard)
    {
        throw new NotImplementedException();
    }
}
