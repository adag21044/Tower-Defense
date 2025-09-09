using System;
using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float lookSpead = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(moveX, 0, moveZ);


        transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
    }
}
