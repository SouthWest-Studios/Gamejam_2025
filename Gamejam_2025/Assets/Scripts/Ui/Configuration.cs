using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Configuration : MonoBehaviour
{
    public GameObject configPanel;
    public TextMeshProUGUI[] buttonTexts;
    private Animator[] buttonTextAnimators;
    private bool isConfigOpen = false;
    private TextMeshProUGUI currentSelectedButtonText = null;

    private bool[] buttonSelectedStates;
    public Image backgroundImage;
    public GameObject[] panels;

    public FirstPersonLook isPanelOpen;
    public PanelController panelController;

    private void Start()
    {
        buttonTextAnimators = new Animator[buttonTexts.Length];
        buttonSelectedStates = new bool[buttonTexts.Length];

        for (int i = 0; i < buttonTexts.Length; i++)
        {
            buttonTextAnimators[i] = buttonTexts[i].GetComponent<Animator>();
            buttonSelectedStates[i] = false;
        }

        configPanel.SetActive(false);
        SetTextTriggers("Out");

        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isConfigOpen)
            {
                CloseConfig();
            }
            else
            {
                OpenConfig();
            }
        }

        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
        {
            TextMeshProUGUI selectedText = GetSelectedText();
            int selectedIndex = System.Array.IndexOf(buttonTexts, selectedText);

            if (selectedText != currentSelectedButtonText)
            {
                if (currentSelectedButtonText != null)
                {
                    SetTextTrigger(currentSelectedButtonText, "Unselect");
                    int currentIndex = System.Array.IndexOf(buttonTexts, currentSelectedButtonText);
                    buttonSelectedStates[currentIndex] = false;
                }

                currentSelectedButtonText = selectedText;
                buttonSelectedStates[selectedIndex] = true;
                SetTextTrigger(currentSelectedButtonText, "Select");
            }
        }
        else
        {
            SetTextTriggers("In");
        }
    }

    private void OpenConfig()
    {
        isConfigOpen = true;
        configPanel.SetActive(true);
        SetTextTriggers("In");

        if (backgroundImage != null)
        {
            Animator backgroundAnimator = backgroundImage.GetComponent<Animator>();
            if (backgroundAnimator != null)
            {
                backgroundAnimator.SetTrigger("Start");
            }
        }

        isPanelOpen.isPanelOpen = true;
    }

    private void CloseConfig()
    {
        isConfigOpen = false;
        SetTextTriggers("Out");

        if (backgroundImage != null)
        {
            Animator backgroundAnimator = backgroundImage.GetComponent<Animator>();
            if (backgroundAnimator != null)
            {
                backgroundAnimator.SetTrigger("Stop");
            }
        }

        foreach (GameObject panel in panels)
        {
            if (panel.activeSelf)
            {
                Animator panelAnimator = panel.GetComponent<Animator>();
                if (panelAnimator != null)
                {
                    panelAnimator.SetTrigger("Out");
                }
            }
        }

        isPanelOpen.isPanelOpen = false;
        StartCoroutine(CloseConfigWithDelay());
    }

    private IEnumerator CloseConfigWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        configPanel.SetActive(false);
    }

    private void SetTextTriggers(string trigger)
    {
        foreach (Animator textAnimator in buttonTextAnimators)
        {
            if (textAnimator != null)
            {
                textAnimator.ResetTrigger("In");
                textAnimator.ResetTrigger("Out");
                textAnimator.SetTrigger(trigger);
            }
        }
    }

    private void SetTextTrigger(TextMeshProUGUI text, string trigger)
    {
        Animator textAnimator = text.GetComponent<Animator>();
        if (textAnimator != null)
        {
            textAnimator.ResetTrigger("In");
            textAnimator.ResetTrigger("Out");
            textAnimator.SetTrigger(trigger);
        }
    }

    private TextMeshProUGUI GetSelectedText()
    {
        return UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnButtonClick(int index)
    {
        panelController.ActivatePanel(index);
    }
}
