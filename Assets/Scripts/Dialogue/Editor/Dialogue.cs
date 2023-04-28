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

        //if the dialogue is created the interaction start
        private void Awake()
        {
            if (nodes.Count == 0)
            {
                nodes.Add(new DialogueNode());                
            }
        }
    }
}


