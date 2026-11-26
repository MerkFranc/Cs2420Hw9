namespace hw9;

using System;
using System.Collections.Generic;

// ========== Part A ==========
// A1
//      0 → [1, 4]
//      1 → [0, 2, 3]
//      2 → [1, 3]
//      3 → [1, 2, 4]
//      4 → [0, 3, 5]
//      5 → [4]
//
// A2
//          0   1   2   3   4   5
//      0   1   0   0   0   1   0
//      1   1   0   1   1   0   0
//      2   0   1   0   1   0   0
//      3   0   1   1   0   1   0
//      4   1   0   0   1   0   1
//      5   0   0   0   0   1   0
//      # of 1s = 14
//      It is 2 x E because each edge gets recorded twice
//
// A3
//      Vertex  Degree
//      0       2
//      1       3
//      2       2
//      3       3
//      4       3
//      5       1
//       sum = 14
//
// A4
//      Step    Vertex taken out    Vertices added to queue     Queue after step
//      Start                       0                           [0]
//      1       0                   1, 4                        [1, 4]
//      2       1                   2, 3                        [4, 2, 3]
//      3       4                   5                           [2, 3, 5]
//      4       2                   none                        [3, 5]
//      5       3                   none                        [5]
//      6       5                   none                        []
//      Final BFS order: 0, 1, 4, 2, 3, 5
//
// A5
//      Final DFS order: 0, 1, 2, 3, 4, 5
//      Call stack at deepest vertex(5): 0 → 1 → 2 → 3 → 4 → 5 (top)
//
// A6
//      Shortest path from 2 to 5: 2 → 3 → 4 → 5
//      Traversal that finds it: BFS
//      Why: BFS moves out level by level, finding the shortest path. DFS just follows one path as deep as it can go with no gaurantee of it being the shortest.
//
// A7
//      Cycle: 0 → 1 → 3 → 4 → 0
//
// A8
//      0 → [1, 4]
//      1 → [0, 2]
//      2 → [1]
//      4 → [0, 5]
//      5 → [4]
//
//      Still connected? Yes
//      Explanation: There is still one connection to each vertex, but no cycles.
//
// A9
//      Adjacency matrix or adjacency list? Adjacency list
//      Memory needed for each:
//          Adjacency matrix (O(v^2)): 10000^2 = 100000000
//          Adjacency list (O(V + E)): V = 10000, E = 10000 * 30 = 300000; 10000 + 300000 = 310000
//      Choice: I would choose an adjacency list because it requires less memory.

// ========== Part B ==========
class Graph
{
    private Dictionary<int, List<int>> adj = new Dictionary<int, List<int>>();

    public void AddVertex(int v)
    {
        if (!adj.ContainsKey(v))
            adj[v] = new List<int>();
    }

    public void AddEdge(int u, int v)
    {
        if (u == v)
            return;
        AddVertex(u);
        AddVertex(v);
        if (adj[u].Contains(v))
            return;
        adj[u].Add(v);
        adj[v].Add(u);
    }

    public bool HasEdge(int u, int v)
    {
        return adj.ContainsKey(u) && adj[u].Contains(v);
    }

    public void RemoveEdge(int u, int v)
    {
        if (!adj.ContainsKey(u) || !adj.ContainsKey(v))
            return;

        adj[u].Remove(v);
        adj[v].Remove(u);
    }

    public void RemoveVertex(int v)
    {
        if (!adj.ContainsKey(v))
            return;

        foreach (int n in adj[v])
            adj[n].Remove(v);

        adj.Remove(v);
    }

    public void Print()
    {
        foreach (var kvp in adj)
        {
            Console.WriteLine($"{kvp.Key}: [{string.Join(", ", kvp.Value)}]");
        }
    }

    public List<int> BFS(int start)
    {
        if (!adj.ContainsKey(start))
            return new List<int>();
        return BFSFrom(start, new HashSet<int>());
    }

    private List<int> BFSFrom(int start, HashSet<int> visited)
    {
        List<int> order = new List<int>();
        Queue<int> queue = new Queue<int>();

        visited.Add(start);
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            order.Add(current);

            foreach (int next in adj[current])
            {
                if (!visited.Contains(next))
                {
                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }
        }
        return order;
    }

