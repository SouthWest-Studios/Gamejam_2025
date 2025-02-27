﻿using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;

    public Rewind player;

    public bool isPanelOpen = true;

    public CrosshairController crosshairController;
    public static FirstPersonLook instance;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }


    void Reset()
    {
        // Get the character from the FirstPersonMovement in parents.
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        // Lock the mouse cursor to the game screen.
        Cursor.lockState = CursorLockMode.Locked;

    }

    void LateUpdate()
    {
        if (isPanelOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            crosshairController.ShowCrosshair(false);
        }
        else
        {
            if (!player.isRewinding)
            {
                Cursor.lockState = CursorLockMode.Locked;
                crosshairController.ShowCrosshair(true);

                // Get smooth velocity.
                Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
                Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
                frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
                velocity += frameVelocity;
                velocity.y = Mathf.Clamp(velocity.y, -90, 90);

                // Rotate camera up-down and controller left-right from velocity.
                transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
                character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
            }
        }
    }
}