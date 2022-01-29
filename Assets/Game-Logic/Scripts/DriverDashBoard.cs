using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriverDashBoard : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private float lookSpeed = 10.0F;
    [SerializeField] private float followSpeed = 10.0F;
    [SerializeField] private Vector3 offsite;
    [SerializeField] private RadorAI driverAI;
    [SerializeField] private RadorNAV driverNav;
    [SerializeField] private Text driverPosition;
    [SerializeField] private Text driverSpeed;
    [SerializeField] private Text driverName;
    [SerializeField] private Image driverImage;
    [SerializeField] private string playerName;

    void Start() {
        GameObject[] textComponents     = GameObject.FindGameObjectsWithTag ("gui_world_space_driver_info_text");
        GameObject[] speedComponents    = GameObject.FindGameObjectsWithTag ("gui_world_space_driver_info_speed");
        GameObject[] imgComponents      = GameObject.FindGameObjectsWithTag ("gui_world_space_driver_info_img");
        GameObject[] posComponents      = GameObject.FindGameObjectsWithTag ("gui_world_space_driver_info_pos");

        GameObject textComponent = null;
        GameObject speedComponent = null;
        GameObject imgComponent = null;
        GameObject posComponent = null;
        driverAI = transform.parent.gameObject.GetComponent<RadorAI>();
        driverNav = transform.parent.gameObject.GetComponent<RadorNAV>();

        foreach ( GameObject posComp in posComponents) {
            if (posComp.transform.parent == transform)
                posComponent = posComp.gameObject;
        }

        foreach ( GameObject textComp in textComponents) {
            if (textComp.transform.parent.parent.parent == transform)
                textComponent = textComp.gameObject;
        }

        foreach ( GameObject speedComp in speedComponents) {
            if (speedComp.transform.parent.parent.parent == transform)
                speedComponent = speedComp.gameObject;
        }

        foreach ( GameObject imgComp in imgComponents) {
            if (imgComp.transform.parent.parent == transform)
                imgComponent = imgComp.gameObject;
        }

        if (speedComponent != null) {
            driverSpeed = speedComponent.GetComponent<Text>();
        }

        if (posComponent != null) {
            driverPosition = posComponent.GetComponent<Text>();
        }

        if (imgComponent != null) {
            driverImage = imgComponent.GetComponent<Image>();
            if (driverImage != null) {
                playerName = FormateName(driverImage.sprite.name);
            } else {
                playerName = "AI Controler";
            }
        }

        if (textComponent != null) {
            driverName = textComponent.GetComponent<Text>();
            if (driverName != null) {
                driverName.text = playerName;
            }
        }

        RaceManager.racers.Add(this);
    }

    public float GetCircuitProgress () { 
        if (driverNav != null) {
            return driverNav.GetCircuitProgress();
        } 
        return 0.0f;
    }

    private void LookAtTarget() {
        targetObject = Camera.main.transform;
        Vector3 lookDirection = transform.position - targetObject.position;
        Quaternion rot = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, lookSpeed * Time.deltaTime);
    }
    // Update is called once per frame

    private void UpdateCanvasData() {
        if (driverSpeed != null && driverAI != null) {
            driverSpeed.text = driverAI.GetSpeed()+" kmp";
        }

        if (driverPosition != null && driverAI != null) {
            driverPosition.text = "P "+RaceManager.GetPosition(this);
        }
    }
    
    void FixedUpdate()
    {
        LookAtTarget();
        UpdateCanvasData();
        float distance = Vector3.Distance(transform.position, targetObject.position);
        CanvasGroup c = gameObject.GetComponent<CanvasGroup>();

        if (distance > 30 && c != null)
        {
            c.alpha = Mathf.Lerp(c.alpha, 0, 2 * Time.deltaTime);
            // c.enabled = (false);
        }
        else if (c != null) {
            c.alpha = Mathf.Lerp(c.alpha, 1, 2 * Time.deltaTime);
            // c.enabled = (true);
        }
    }

    string FormateName (string name) {
        var res = name.Replace("-"," ");
        var fin = "";
        for (int n = 0; n < res.Length; n++) {
            if (n != 0) {
                if (res[n-1].ToString() == " ") {
                    fin += res[n].ToString().ToUpper();
                    continue;
                } else {
                    fin += res[n].ToString();
                    continue;
                }
            } else {
                fin += res[n].ToString().ToUpper();
                continue;
            }
        }
        return fin;
    }
}
