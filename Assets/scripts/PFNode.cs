using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PFNode : System.IComparable<PFNode>
{
    public int gcost;
    public int hcost;
    public int fcost;

    public Vector2 position;

    public bool open;
    public bool obstructed;
    public PFNode parent;

    public PFNode(Vector2 position)
    {
        this.position = position;
        this.obstructed = Physics2D.OverlapPointAll(position).Where(e => e.GetComponent<MovementInteraction>().movementCost() > 0).Count() > 0;
    }

    public int CompareTo(PFNode b)
    {
        return (this.fcost == b.fcost) ? (this.hcost - b.hcost) : (this.fcost - b.fcost);
    }

    public new string ToString()
    {
        return "Node: " + position + ":\nGCost: " + gcost + "\nHCost: " + hcost + "\nFCost: " + fcost;
    }
}
