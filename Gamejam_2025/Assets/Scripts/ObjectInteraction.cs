using System.Collections;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public GameObject canvasToOpen;
    private bool playerIsNear = false;
    public FirstPersonLook cameraFirstPerson;
    public Camera focusCamera;
    public float transitionSpeed = 2.0f; // Velocidad de transición
    private bool isTransitioning = false;

    public bool isCanvasToOpen = false;

    private Vector3 initialPlayerCameraPosition;
    private Quaternion initialPlayerCameraRotation;
    private bool hasSavedInitialTransform = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            Debug.Log("El jugador está cerca del cubo.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            Debug.Log("El jugador se alejó del cubo.");
        }
    }

    private void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            if (focusCamera != null && !isTransitioning)
            {
                StartCoroutine(StartFocusTransition(focusCamera));
            }
            else if (canvasToOpen != null)
            {
                Cursor.lockState = CursorLockMode.None;
                canvasToOpen.SetActive(true);
                cameraFirstPerson.isPanelOpen = true;
            }
        }

        if(canvasToOpen!=null)
        {
            if (playerIsNear && Input.GetKeyDown(KeyCode.Escape) && canvasToOpen.activeSelf)
            {
                EndFocusTransition();
            }
        }
    }

    IEnumerator StartFocusTransition(Camera targetCamera)
    {
        isTransitioning = true;

        // Desactivar el control de la cámara del jugador
        cameraFirstPerson.enabled = false;

        // Guardar la posición y rotación iniciales de la cámara del jugador
        if (!hasSavedInitialTransform)
        {
            initialPlayerCameraPosition = cameraFirstPerson.transform.position;
            initialPlayerCameraRotation = cameraFirstPerson.transform.rotation;
            hasSavedInitialTransform = true;
        }

        Transform playerCameraTransform = cameraFirstPerson.transform;
        Vector3 startPosition = playerCameraTransform.position;
        Quaternion startRotation = playerCameraTransform.rotation;

        // Posición y rotación de la cámara objetivo
        Vector3 targetPosition = targetCamera.transform.position;
        Quaternion targetRotation = targetCamera.transform.rotation;

        float elapsedTime = 0f;

        
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;

            
            playerCameraTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            playerCameraTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);

            yield return null;
        }

        
        cameraFirstPerson.gameObject.SetActive(false);
        targetCamera.gameObject.SetActive(true);

        
        if (isCanvasToOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            canvasToOpen.SetActive(true);
            cameraFirstPerson.isPanelOpen = true;
            cameraFirstPerson.crosshairController.gameObject.SetActive(false);
        }

        isTransitioning = false;
    }

    public void EndFocusTransition()
    {
        if (focusCamera == null || isTransitioning) return;

        
        if (isCanvasToOpen)
        {
            canvasToOpen.SetActive(false);
            cameraFirstPerson.isPanelOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
            
        }

        
        focusCamera.gameObject.SetActive(false);
        cameraFirstPerson.gameObject.SetActive(true);
        cameraFirstPerson.crosshairController.gameObject.SetActive(true);
        cameraFirstPerson.transform.position = initialPlayerCameraPosition;
        cameraFirstPerson.transform.rotation = initialPlayerCameraRotation;

        cameraFirstPerson.enabled = true;

        
        isTransitioning = false;
        hasSavedInitialTransform = false;
        
    }
}
