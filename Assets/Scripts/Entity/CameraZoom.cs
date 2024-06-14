using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private float zoom;
    private float zoomMultiplier = 50f;
    private float minZoom = 8f;
    public float maxZoom = 15f;
    private float velocity = 0f;
    private float smoothTime = 0.25f;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera cam;
    void Start()
    {
        zoom = cam.m_Lens.OrthographicSize;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * zoomMultiplier;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        cam.m_Lens.OrthographicSize = Mathf.SmoothDamp(cam.m_Lens.OrthographicSize, zoom, ref velocity, smoothTime);

    }
}
