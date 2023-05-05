using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//possible to change name in the future.
//dialogue create on right click at Unity
namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();

        private Dictionary<string, DialogueNode> nodeLookUp = new Dictionary<string, DialogueNode>();
        //#if is a preprocessor directive in C# that is processed by the compiler.
        //Use for conditionally compile or exclude code based on certain conditions at compile time.
#if UNITY_EDITOR
        //if the dialogue is created the interaction start
        private void Awake()
        {
            if (nodes.Count == 0)
            {
                DialogueNode rootNode = new DialogueNode();
                rootNode.uniqueID = Guid.NewGuid().ToString();
                nodes.Add(rootNode);
            }
        }
#endif
      //use this code when the information is update. This is for look up with ID
        private void OnValidate()
        {
            nodeLookUp.Clear();
            foreach (DialogueNode _node in GetAllNodes())
            {
                nodeLookUp[_node.uniqueID] = _node;
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
            foreach (string childID in _parentNode.children)
            {
                if (nodeLookUp.ContainsKey(childID))
                {
                    yield return nodeLookUp[childID];
                }
            }
        }

        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = new DialogueNode();
            newNode.uniqueID = Guid.NewGuid().ToString();
            parent.children.Add(newNode.uniqueID);
            nodes.Add(newNode);
            OnValidate();
        }
    }
}
