using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class DoubleClickUI : MonoBehaviour
{
    [Header("Configuraci�n")]
    private Button myButton;
    [SerializeField] private float doubleClickTime = 0.3f; // Tiempo m�ximo entre clics

    [Header("Eventos")]
    public UnityEvent onSingleClick;  // Evento para un solo clic
    public UnityEvent onDoubleClick;  // Evento para doble clic
    public UnityEvent onClickOutside; // Evento cuando se hace clic fuera del bot�n

    public Color tintSelect;
    private Color originalColor;

    private float lastClickTime = 0f;

    void Start()
    {
        if (myButton == null)
        {
            myButton = GetComponent<Button>(); // Intenta obtener el bot�n autom�ticamente
        }
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnClick);
            if (myButton.targetGraphic)
            {
                originalColor = myButton.targetGraphic.color;
            }
            
        }
        else
        {
            Debug.LogError("No se encontr� un bot�n en este GameObject.");
        }
    }

    void Update()
    {
        // Detectar clic fuera del bot�n
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement(myButton.gameObject))
        {
            onClickOutside?.Invoke();
            Debug.Log("Clic fuera del bot�n detectado!");
        }
    }

    void OnClick()
    {
        if (Time.time - lastClickTime < doubleClickTime)
        {
            onDoubleClick?.Invoke(); // Llama al evento de doble clic
            Debug.Log("Doble clic detectado!");
        }
        else
        {
            onSingleClick?.Invoke(); // Llama al evento de un solo clic
        }
        lastClickTime = Time.time;
    }

    // Verifica si el cursor est� sobre el bot�n o cualquier otro elemento UI
    private bool IsPointerOverUIElement(GameObject target)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == target)
            {
                return true; // El clic ocurri� sobre el bot�n
            }
        }
        return false; // El clic ocurri� fuera del bot�n
    }


    public void ApplyTint()
    {
        if (!myButton.targetGraphic) { return; }
       
            myButton.targetGraphic.color = tintSelect;
        myButton.targetGraphic.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = tintSelect;
    }
    public void UnapplyTint()
    {
        if (!myButton.targetGraphic) { return; }
        myButton.targetGraphic.color = originalColor;
        myButton.targetGraphic.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = originalColor;
    }


}
