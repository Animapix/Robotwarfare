using System;
using System.Collections.Generic;

namespace Ánimapix.TileMap
{
    public static class AStar
    {
        private static Node[,] graph;
        private static List<Node> openList;
        private static List<Node> closedList;

        public static List<(int column, int row)> FindPath((int column, int row)start, (int column, int row) target, TileMap tileMap)
        {
            CreateGraph(tileMap);

            Node startNode = graph[start.column, start.row];
            Node targetNode = graph[target.column, target.row];

            openList = new List<Node> { graph[start.column, start.row] };
            closedList = new List<Node>();

            while (openList.Count > 0)
            {
                Node currentNode = openList[0];

                for (int i = 1; i < openList.Count; i++)
                    if (openList[i].f < currentNode.f || (openList[i].f == currentNode.f && openList[i].h < currentNode.h))
                        currentNode = openList[i];

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);

                foreach (Node neighborNode in GetNeighborNodes(currentNode))
                {
                    if (!neighborNode.walkable || closedList.Contains(neighborNode)) continue;

                    float newCostToNeighbor = currentNode.g + GetDistance(currentNode, neighborNode) * (currentNode.border ? 1.5f : 1f);
                    if (newCostToNeighbor < neighborNode.g || !openList.Contains(neighborNode))
                    {
                        neighborNode.g = newCostToNeighbor;
                        neighborNode.h = GetDistance(neighborNode, targetNode);
                        neighborNode.parent = currentNode;

                        if (!openList.Contains(neighborNode))
                            openList.Add(neighborNode);
                    }
                }

            }

            return null;
        }

        private static float GetDistance(Node nodeA, Node nodeB)
        {
            float dColumn = nodeA.column - nodeB.column;
            float dRow = nodeA.row - nodeB.row;

            return MathF.Sqrt(dColumn * dColumn + dRow * dRow);
        }

        private static List<Node> GetNeighborNodes(Node node)
        {
            List<Node> neighborNodes = new List<Node>();

            for (int column = -1; column <= 1; column++)
            {
                for (int row = -1; row <= 1; row++)
                {
                    if (column == 0 && row == 0) continue;

                    int checkColumn = node.column + column;
                    int checkRow = node.row + row;

                    if (Math.Abs(column) + Math.Abs(row) == 2 && !graph[checkColumn, node.row].walkable && !graph[node.column, checkRow].walkable) continue;

                    if (checkColumn >= 0 && checkColumn < graph.GetLength(0) && checkRow >= 0 && checkRow < graph.GetLength(1))
                        neighborNodes.Add(graph[checkColumn, checkRow]);
                }
            }

            return neighborNodes;
        }

        private static List<(int column, int row)> RetracePath(Node startNode, Node endNode)
        {
            List<(int column, int row)> path = new List<(int column, int row)>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add((currentNode.column, currentNode.row));
                currentNode = currentNode.parent;
            }
            path.Add((startNode.column, startNode.row));
            path.Reverse();
            return path;
        }

        private static void CreateGraph(TileMap tileMap)
        {
            
            graph = new Node[tileMap.columns, tileMap.rows];

            for (int column = 0; column < tileMap.columns; column++)
                for (int row = 0; row < tileMap.rows; row++)
                    graph[column, row] = new Node(column, row, tileMap.tiles[column][row].type == Tile.Type.FLOOR, tileMap.tiles[column][row].bitmask > 0);
        }

        private class Node
        {
            public int column;
            public int row;
            public bool walkable;
            public float g = 0;
            public float h = 0;
            public float f { get { return g + h; } }
            public bool border;
            public Node parent;

            public Node(int column, int row, bool walkable, bool border = false)
            {
                this.column = column;
                this.row = row;
                this.walkable = walkable;
                this.border = border;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType())) return false;
                else return column == ((Node)obj).column && row == ((Node)obj).row;
            }

            public override int GetHashCode() => (column, row).GetHashCode();
        }
    }
}
