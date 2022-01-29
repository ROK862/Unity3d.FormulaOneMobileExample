using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeZone : MonoBehaviour
{
    [SerializeField] private Color warningColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private GameObject trackNavigator;
    [SerializeField] private int triggerSpeed = 100;

    private void Start()
    {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.enabled = false;
        }

        if (trackNavigator == null)
        {
            trackNavigator = GameObject.Find("Track-Navigator");
        }
        else
        {
            trackNavigator = null;
        }
    }

    void OnTriggerStay(Collider other)
    {
        VehicleRemote target = other.gameObject.GetComponent<VehicleRemote>();
        if (target != null)
        {
            int targetSpeed = Convert.ToInt32(target.GetSpeed());
            if (targetSpeed > triggerSpeed)
            {
                target.InvokeBrakingSequence();

                if (trackNavigator != null)
                {
                    var render = trackNavigator.GetComponent<MeshRenderer>();
                    if (render)
                    {
                        render.material.color = warningColor;
                    }
                }
            }
        }
        RadorAI ai = other.gameObject.GetComponent<RadorAI>();
        if (ai != null)
        {
            int targetSpeed = Convert.ToInt32(ai.GetSpeed());
            if (targetSpeed > triggerSpeed)
            {
                ai.InvokeBrakingSequence();
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (!trackNavigator)
            return;
        var render = trackNavigator.GetComponent<MeshRenderer>();
        if (render)
        {
            render.material.color = normalColor;
        }
    }
}
