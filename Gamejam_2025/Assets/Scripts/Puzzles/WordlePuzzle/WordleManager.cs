using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class WordleController : MonoBehaviour
{
    public List<SlotScript> slots; // Lista de todos los slots
    public int numberOfSlots = 5; // N�mero de slots en el juego
    public int maxMoleculeID = 11; // Rango m�ximo de IDs de mol�culas
    public static List<int> correctCombination; // Lista de la combinaci�n correcta
    private PuzzleManager puzzleManager;
    bool allCorrect;
    public GameObject canvas;
    public GameObject finalText;

    public GameObject hint;

    public List<InitialSlotScript> initialSlotScripts;

    public ObjectInteraction trigger;



    public AudioClip[] clips;


    public string[] lines_EN;
    public string[] lines_ES;
    public string[] lines_CA;

    private void Start()
    {
        GenerateRandomCombination();
    }

    private void GenerateRandomCombination()
    {
        correctCombination = new List<int>();

        // Crear una lista de n�meros disponibles
        List<int> availableIDs = new List<int>();
        for (int i = 0; i <= maxMoleculeID; i++)
        {
            availableIDs.Add(i);
        }

        // Elegir n�meros aleatorios sin repetici�n
        for (int i = 0; i < numberOfSlots; i++)
        {
            int randomIndex = Random.Range(0, availableIDs.Count); // Elegir un �ndice aleatorio
            int randomID = availableIDs[randomIndex]; // Obtener el ID
            correctCombination.Add(randomID); // Agregar a la combinaci�n

            // Eliminar el n�mero elegido para evitar duplicados
            availableIDs.RemoveAt(randomIndex);
        }

        Debug.Log("Combinaci�n Correcta: " + string.Join(", ", correctCombination));
    }

    private void Update()
    {
        int count = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                count++;
                allCorrect = true;
            }
        }
        if (count >= 5)
        {
            allCorrect = true;
        }
        else 
        {
            allCorrect = false;
        }
        
            if (allCorrect)
        {
            if(canvas.activeSelf)
            {






                SubtitulosManager.instance.PlayDialogue(lines_ES, lines_EN, lines_CA, clips);

                puzzleManager = FindAnyObjectByType<PuzzleManager>();
                puzzleManager.CompletePuzzle("WordlePuzzle");
                //finalText.SetActive(true);
                trigger.EndFocusTransition();
                //trigger.gameObject.GetComponent<Collider>().enabled = false;
                trigger.enabled = false;
                SetHintActive();
                



                return;
            }
        }
    }

    public void CheckCombination()
    {
         
        for (int i = 0; i < slots.Count; i++)
        {

            var molecule = slots[i].GetComponentInChildren<DraggableMolecule>();
            if (molecule != null)
            {
                //if (molecule.moleculeID == correctCombination[i])
                //{
                //    // Verde: Correcta y en la posici�n correcta
                    slots[i].GetComponent<SlotScript>().SetSlotColor(Color.green);
                    EnableMoleculeInteraction(molecule, true); // Habilitar interacci�n si es correcta
            //    }
            //    else if (correctCombination.Contains(molecule.moleculeID))
            //    {
            //        // Amarillo: Correcta pero en la posici�n incorrecta
            //        slots[i].GetComponent<SlotScript>().SetSlotColor(Color.yellow);
            //        EnableMoleculeInteraction(molecule, true); // Habilitar interacci�n si es correcta pero en lugar incorrecto
            //        allCorrect = false; // Si alguna mol�cula est� en amarillo, no est� completamente correcta
            //    }
            //    else
            //    {
            //        // Rojo: Incorrecta
            //        slots[i].GetComponent<SlotScript>().SetSlotColor(Color.red);
            //        molecule.ReturnToInitialPosition();
            //        EnableMoleculeInteraction(molecule, false); // Deshabilitar interacci�n si es incorrecta
            //        initialSlotScripts[molecule.moleculeID].gameObject.GetComponent<Image>().color = Color.red;
            //        allCorrect = false; // Si alguna mol�cula es incorrecta, no est� completamente correcta
            //    }
            //}
            //else
            //{
            //    // Sin mol�cula en este slot
            //    slots[i].GetComponent<SlotScript>().SetSlotColor(Color.black);
            //    allCorrect = false; // Si hay un slot vac�o, el juego no est� completado
            //}
            }
        }
        
        if (allCorrect)
        {
            
        }
    }

    public void SaveMolecules()
    {
        for(int i = 0; i < slots.Count; i++)
        {
            if(slots[i].GetComponentInChildren<DraggableMolecule>() != null)
            {
                PuzzleManager.wordleFinalList[i] = new Vector3(slots[i].GetComponentInChildren<DraggableMolecule>().transform.position.x, slots[i].GetComponentInChildren<DraggableMolecule>().transform.position.y, slots[i].GetComponentInChildren<DraggableMolecule>().moleculeID);
                PuzzleManager.wordleFinalListPosition[i] = slots[i].GetComponentInChildren<DraggableMolecule>().initialPosition;
            }
            
        }
        
    }
    public void LoadMolecules()
    {
        canvas.SetActive(true);
        DraggableMolecule[] draggableMolecules = Resources.FindObjectsOfTypeAll<DraggableMolecule>();

        for (int i = 0; i < PuzzleManager.wordleFinalList.Count; i++)
        {
            for (int j = 0; j < draggableMolecules.Length; j++) // Evita usar un n�mero fijo (12)
            {
                if (PuzzleManager.wordleFinalList[i].z == draggableMolecules[j].moleculeID)
                {

                    draggableMolecules[j].gameObject.transform.SetParent(slots[i].transform);
                    draggableMolecules[j].initialPosition = PuzzleManager.wordleFinalListPosition[i];
                    draggableMolecules[j].gameObject.GetComponent<RectTransform>().position = new Vector2(PuzzleManager.wordleFinalList[i].x, PuzzleManager.wordleFinalList[i].y);
                    draggableMolecules[j].transform.localScale = new Vector3(0.922f, 0.922f, 0.922f);
                    
                }
            }
        }

        CheckCombination();
        canvas.SetActive(false);
    }

    private T[] FindObjectsOfTypeAll<T>()
    {
        throw new System.NotImplementedException();
    }

    private void EnableMoleculeInteraction(DraggableMolecule molecule, bool enable)
    {
        // Aqu� puedes deshabilitar la interacci�n con las mol�culas que han sido marcadas como rojas
        var draggable = molecule.GetComponent<DraggableMolecule>();
        if (draggable != null)
        {
            draggable.enabled = enable; // Si 'enable' es false, deshabilita el script DraggableMolecule
            
            if (draggable.canvasGroup != null)
            {
                draggable.canvasGroup.blocksRaycasts = enable; // Si 'enable' es false, bloquea los raycasts para que no pueda ser arrastrada
            }
        }
    }

    public void SetHintActive()
    {
        if(hint != null)
        {
            hint.SetActive(true);
        }
        
    }
}
