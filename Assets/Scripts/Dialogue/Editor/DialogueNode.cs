using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Dialogue
{ 
    public class DialogueNode : ScriptableObject
    { 
        [SerializeField] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(0, 0, 200, 100);

        public Rect GetRect()
        {
            return rect;
        }

        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }
        
#if UNITY_EDITOR
        public void SetPosition(Vector2 _newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = _newPosition;
        }

        public void SetText(string _newText)
        {
            if (_newText != text)
            {
                Undo.RecordObject(this, "Update Dialogue Node");
                text = _newText;
            }
        }

        public void AddChild(string _childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(_childID);
        }

        public void RemoveChild(string _childID)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(_childID);
        }
#endif
        
    }
}