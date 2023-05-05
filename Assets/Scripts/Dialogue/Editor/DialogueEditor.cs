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
<<<<<<< HEAD
        [NonSerialized] private GUIStyle nodeStyle;
        [NonSerialized] private DialogueNode draggingNode = null;
        [NonSerialized] private Vector2 draggingOffset;
        [NonSerialized] private DialogueNode creatingNode = null;
=======
        private GUIStyle nodeStyle;
        private DialogueNode draggingNode = null;
        private Vector2 draggingOffset;
>>>>>>> main
        
        //show editor from window panel
        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            //utility true/false -> using once(true) or not(false)
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }
        
        //double click on the project folder, interaction happens
        [OnOpenAsset(1)]
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

        //text & ID showing on the editor (text info can change on the editor) 
        private void OnGUI()
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No dialogue selected");
            }

            else
            {
                ProcessEvents();
<<<<<<< HEAD
                foreach (DialogueNode _node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(_node);
                }
                foreach (DialogueNode _node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(_node);
                }
                if(creatingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Added Dialogue Node");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
            }
        }

        //drawing dialogue panel bezier(line), 노드 선 그리기
        private void DrawConnections(DialogueNode _node)
        {
            Vector3 startPosition = new Vector2(_node.rect.xMax, _node.rect.center.y);
            
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(_node))
            {
                Vector3 endPosition = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + controlPointOffset, 
                    endPosition - controlPointOffset, 
                    Color.white, null, 4f);
            }
        }

        private void DrawNode(DialogueNode _node)
        {

            //rectangle grouping size
            GUILayout.BeginArea(_node.rect, nodeStyle);
            EditorGUI.BeginChangeCheck();
                
            string newText = EditorGUILayout.TextField(_node.text);
                
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
                _node.text = newText;
            }

            if(GUILayout.Button("+"));
            {
                creatingNode = _node;
            }

            GUILayout.EndArea(); //end rectangle grouping size
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

=======
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

>>>>>>> main
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

