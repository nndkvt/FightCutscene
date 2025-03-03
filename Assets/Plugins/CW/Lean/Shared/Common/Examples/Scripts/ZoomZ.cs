
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ZoomZ : MonoBehaviour
{
    [SerializeField] private float _zoomScale = 10f;
    [SerializeField] private float _minZoom = 0f;
    [SerializeField] private float _maxZoom = 20f;
    [SerializeField] private float _zoomDuration = 0.2f;

    private Coroutine _smoothZoomCoroutine;
    private float _scrollWheelInput;

    private void Update()
    {
        _scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (_scrollWheelInput != 0f)
        {
            StopSmoothZoom();
            StartSmoothZoom();
        }
    }

    private void StopSmoothZoom()
    {
        if (_smoothZoomCoroutine != null)
        {
            StopCoroutine(_smoothZoomCoroutine);
            _smoothZoomCoroutine = null;
        }
    }

    private void StartSmoothZoom()
    {
        float newZoom = transform.localPosition.z + _scrollWheelInput * _zoomScale;
        newZoom = Mathf.Clamp(newZoom, _minZoom, _maxZoom);

        _smoothZoomCoroutine = StartCoroutine(SmoothZoom(newZoom));
    }

    private IEnumerator SmoothZoom(float targetZoom)
    {
        float currentZoom = transform.localPosition.z;      
        float zoomStartTime = Time.time;

        while (Time.time < zoomStartTime + _zoomDuration)
        {
            float zoomProgress = (Time.time - zoomStartTime) / _zoomDuration;
            float smoothZoom = Mathf.Lerp(currentZoom, targetZoom, zoomProgress);

            ApplyZoom(smoothZoom);

            yield return null;
        }

        _smoothZoomCoroutine = null;
    }

    private void ApplyZoom(float zoomValue)
    {
        Vector3 cameraPosition = transform.localPosition;
        cameraPosition.z = zoomValue;
        transform.localPosition = cameraPosition;
    }


}
