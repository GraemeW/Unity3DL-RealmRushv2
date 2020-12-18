using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Tunables
    [SerializeField] CinemachineVirtualCamera virtualCamera = null;
    [SerializeField] int defaultZoom = 55; // FoV
    [SerializeField] int zoomInZoom = 35; // FoV
    [SerializeField] GameObject gamePlane = null;
    [SerializeField] float throttleZoomFactor = 0.5f;

    // State 
    Vector3 defaultPosition;
    bool zoomIn = false;

    private void Start()
    {
        defaultPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z += Vector3.Distance(transform.position, gamePlane.transform.position);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 translateCamera = new Vector3(worldPosition.x - transform.position.x, worldPosition.y - transform.position.y, worldPosition.z - transform.position.z);
            ZoomToPosition(translateCamera * throttleZoomFactor);
        }
    }

    public void ZoomToPosition(Vector3 translateCamera)
    {
        if (!zoomIn)
        {
            transform.position += translateCamera;
            UpdateFOV(zoomInZoom);
            zoomIn = true;
        }
        else
        {
            transform.position = defaultPosition;
            UpdateFOV(defaultZoom);
            zoomIn = false;
        }

    }

    private void UpdateFOV(int zoomSetting)
    {
        LensSettings lensSettings = virtualCamera.m_Lens;
        lensSettings.FieldOfView = zoomSetting;
        virtualCamera.m_Lens = lensSettings;
    }
}
