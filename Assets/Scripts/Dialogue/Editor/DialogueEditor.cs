using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Graphs;
using UnityEditor.MPE;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue = null;
        private GUIStyle nodeStyle;
        private DialogueNode draggingNode = null;
        private Vector2 draggingOffset;
        
        //show editor from window panel
        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            //utility true/false -> using once(true) or not(false)
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }
        
        //double click on the project folder, interaction happens
        [OnOpenAssetAttribute(1)]
        public static bool OpenDialogue(int instanceID, int line)
        {
            //get ID and try to find the object. if there is no object, return null
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                //open dialogue editor
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        //similar as a start function but OnEnable function occurs every object active.
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        //void name can be change
        //give an information to the that is selected on the opened Dialogue Editor
        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No dialogue selected");
            }

            else
            {
                ProcessEvents();
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    //rectangle grouping size
                    GUILayout.BeginArea(node.rect, nodeStyle);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.LabelField("Node : ", EditorStyles.whiteLabel);
                    
                    string newText = EditorGUILayout.TextField(node.text);
                    string newUniqueID = EditorGUILayout.TextField(node.uniqueID);
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
                        node.text = newText;
                        node.uniqueID = newUniqueID;
                    }

                    foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
                    {
                        EditorGUILayout.LabelField(childNode.text);
                    }

                    GUILayout.EndArea(); //end rectangle grouping size
                }
            }
        }
        
        private void ProcessEvents()
        {
            //dragging
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                }
            }
            
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                //track the position
                Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                draggingNode.rect.position = Event.current.mousePosition + draggingOffset;
                GUI.changed = true;
            }
            
            //stop dragging
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 _point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode _node in selectedDialogue.GetAllNodes())
            {
                if (_node.rect.Contains(_point))
                {
                    foundNode = _node;
                }
            }
            return foundNode;
        }
    }
}

