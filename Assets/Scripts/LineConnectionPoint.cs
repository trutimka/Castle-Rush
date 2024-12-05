using System.Collections.Generic;
using UnityEngine;

public class LineConnectionPoint : MonoBehaviour
{
    [SerializeField] private Player owner;
    [SerializeField] private List<Line> lines;
    
    public Player Owner => owner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetOwner(Player new_owner)
    {
        owner = new_owner;
    }

    public void AddLine(Line line)
    {
        lines.Add(line);
    }

    public void RemoveLine(Line line)
    {
        lines.Remove(line);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