    public List<int> DFS(int start)
    {
        List<int> order = new List<int>();
        if (!adj.ContainsKey(start))
            return order;
        DFSVisit(start, new HashSet<int>(), order);
        return order;
    }

    private void DFSVisit(int current, HashSet<int> visited, List<int> order)
    {
        visited.Add(current);
        order.Add(current);

        foreach (int next in adj[current])
        {
            if (!visited.Contains(next))
                DFSVisit(next, visited, order);
        }
    }

    public List<int> ShortestPath(int from, int to)
    {
        List<int> path = new List<int>();
        if (!adj.ContainsKey(from) || !adj.ContainsKey(to))
            return path;

        Dictionary<int, int> parent = new Dictionary<int, int>();
        HashSet<int> visited = new HashSet<int>();
        Queue<int> queue = new Queue<int>();

        visited.Add(from);
        queue.Enqueue(from);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            if (current == to)
                break;

            foreach (int next in adj[current])
            {
                if (!visited.Contains(next))
                {
                    visited.Add(next);
                    parent[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        if (!visited.Contains(to))
            return path;

        int step = to;
        path.Add(step);
        while (step != from)
        {
            step = parent[step];
            path.Add(step);
        }
        path.Reverse();
        return path;
    }

    public int CountPieces()
    {
        HashSet<int> visited = new HashSet<int>();
        int count = 0;
        foreach (int v in adj.Keys)
        {
            if (!visited.Contains(v))
            {
                count++;
                MarkPiece(v, visited);
            }
        }
        return count;
    }

    private void MarkPiece(int current, HashSet<int> visited)
    {
        visited.Add(current);
        foreach (int next in adj[current])
            if (!visited.Contains(next))
                MarkPiece(next, visited);
    }

    public bool HasCycle()
    {
        HashSet<int> visited = new HashSet<int>();
        foreach (int v in adj.Keys)
        {
            if (!visited.Contains(v) && CycleFrom(v, -1, visited))
                return true;
        }
        return false;
    }

    private bool CycleFrom(int current, int parent, HashSet<int> visited)
    {
        visited.Add(current);
        foreach (int next in adj[current])
        {
            if (!visited.Contains(next))
            {
                if (CycleFrom(next, current, visited))
                    return true;
            }
            else if (next != parent)
                return true;
        }
        return false;
    }
}

class Program
{
    static string Show(List<int> list)
    {
        return "[" + string.Join(", ", list) + "]";
    }

    static void Main(string[] args)
    {
        Graph graph = new Graph();

        graph.AddEdge(0, 1);
        graph.AddEdge(1, 2);
        graph.AddEdge(0, 3);
        graph.AddEdge(3, 4);
        graph.AddEdge(1, 4);
        graph.AddEdge(6, 7);
        graph.AddEdge(4, 6);
        graph.AddEdge(2, 5);
        graph.AddEdge(4, 5);
        graph.AddVertex(8);

        Console.WriteLine($"BFS(0): {Show(graph.BFS(0))}");
        Console.WriteLine($"DFS(0): {Show(graph.DFS(0))}");
        Console.WriteLine($"BFS(42): {Show(graph.BFS(42))}");
        Console.WriteLine($"ShortestPath(0, 7): {Show(graph.ShortestPath(0, 7))}");
        Console.WriteLine($"ShortestPath(0, 8): {Show(graph.ShortestPath(0, 8))}");
        Console.WriteLine($"HasEdge(4, 1): {graph.HasEdge(4, 1)}");
        Console.WriteLine($"CountPieces(): {graph.CountPieces()}");
        Console.WriteLine($"HasCycle(): {graph.HasCycle()}");

        Console.WriteLine();
        graph.Print();
        Console.WriteLine("AddEdge(5, 4)");
         graph.AddEdge(5, 4);
        Console.WriteLine("AddEdge(7, 7)");
        graph.AddEdge(7, 7);
        graph.Print();
        Console.WriteLine();
        Console.WriteLine("RemoveEdge(1, 4)");
        graph.RemoveEdge(1, 4);
        Console.WriteLine("RemoveEdge(4, 5)");
        graph.RemoveEdge(4, 5);
        Console.WriteLine($"HasCycle(): {graph.HasCycle()}");
        Console.WriteLine();
        Console.WriteLine("RemoveVertex(4)");
        graph.Print();
        Console.WriteLine($"CountPieces(): {graph.CountPieces()}");

    }
}
