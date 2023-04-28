using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//possible to change name in the future.
//dialogue create on right click at Unity
namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> nodes;
        
        //#if is a preprocessor directive in C# that is processed by the compiler.
        //Use for conditionally compile or exclude code based on certain conditions at compile time.
#if UNITY_EDITOR
        //if the dialogue is created the interaction start
        private void Awake()
        {
            if (nodes.Count == 0)
            {
                nodes.Add(new DialogueNode());                
            }
        }
#endif
        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }
    }
}
