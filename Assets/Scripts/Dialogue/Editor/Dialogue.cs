using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor;

//possible to change name in the future.
//dialogue create on right click at Unity
namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField] private Vector2 newNodeOffset = new Vector2(250, 0);

        private Dictionary<string, DialogueNode> nodeLookUp = new Dictionary<string, DialogueNode>();

      //use this code when the information is update. This is for look up with ID
        private void OnValidate()
        {
            //if there is node add one
            if (nodes.Count == 0)
            {
                CreateNode(null); //pass no parent
            }
            
            nodeLookUp.Clear();
            foreach (DialogueNode _node in GetAllNodes())
            {
                nodeLookUp[_node.name] = _node;
            }
        }

        //nodes information
        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            //initial(root) node return
            return nodes[0];
        }

        //Get DialogueNode and find a information by childID
        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode _parentNode)
        {
            foreach (string childID in _parentNode.GetChildren())
            {
                if (nodeLookUp.ContainsKey(childID))
                {
                    yield return nodeLookUp[childID];
                }
            }
        }
        
#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode _nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(_nodeToDelete);
            Undo.DestroyObjectImmediate(_nodeToDelete);
            OnValidate();
            CleanDanglingChildren(_nodeToDelete);
        }
        
        private DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parent != null)
            {
                parent.AddChild(newNode.name);
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }
        
        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private void CleanDanglingChildren(DialogueNode _nodeToDelete)
        {
            foreach (DialogueNode _node in GetAllNodes())
            {
                _node.RemoveChild(_nodeToDelete.name);
            }
        }
        
#endif
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            //if there is node add one
            if (nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }
            
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode _node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(_node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(_node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}
