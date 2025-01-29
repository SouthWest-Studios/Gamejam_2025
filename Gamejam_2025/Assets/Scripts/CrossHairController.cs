using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public static CrosshairController instance;
    public GameObject crosshair; // Arrastra aqu� el punto de apuntado.


    private void Awake()
    {
        if(instance == null) { instance = this; }
    }

    void Start()
    {
        // Aseg�rate de que el punto est� activo al inicio.
        crosshair.SetActive(true);
    }

    public void ShowCrosshair(bool show)
    {
        // Activa o desactiva el punto.
        crosshair.SetActive(show);
    }
}