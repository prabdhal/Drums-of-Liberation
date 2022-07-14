using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    #endregion

    private Queue<Sprite> dialogueIcons = new Queue<Sprite>();
    private Queue<string> dialogueLines = new Queue<string>();

    // UI Fields
    public GameObject dialoguePanel;
    [SerializeField] Button continueButton;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Image dialogueIcon;
    private bool isTyping = false;

    /// <summary>
    /// When player completes the queue set of dialogues.
    /// </summary>
    public delegate void OnDialogueComplete();
    public event OnDialogueComplete OnDialogueCompleteEvent;
    /// <summary>
    /// When player exits the dialogue with/without completing it.
    /// </summary>
    public delegate void OnDialogueExit();
    public event OnDialogueExit OnDialogueExitEvent;

    private void Start()
    {
        if (continueButton == null) continueButton = dialoguePanel.transform.Find("ContinueButton").GetComponent<Button>();
        if (dialogueText == null) dialogueText = dialoguePanel.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        if (dialogueIcon == null) dialogueIcon = dialoguePanel.transform.Find("DialogueIcon").GetComponent<Image>();
        continueButton.onClick.AddListener(delegate { ContinueDialogue(); });
        dialoguePanel.SetActive(false);

        PlayerControls.Instance.OnInteractEvent += ContinueDialogue;
    }

    public void AddNewDialogue(Dialogue dialogue)
    {
        dialogueLines.Clear();
        dialogueIcons.Clear();
        foreach (Sentence sentence in dialogue.sentences)
        {
            dialogueLines.Enqueue(sentence.sentence);
            dialogueIcons.Enqueue(sentence.icon);
        }

        CreateDialogue();
    }

    private void CreateDialogue()
    {
        //nameText.text = npcName;
        dialoguePanel.SetActive(true);
        ContinueDialogue();
        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
    }

    public void ContinueDialogue()
    {
        Debug.Log("Continue!");
        if (isTyping) return;
        isTyping = true;
        if (dialogueLines.Count == 0)
        {
            CompleteDialogue();
            return;
        }

        string line = dialogueLines.Dequeue();
        Sprite icon = dialogueIcons.Dequeue();
        dialogueIcon.sprite = icon;

        StartCoroutine(TypeSentence(line));
    }

    public IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        isTyping = false;
    }

    public void ExitDialogue()
    {
        dialoguePanel.SetActive(false);
        FireOnDialogueExitEvent();
        ClearDialoguePanel();
    }

    public void CompleteDialogue()
    {
        dialoguePanel.SetActive(false);
        FireOnDialogueCompleteEvent();
        ClearDialoguePanel();
    }

    private void ClearDialoguePanel()
    {
        isTyping = false;
        dialogueLines.Clear();
    }

    private void FireOnDialogueCompleteEvent()
    {
        OnDialogueCompleteEvent?.Invoke();
        RemoveAllListenersFromOnDialogueCompleteEvent();
    }

    private void FireOnDialogueExitEvent()
    {
        OnDialogueExitEvent?.Invoke();
        RemoveAllListenersFromOnDialogueExitEvent();
    }

    public void RemoveAllListenersFromOnDialogueCompleteEvent()
    {
        OnDialogueCompleteEvent = null;
    }

    public void RemoveAllListenersFromOnDialogueExitEvent()
    {
        OnDialogueExitEvent = null;
    }
}
