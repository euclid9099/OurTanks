using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private static List<PFNode> openNodes = new List<PFNode>();
    private static List<PFNode> closeNodes = new List<PFNode>();

    public static Vector2 FindPath(Vector2 startPos, Vector2 endPos)
    {
        openNodes.Clear();
        closeNodes.Clear();

        endPos = new Vector2(Mathf.RoundToInt(endPos.x), Mathf.RoundToInt(endPos.y));
        startPos = new Vector2(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y));

        PFNode curnode;

        addOrUpdate(null, startPos.x, startPos.y, 0, endPos);

        int nodecount = 0;
        while(openNodes.Count > 0 && nodecount < 1000)
        {
            nodecount++;

            curnode = openNodes[0];

            for (int i = 1; i < openNodes.Count; i++)
            {
                if (openNodes[i].CompareTo(curnode) < 0)
                {
                    curnode = openNodes[i];
                }
            }

            if (curnode.obstructed)
            {
                openNodes.Remove(curnode);
            } else
            {

                if (curnode.position.x == endPos.x && curnode.position.y == endPos.y)
                {
                    Vector2 lastposition = curnode.position;
                    while (curnode.parent.parent != null)
                    {
                        if(Vector2.Angle(curnode.position - curnode.parent.position, curnode.parent.position - curnode.parent.parent.position) > 15)
                        {
                            lastposition = curnode.parent.position;
                        }
                        curnode = curnode.parent;
                    }
                    return lastposition;
                } else
                {
                    bool north, east, south, west;
                    //add surrounding nodes
                    north = addOrUpdate(curnode, curnode.position.x, curnode.position.y - 1, curnode.gcost + 10, endPos).obstructed;
                    east = addOrUpdate(curnode, curnode.position.x + 1, curnode.position.y, curnode.gcost + 10, endPos).obstructed;
                    south = addOrUpdate(curnode, curnode.position.x, curnode.position.y + 1, curnode.gcost + 10, endPos).obstructed;
                    west = addOrUpdate(curnode, curnode.position.x - 1, curnode.position.y, curnode.gcost + 10, endPos).obstructed;

                    if (!west && !north) addOrUpdate(curnode, curnode.position.x - 1, curnode.position.y - 1, curnode.gcost + 14, endPos);
                    if (!east && !north) addOrUpdate(curnode, curnode.position.x + 1, curnode.position.y - 1, curnode.gcost + 14, endPos);
                    if (!west && !south) addOrUpdate(curnode, curnode.position.x - 1, curnode.position.y + 1, curnode.gcost + 14, endPos);
                    if (!east && !south) addOrUpdate(curnode, curnode.position.x + 1, curnode.position.y + 1, curnode.gcost + 14, endPos);

                    openNodes.Remove(curnode);
                    closeNodes.Add(curnode);
                }

            }
        }

        //return nothing if no path was found
        return Vector2.zero;
    }

    private static PFNode addOrUpdate(PFNode parent, float posX, float posY, int gcost, Vector2 endPos)
    {
        PFNode node = openNodes.Find(e => e.position.x == posX && e.position.y == posY);
        if (node == null)
        {
            node = closeNodes.Find(e => e.position.x == posX && e.position.y == posY);
            if (node == null)
            {
                node = new PFNode(new Vector2(posX, posY));
                node.gcost = gcost;
                node.hcost = (int)Mathf.Min(Mathf.Abs(endPos.x - node.position.x), Mathf.Abs(endPos.y - node.position.y)) * 14
                + ((int)Mathf.Max(Mathf.Abs(endPos.x - node.position.x), Mathf.Abs(endPos.y - node.position.y))
                - (int)Mathf.Min(Mathf.Abs(endPos.x - node.position.x), Mathf.Abs(endPos.y - node.position.y))) * 10;

                node.fcost = node.hcost + node.gcost;
                node.parent = parent;
                openNodes.Add(node);

            }
            return node;
        } else if (node.gcost > gcost)
        {
            node.gcost = gcost;
            node.fcost = node.gcost + node.hcost;
            node.parent = parent;
        }

        return node;
    }
}
