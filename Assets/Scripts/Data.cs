using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class Data
{
    public string state;
    public int player;
    public List<int> top;
    public List<List<int>> options;
    public List<List<int>> hands;
    // public int[] top;
    // public int[][] options;
    // public int[][] hands;
}