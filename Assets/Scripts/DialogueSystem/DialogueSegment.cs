using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using XNode;
using static UnityEngine.GraphicsBuffer;
using static XNodeEditor.NodeEditor;
using XNodeEditor;
using UnityEngine.Serialization;

public class DialogueSegment : Node
{
    [Input(backingValue = ShowBackingValue.Never)]
    [SerializeField] private DialogueSegment _origin;

    [SerializeField] private string _name;

    [TextArea]
    [SerializeField] private string _sentence;

    [SerializeField] private bool _hasChoices;

    [Output(dynamicPortList = true, connectionType = ConnectionType.Override)]
    [TextArea]
    [SerializeField] private List<string> _choices;

    [SerializeField] private Vector2 _bubblePosition;
    [SerializeField] private Vector2 _bubblePivot;

    [SerializeField] private Vector2 _choicePosition;
    [SerializeField] private Vector2 _choicePivot;

    [Output(connectionType = ConnectionType.Override)]
    [SerializeField] private DialogueSegment _next;

    public bool HasChoices { get => _hasChoices;}
    public Vector2 BubblePivot { get => _bubblePivot; }
    public Vector2 BubblePosition { get => _bubblePosition; }
    public Vector2 ChoicePosition { get => _choicePosition; set => _choicePosition = value; }
    public Vector2 ChoicePivot { get => _choicePivot; set => _choicePivot = value; }

    public override object GetValue(NodePort port)
    {
        return this;
    }

    public string GetSentence()
    {
        return _sentence;
    }

    public bool HasOrigin()
    {
        return GetInputPort("_origin").ConnectionCount > 0;
    }

    public DialogueSegment GetNextDialogue(int option)
    {
        Debug.Log(option);
        if(HasNextDialogue())
            if (HasChoices)
            {
                var choiceString = "_choices " + option.ToString();
                return (DialogueSegment)GetOutputPort(choiceString).Connection.node.GetValue(GetOutputPort(choiceString).Connection);
            }
            else
            {
                return (DialogueSegment)GetOutputPort("_next").Connection.node.GetValue(GetOutputPort("_next").Connection);
            }
        else
            return null;
    }

    public List<string> GetChoices()
    {
        return _choices;
    }

    public bool HasNextDialogue()
    {
        if (_hasChoices)
        {
            for (int i = 0; i < _choices.Count; i++)
            {
                if (GetOutputPort("_choices " + i.ToString()).ConnectionCount > 0)
                    return true;
            }
            return false;
        }
        else
            return GetOutputPort("_next").ConnectionCount > 0;
    }

    private void OnValidate()
    {
        name = _name;
    }
}


[CustomNodeEditor(typeof(DialogueSegment))]
public class SimpleNodeEditor : NodeEditor
{
    private DialogueSegment dialogueSegment;

    public override void OnBodyGUI()
    {
        if (dialogueSegment == null) dialogueSegment = target as DialogueSegment;

        // Update serialized object's representation
        serializedObject.Update();

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_origin"));
        List<GUILayoutOption> layoutOptions = new List<GUILayoutOption> { GUILayout.Width(200), GUILayout.Height(50)};
        if (!dialogueSegment.HasOrigin())
            EditorGUILayout.LabelField("No origin seguments.\nThis segment will begin dialogue.\nMake sure there's only one", options: layoutOptions.ToArray());
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_name"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_sentence"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_bubblePosition"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_bubblePivot"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_hasChoices"));
        if (dialogueSegment.HasChoices)
        {
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_choices"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_choicePosition"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_choicePivot"));
        }
        else
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_next"));
        if (!dialogueSegment.HasNextDialogue())
            EditorGUILayout.LabelField("No connected segument.\nThis segment will end dialogue.", options: layoutOptions.ToArray());
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
}