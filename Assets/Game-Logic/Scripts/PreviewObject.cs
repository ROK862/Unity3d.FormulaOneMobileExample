using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PreviewObject : MonoBehaviour
{
    public Vector3 ePrevPosition = Vector3.zero;
    public Vector3 ePosDelta = Vector3.zero;
    public float touchSensitivity = 0.4F;
    public float zoomSensitivity = 0.5F;

    public float minFieldView = 60.0F;
    public float maxFieldView = 90.0F;
    public Camera mainCamera;

    private float zoomInput = 0.5F;
    private int childIndex = 0;
    private int childCount = 0;

    // Update is called once per frame
    void Update()
    {
        return;

        Touch touch = new Touch();
        var touching = false;
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            touching = true;
        }


        if (Input.GetMouseButton(0) && !touching)
        {
            ePosDelta = Input.mousePosition - ePrevPosition;
            transform.Rotate(transform.up, -Vector3.Dot(ePosDelta, Camera.main.transform.right), Space.World);
        } else if (touching) {
            if (Input.touchCount == 1) {
                ePosDelta = Input.mousePosition - ePrevPosition;
                transform.Rotate(transform.up, -(Vector3.Dot(ePosDelta, Vector3.right)) * touchSensitivity, Space.World);
            } else if (Input.touchCount == 2)
            {
                ePosDelta = Input.mousePosition - ePrevPosition;
                zoomInput = (Vector3.Dot(ePosDelta, Vector3.right));
                if (mainCamera != null) {
                    float viewDelta = mainCamera.fieldOfView;

                    viewDelta += (zoomInput * zoomSensitivity);

                    viewDelta = (viewDelta < minFieldView) ? minFieldView : (viewDelta > maxFieldView) ? maxFieldView : viewDelta;

                    mainCamera.fieldOfView = viewDelta;
                }
            }
        }
        ePrevPosition = Input.mousePosition;
    }

    public void NextObject()
    {
        childIndex++;
        for (var x = 0; x < transform.childCount; x++)
        {
            transform.GetChild(x).gameObject.SetActive(false);
        }
        if (childIndex >= transform.childCount)
        {
            childIndex = 0;
        }
        transform.GetChild(childIndex).gameObject.SetActive(true);
    }

    public void PrevObject()
    {
        childIndex--;
        for (var x = 0; x < transform.childCount; x++)
        {
            transform.GetChild(x).gameObject.SetActive(false);
        }
        if (childIndex < 0)
        {
            childIndex = transform.childCount-1;
        }
        transform.GetChild(childIndex).gameObject.SetActive(true);
    }
}
