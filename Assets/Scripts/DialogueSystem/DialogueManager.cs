using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private RectTransform _dialoguePanel;
    [SerializeField] private RectTransform _choicePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private DialogueSO _loadedDialogue;
    [SerializeField] private DialogueChoiceUI _choicePrefab;
    [SerializeField] private bool _playDialogueOnAwake;
    [SerializeField] private float _textSpeed = 0.02f;
    [SerializeField] private float _bubbleResizeSpeed = 10f;
    private Vector2 _textSize;
    private DialogueSegment _currentSegment;
    private bool _awaitingInput;
    private bool _showChoices;
    private float _sizeAdjustment;

    private void Awake()
    {
        if (_playDialogueOnAwake)
            StartDialogue(_loadedDialogue);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _awaitingInput)
        {
            _awaitingInput = false;
            if (_showChoices)
                StartCoroutine(DisplayChoices());
            else
                NextSegment(0);
        }
        if(_sizeAdjustment < 1)
        {
            _dialoguePanel.sizeDelta = Vector2.LerpUnclamped(_dialoguePanel.sizeDelta, _textSize, _sizeAdjustment);
            _sizeAdjustment = Mathf.Clamp01(_sizeAdjustment + Time.deltaTime * _bubbleResizeSpeed);
        }
    }

    private void NextSegment(int option)
    {
        foreach (Transform child in _choicePanel)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(DisplaySegment(_currentSegment.GetNextDialogue(option)));
    }

    public void StartDialogue(DialogueSO dialogue)
    {
        StartCoroutine(DisplaySegment(dialogue.getStartSegment()));
    }

    private IEnumerator DisplayChoices()
    {
        foreach(Transform child in _choicePanel)
        {
            Destroy(child.gameObject);
        }
        _dialogueText.text = "";
        var choiceText = "";
        List<string> list = _currentSegment.GetChoices();
        for (int i = 0; i < list.Count; i++)
        {
            string choice = list[i];
            choiceText += choice.ToString();
            if(i < list.Count - 1)
            choiceText += "\n\n";
        }
        _textSize = _dialogueText.GetPreferredValues(choiceText);
        _sizeAdjustment = 0;
        var calculatedPosition = new Vector2(_currentSegment.ChoicePosition.x * Screen.width, _currentSegment.ChoicePosition.y * Screen.height);
        if (_dialoguePanel.anchoredPosition != calculatedPosition)
            _dialoguePanel.sizeDelta = Vector2.zero;
        _dialoguePanel.anchoredPosition = calculatedPosition;
        _dialoguePanel.pivot = _currentSegment.ChoicePivot;
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < list.Count; i++)
        {
            string choice = list[i];
            var _choiceText = Instantiate(_choicePrefab, _choicePanel);
            _choiceText.Choice = i;
            _choiceText.Text = choice;
            _choiceText.Button.onClick.AddListener(() => { NextSegment(_choiceText.Choice); });
        }
        _showChoices = false;
    }

    public IEnumerator DisplaySegment(DialogueSegment dialogueSegment)
    {
        if(dialogueSegment == null)
        {
            EndDialogue();
            yield break;
        }
        _currentSegment = dialogueSegment;
        _dialogueText.text = "";
        _textSize = _dialogueText.GetPreferredValues(dialogueSegment.GetSentence());
        _sizeAdjustment = 0;
        var calculatedPosition = new Vector2(dialogueSegment.BubblePosition.x * Screen.width, dialogueSegment.BubblePosition.y * Screen.height);
        if (_dialoguePanel.anchoredPosition != calculatedPosition)
            _dialoguePanel.sizeDelta = Vector2.zero;
        _dialoguePanel.anchoredPosition = calculatedPosition;
        _dialoguePanel.pivot = dialogueSegment.BubblePivot;
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeSentence(dialogueSegment.GetSentence()));
        if (dialogueSegment.HasChoices)
            _showChoices = true;
        _awaitingInput = true;
    }

    private IEnumerator TypeSentence(string sentence)
    {
        string hold = "";
        bool holding = false;
        foreach (var letter in sentence.ToCharArray())
        {
            if (letter == '<')
                holding = true;
            else if (letter == '>')
            {
                _dialogueText.text += hold;
                holding = false;
                hold = "";
            }
            if (holding)
                hold += letter;
            else
            {
                _dialogueText.text += letter;
                yield return new WaitForSeconds(_textSpeed);
            }
        }
    }

    private void EndDialogue()
    {
        //
    }
}
