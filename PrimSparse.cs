/*
Prim's MST Algorithm on Adjacency Lists 
representation Utilizes an Adjacency Linked 
Lists, which is suitable for sparse graphs
*/

using System;
using System.IO;

/* 
Heap code to be adapted for 
Prim's algorithmon adjacency 
lists graph 
*/
public class Heap
{
    // The Heap array and size of the Heap
    private int[] h;
    private int N;  

    // dist[v] is assigned the priority of v
    private int[] dist;

    // hPos[h[k]] is equal to k
    private int[] hPos;       
    
   
    /* The Heap constructor gets passed-in from the Graph:
       ===================================================
            > Maximum heap size
            > Reference to the dist[] array
            > Reference to the hPos[] array
    */
    public Heap(int maxSize, int[] _dist, int[] _hPos) 
    {
        N = 0;
        h = new int[maxSize + 1];
        dist = _dist;
        hPos = _hPos;
    }

    public bool isEmpty() 
    {
        return N == 0;
    }

    public void siftUp(int k)
    {
        int v = h[k];

        h[0] = 0;
        dist[0] = int.MinValue;

        while (dist[v] < dist[h[k / 2]])
        {
            h[k] = h[k / 2];
            hPos[h[k]] = k;
            k = k / 2;
        }

        h[k] = v;
        hPos[v] = k;
    }

    public void siftDown( int k) 
    {
        int v, j;

        v = h[k];
        j = 2 * k;
        dist[0] = int.MinValue;

        while (j <= N)
        {
            if (j + 1 <= N && dist[h[j + 1]] > dist[h[j]])
            {
                j = j = 1;
            }

            if (v >= dist[h[j]])
            {
                break;
            }

            h[k] = h[j];
            k = j;
            j = 2 * k;
        }

        h[k] = v;
        hPos[v] = k;
    }

    public void insert( int x) 
    {
        h[++N] = x;
        siftUp( N);
    }

    public int remove() 
    {   
        int v = h[1];

        // v is now not in the Heap anymore
        hPos[v] = 0; 

        // the NULL node is assigned to the empty position
        h[N+1] = 0;  
        
        h[1] = h[N--];
        siftDown(1);
        
        return v;
    }
}

// The Graph class to support Prim's Algorithm MST
public class Graph
{
    class Node
    {
        public int vertex;
        public int wgt;
        public Node next;
    }

    // V represents is the number of Vertices
    int V;

    // E represents the number of Edges
    int E;

    // adj is the Adjacency Lists Array
    Node[] adj;
    private Node z;

    // 'visited' is used for traveling through the graph
    private int[] visited;

    // Graph constructor
    public Graph(string graphFile)
    {
        int u, v;
        int e, wgt;
        Node t = z;

        StreamReader reader = new StreamReader(graphFile);

        char[] splits = new char[] { ' ', ',', '\t' };
        string line = reader.ReadLine();
        string[] parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);

        // Get the number of Vertices
        V = int.Parse(parts[0]);

        // Get the number of Edges
        E = int.Parse(parts[1]);

        // Create the sentinel Node
        z = new Node();
        z.next = z;

        /*
        Create the adjacency lists
        and initialised them to 
        the sentinel node z
        */
        adj = new Node[V + 1];
        for (v = 1; v <= V; ++v)
            adj[v] = z;

        // Read the Edges from the file
        Console.WriteLine("Reading in the Edges..");
        for (e = 1; e <= E; ++e)
        {
            line = reader.ReadLine();
            parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);

            u = int.Parse(parts[0]);
            v = int.Parse(parts[1]);

            wgt = int.Parse(parts[2]);

            Console.WriteLine("Edge {0}--({1})--{2}", toChar(u), wgt, toChar(v));

            // Puts the Edge into the Adjacency List
            t = new Node();
            t.wgt = wgt;
            t.vertex = u;
            t.next = adj[v];
            adj[v] = t;

            t = new Node();
            t.wgt = wgt;
            t.vertex = v;
            t.next = adj[u];
            adj[u] = t;
        }
    }

    // Convert the Vertex into a Char to display
    private char toChar(int u)
    {
        return (char)(u + 64);
    }

    // Display the representation of the Graph
    public void display()
    {
        int v;
        Node n;

        for (v = 1; v <= V; ++v)
        {
            Console.Write("\nAdjacency List[{0}] >>", toChar(v));
            for (n = adj[v]; n != z; n = n.next)
            {
                Console.Write(" " + toChar(n.vertex) + n.wgt + " >> " );    
            }
            Console.Write(" z ");
        }

        Console.WriteLine();
    }

    /*
    Prim's Algorithm to calculate the
    Minimum Spanning Tree
    */
    int[] MST_Prim(int s)
    {
        int v, u;
        int wgt_sum = 0;
        int[] dist;
        int[] parent;
        int[] hPos;
        int[] seen;

        Node t;

        dist = new int[V + 1];
        parent = new int[V + 1];
        hPos = new int[V + 1];
        seen = new int[V + 1];
           
        for (v = 1; v <= V; ++v)
        {
            // Initialise the Arrays
            dist[v] = int.MaxValue;
            parent[v] = 0;
            hPos[v] = 0;
        }   

        Heap pq = new Heap(V, dist, hPos);

        pq.insert(s);

        dist[s] = 0;

        while (!pq.isEmpty())
        {
            v = pq.remove();

            wgt_sum += dist[v];

            Console.Write("\nAdding to the MST edge {0}--({1})--{2}", toChar(parent[v]), dist[v], toChar(v));

            dist[v] = -dist[v];           

            for (t = adj[v]; t != z; t = t.next)
            {
                u = t.vertex;

                if (t.wgt < dist[u])
                {
                    dist[u] = t.wgt;
                    
                    parent[u] = v;

                    if (hPos[u] == 0 )
                    {    
                        pq.insert(u);
                    }
                    else
                    {
                        pq.siftUp(hPos[u]);
                    }
                }
            }    
        }

        Console.Write("\n\nThe Weight of the MST is {0}\n", wgt_sum);

        return parent;
    }

    public void showMST(int[] mst)
    {
        Console.Write("\n\nMinimum Spanning tree parent array is:\n");

        for (int v = 1; v <= V; ++v)
        {
            Console.Write("{1} > Traverses to > {0}\n", toChar(v), toChar(mst[v]));
        }

        Console.WriteLine("");
    }

    // Main Method
    public static void Main()
    {
        int s = 1;
        int[] mst;
        string fname;

        // Request the user inputs a Text files name
        Console.WriteLine("Enter a filename for your Graph:");

        // Attaches ".txt" to the name to assist the user
        fname = Console.ReadLine() + ".txt";

        Graph g = new Graph(fname);
       
        g.display();
        
        mst = g.MST_Prim(s);
        
        // g.showMST(mst);

        Console.ReadKey();
    }
}


