using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class SimonGameManager : MonoBehaviour
{
    public List<ButtonController> buttons;
    public float lightOnTime = 0.5f;
    public float lightOffTime = 0.3f;

    private List<int> pattern = new List<int>();
    private int currentStep = 0;
    private bool playerTurn = false;
    private bool gameStarted = false;
    private bool gameFinished = false;
    public TraumaInducer shakeEffect;
    public float errorShakeIntensity = 0.3f;

    public TextMeshPro finalText;

    private PuzzleManager puzzleManager;

    private int seed;

    private Puzzle puzzle;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonSound;
    public AudioClip errorSound;
    public AudioClip victorySound;

    private readonly float[] buttonPitches = { 0.6f, 0.8f, 1f, 1.2f };

    void Start()
    {
        puzzle = PuzzleManager.instance.GetPuzzle("SimonSaysPuzzle");

        
        if (puzzle != null)
        {
            seed = puzzle.seed;

            Random.InitState(seed);
        }
    }

    private IEnumerator StartGame()
    {
        yield return StartCoroutine(FlashLightsAtStart());
        GenerateNextStep();
        yield return PlayPattern();
        
    }

    private IEnumerator FlashLightsAtStart()
    {
        playerTurn = false;
        float[] pitches = { 0.3f, 0.6f, 1.0f };
        
        for (int i = 0; i < 3; i++)
        {
            foreach (var button in buttons)
            {
                button.ActivateLight();
            }
            if (buttonSound != null)
            {
                audioSource.pitch = pitches[i];
                audioSource.PlayOneShot(buttonSound);
            }
            yield return new WaitForSeconds(0.3f);

            foreach (var button in buttons)
            {
                button.DeactivateLight();
            }
            yield return new WaitForSeconds(0.3f);
        }
        
        audioSource.pitch = 1f; // Restauramos el pitch original
    }

    private void GenerateNextStep()
    {

        int randomIndex = Random.Range(0, buttons.Count);
        if (pattern.Count < 6)
        {
            pattern.Add(randomIndex);
        }
    }

    private IEnumerator PlayPattern()
    {
        playerTurn = false;

        foreach (int index in pattern)
        {
            buttons[index].ActivateLight();
            PlayButtonSound(index);
            yield return new WaitForSeconds(lightOnTime);
            buttons[index].DeactivateLight();
            yield return new WaitForSeconds(lightOffTime);
        }

        playerTurn = true;
        currentStep = 0; // Resetea el progreso del jugador para la nueva ronda
    }

    private void PlayButtonSound(int buttonIndex)
    {
        if (buttonSound != null && buttonIndex < buttonPitches.Length)
        {
            audioSource.pitch = buttonPitches[buttonIndex];
            audioSource.PlayOneShot(buttonSound);
            //audioSource.pitch = 1f;
        }
    }

    public void ButtonPressed(int buttonIndex)
    {
        if (!gameStarted)
        {
            int random1 = Random.Range(0, 1000);
            Random.InitState(seed);
            int random2 = Random.Range(0, 1000);
            StartCoroutine(StartGame());
            gameStarted = true;
        }
        else
        {
            if(gameFinished) return;
            if (!playerTurn) return;

            if (pattern[currentStep] == buttonIndex)
            {
                PlayButtonSound(buttonIndex);
                currentStep++;
                if (currentStep >= pattern.Count)
                {
                    if (pattern.Count < 6)
                    {
                        // El jugador complet� el patr�n correctamente
                        StartCoroutine(NextRound());
                    }
                    else
                    {
                        if (victorySound != null)
                        {
                            audioSource.PlayOneShot(victorySound);
                        }
                        puzzleManager = FindObjectOfType<PuzzleManager>();
                        puzzleManager.CompletePuzzle("SimonSaysPuzzle");
                        gameFinished = true;

                        int indexCode = 0;
                        finalText.gameObject.SetActive(true);
                        finalText.text = PuzzleManager.numpadFinalCode[indexCode].ToString() + "***";

                        char[] auxList = PuzzleManager.numpadActualCode.ToCharArray();
                        auxList[indexCode] = PuzzleManager.numpadFinalCode[indexCode];
                        string finalCharacters = "";
                        for (int i = 0; i < auxList.Length; i++)
                        {
                            finalCharacters += auxList[i].ToString();
                        }
                        PuzzleManager.numpadActualCode = finalCharacters;

                        for(int i = 0; i < buttons.Count; i++)
                        {
                            buttons[i].gameObject.SetActive(false);
                        }
                        
                    }

                    
                }
            }
            else
            {
                if (errorSound != null)
                {
                    audioSource.PlayOneShot(errorSound);
                }
                if(shakeEffect != null)
                {
                    shakeEffect.MaximumStress = errorShakeIntensity;
                    shakeEffect.Delay = 0f;
                    shakeEffect.InduceTrauma();
                    Debug.Log("�Game Over! Reiniciando...");
                    StartCoroutine(RestartGame());
                }
                
            }
        }
    }

    private IEnumerator NextRound()
    {
        yield return new WaitForSeconds(1f);
        GenerateNextStep();
        yield return PlayPattern();
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1f);
        pattern.Clear();
        Random.InitState(seed);
        currentStep = 0;
        StartCoroutine(StartGame());
    }
}
