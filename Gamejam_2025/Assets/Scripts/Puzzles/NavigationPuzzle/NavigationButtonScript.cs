using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NavigationButtonScript : MonoBehaviour
{
    public List<TextMeshPro> textList;
    private int index = 0;

    private void Start()
    {
        // Aseguramos que solo el elemento inicial est� activo.
        for (int i = 0; i < textList.Count; i++)
        {
            textList[i].gameObject.SetActive(i == index);
        }
    }

    public void pressButton(int sumatori)
    {
        // Desactivamos el elemento actual antes de cambiar el �ndice.
        textList[index].gameObject.SetActive(false);

        // Actualizamos el �ndice y aplicamos la l�gica c�clica.
        index += sumatori;

        if (index < 0)
        {
            index = textList.Count - 1; // Pasamos al �ltimo elemento.
        }
        else if (index >= textList.Count)
        {
            index = 0; // Volvemos al primer elemento.
        }

        // Activamos el nuevo elemento.
        textList[index].gameObject.SetActive(true);
    }
}
