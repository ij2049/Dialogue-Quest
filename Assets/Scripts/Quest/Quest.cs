using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "IEnumerables Primer/Quest", order = 0)]
public class Quest : ScriptableObject
{
    [SerializeField] private string[] tasks;
    
    public IEnumerable<string> GetTasks()
    {
        yield return "Task 1";
        Debug.LogFormat("Do Some work");
        yield return "Task 2";
    }
}
