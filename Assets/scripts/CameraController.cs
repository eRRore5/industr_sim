using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotatespeed = 10.0f;
    public float speed = 10.0f;
    public float zoomspeed = 10.0f;

    private float _mult = 1f;

    private void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        float rotate = 0f;

        if (Input.GetKey(KeyCode.Q))
            rotate = -1f;
        else if (Input.GetKey(KeyCode.E))
            rotate = 1f;

        _mult = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;

        // Rotate the camera
        transform.Rotate(Vector3.up * rotatespeed * Time.deltaTime * rotate * _mult, Space.World);

        // Translate the camera
        Vector3 moveDirection = new Vector3(hor, 0, ver);
        transform.Translate(moveDirection * Time.deltaTime * _mult * speed, Space.Self);

        // Zoom the camera
        transform.position += 70f * transform.up * zoomspeed * Time.deltaTime * _mult * Input.GetAxis("Mouse ScrollWheel");
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Clamp(transform.position.y, -20f, 40f),
            transform.position.z
        );
    }
}
