using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD
namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    { 
        public string uniqueID;
        public string text;
        public List<string> children = new List<string>();
        public Rect rect = new Rect(0, 0, 200, 100);
    }
=======
[System.Serializable]
public class DialogueNode
{ 
    public string uniqueID;
    public string text;
    public string[] children;
    public Rect rect = new Rect(0, 0, 200, 100);
>>>>>>> main
}

