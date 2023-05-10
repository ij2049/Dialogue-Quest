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
        [NonSerialized] private GUIStyle nodeStyle;
        [NonSerialized] private DialogueNode draggingNode = null;
        [NonSerialized] private Vector2 draggingOffset;
        [NonSerialized] private DialogueNode creatingNode = null;
        [NonSerialized] private DialogueNode deletingNode = null;
        [NonSerialized] private DialogueNode linkingParentNode = null;
        [NonSerialized] private bool draggingCanvas = false;
        [NonSerialized] private Vector2 draggingCanvasOffset;
        private Vector2 scrollPosition;
        
        private const float canvasSize = 4000;
        private const float backgroundSize = 50;
        
        
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

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;
                Rect textCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, textCoords);
                
                foreach (DialogueNode _node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(_node);
                }
                
                foreach (DialogueNode _node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(_node);
                }
                
                EditorGUILayout.EndScrollView();
                
                if(creatingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Added Dialogue Node");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        //drawing dialogue panel bezier(line), 노드 선 그리기
        private void DrawConnections(DialogueNode _node)
        {
            Vector3 startPosition = new Vector2(_node.GetRect().xMax, _node.GetRect().center.y);
            
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(_node))
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
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
            GUILayout.BeginArea(_node.GetRect(), nodeStyle);
            //EditorGUI.BeginChangeCheck();
                
            _node.SetText(EditorGUILayout.TextField(_node.GetText()));

            GUILayout.BeginHorizontal();

            if(GUILayout.Button("Add"))
            {
                creatingNode = _node;
            }

            DrawLinkButton(_node);

            if(GUILayout.Button("Remove"))
            {
                deletingNode = _node;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea(); //end rectangle grouping size
        }

        private void DrawLinkButton(DialogueNode _node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = _node;
                }
            }

            else if(linkingParentNode == _node)
            {
                if (GUILayout.Button("Cancel"))
                {
                    linkingParentNode = null;
                }
            }
            
            else if (linkingParentNode.GetChildren().Contains(_node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    linkingParentNode.RemoveChild(_node.name);
                    linkingParentNode = null;
                }
            }

            else
            {
                if (GUILayout.Button("Child"))
                {
                    linkingParentNode.AddChild(_node.name);
                    linkingParentNode = null;
                }
            }
        }

        private void ProcessEvents()
        {
            //dragging
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }

                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                //track the position
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
                
                //update scrollPosition
                GUI.changed = true;
            }
            
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;

                GUI.changed = true;
            }
            
            //stop dragging
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 _point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode _node in selectedDialogue.GetAllNodes())
            {
                if (_node.GetRect().Contains(_point))
                {
                    foundNode = _node;
                }
            }
            return foundNode;
        }
    }
}

