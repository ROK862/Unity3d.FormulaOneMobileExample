using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UIMenuNavigator : MonoBehaviour
{
    [SerializeField]
    public GameObject[] nodes;

    public void SetNodeActive(string name) {
        foreach (var node in nodes) {
            if (node.gameObject.name == name) {
                node.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    } 
}