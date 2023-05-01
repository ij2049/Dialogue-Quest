using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private Quest quest;


    private void Start()
    {
        foreach (string task in quest.GetTasks())
        {
            Debug.Log($"Has Task{task}.");
        }
    }
}
