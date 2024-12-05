using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class LineManager : MonoBehaviour
{
    private Dictionary<Tuple<Object, Object>, LineRenderer> _lines;
    private LineRenderer currLineRenderer;
    private void Awake()
    {
        // Получаем компонент LineRenderer
        currLineRenderer = GetComponent<LineRenderer>();
    }
    
    
}
