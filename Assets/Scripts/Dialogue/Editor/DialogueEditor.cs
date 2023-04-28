using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue = null;
        
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
                EditorGUILayout.LabelField(selectedDialogue.name);

            }
        }
    }
}

