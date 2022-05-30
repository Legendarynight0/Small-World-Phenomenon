using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Numerics;

class Class1
{

    public static Dictionary<Tuple<int, int>, int> we = new Dictionary<Tuple<int, int>, int>(); //to minize memory by half, we order pairs value before add or get
    public static Dictionary<Tuple<int, int>, string> movChain = new Dictionary<Tuple<int, int>, string>();
    public static List<ArrayList> adj = new List<ArrayList>();         //vertex will be converted to int
    public static Dictionary<string, int> map = new Dictionary<string, int>();
    public static Dictionary<int, string> revMap = new Dictionary<int, string>();// we can use it as an array not dictionary
    public static int[] level;
    public static List<ArrayList> lvl;
    public static int[] rsw;
    public static int[] chain;

    static public void Main(String[] args)
    {
        read();
        Console.WriteLine("finished");
    }
    static void read()
    {
        string text = File.ReadAllText(@"Movies122806.txt");
        string[] movies = text.Split('\n');
        string[][] actors = new string[movies.Length][];
        Tuple<int, int> tuple;
        for (int i = 0; i < movies.Length; i++)
        {
            actors[i] = movies[i].Split('/');

        }
        for (int i = 0; i < movies.Length; i++)
        {
            int length = actors[i].Length;
            for (int j = 1; j < length; j++)
            {
                if (!map.ContainsKey(actors[i][j]))
                {
                    adj.Add(new ArrayList());
                    map.Add(actors[i][j], map.Count);
                    revMap.Add(revMap.Count, actors[i][j]);

                }
                for (int k = 1; k < length; k++)
                {
                    //Console.Write("  " + j);

                    if (!map.ContainsKey(actors[i][k]))
                    {
                        adj.Add(new ArrayList());
                        map.Add(actors[i][k], map.Count);
                        revMap.Add(revMap.Count, actors[i][k]);

                    }
                    if (k != j)
                    {                        //Console.WriteLine(map[actors[i][j]]);
                        adj[(map[actors[i][j]])].Add(map[actors[i][k]]);
                    }
                }
            }
            for (int j = 1; j < length; j++)
            {
                for (int k = j + 1; k < length; k++)
                {
                    int min = Math.Min(map[actors[i][j]], map[actors[i][k]]);
                    int max = Math.Max(map[actors[i][j]], map[actors[i][k]]);
                    tuple = new Tuple<int, int>(min, max);
                    if (we.ContainsKey(tuple))
                    {
                        we[tuple]++;
                    }
                    else
                    {
                        movChain.Add(tuple, actors[i][0]);

                        we.Add(tuple, 1);
                    }

                }
            }
        }




        string text2 = File.ReadAllText(@"queries22.txt");
        string[] queries = text2.Split('\n');
        text2 = File.ReadAllText(@"queries22 - Solution.txt");
        string[] sol = text2.Split('\n');
        level = new int[(map.Count + 1)];
        rsw = new int[map.Count + 1];
        for (int i = 0; i < queries.Length - 1; i++)
        {
            string[] values = queries[i].Trim().Split('/');
            int src = map[values[0]];
            int dst = map[values[1]];
            rsw = new int[level.Length];
            bibfs(src, dst);
            /*for (int j = 0; j < lvl.Count; j++)
            {
                Console.WriteLine(lvl[j].Count);
            }*/
            //Console.WriteLine();


            rs(src, dst);
            string[] sol2 = sol[1 + 5 * i].Trim().Split();
            int x = Int32.Parse(sol2[2].Substring(0, sol2[2].Length - 1));
            int y = Int32.Parse(sol2[5].Substring(0, sol2[5].Length));

            if (x == (level[dst] - 1) && y == rsw[src])
            {
                // Console.WriteLine("output " + (level[dst]-1) + " ");
                Console.WriteLine("Case: " + (i + 1) + " Accepted");
            }
            else
            {
                Console.WriteLine("src: " + src + " dest " + dst);
                Console.WriteLine("dst " + (level[dst] - 1) + " " + rsw[dst]);
                Console.WriteLine("source " + (level[src] - 1) + " " + rsw[src]);

                Console.WriteLine("Wrong Answer Case: " + i);
                break;
            }

        }
        Console.WriteLine(" \n \n Finished");
    }
    static void bfs(int source, int dest)
    {
        bool[] visited = new bool[level.Length];
        Queue<int> q = new Queue<int>();
        level = new int[level.Length];
        lvl = new List<ArrayList>();
        lvl.Add(new ArrayList());
        lvl[0].Add(source);
        //created after doing the bfs or while doing it

        level[source] = 1;
        visited[source] = true;
        q.Enqueue(source);
        while (q.Count != 0)
        {
            int v = q.Dequeue();
            if (level[v] >= level[dest] && level[dest] != 0)
            {
                break;
            }
            for (int i = 0; i < adj[v].Count; i++)
            {

                int index = (int)adj[v][i];

                /*int x;
                if (v > index) { x = rsw[v] + we[new Tuple<int, int>(index, v)]; }
                else { x = rsw[v] + we[new Tuple<int, int>(v, index)]; }*/
                if (!visited[index] && level[dest] == 0)
                {
                    if (lvl.Count < level[v] + 1) { lvl.Add(new ArrayList()); }
                    lvl[level[v]].Add(index);
                    level[index] = level[v] + 1;
                    visited[index] = true;
                    q.Enqueue(index);
                }

                /*if (rsw[index] < x && level[v] == level[index] - 1)
                {
                    rsw[index] = x;
                }*/

            }

        }
    }
    static void bibfs(int src, int dst)
    {
        Queue<int> q = new Queue<int>();
        Queue<int> d = new Queue<int>();

        bool[] visited = new bool[level.Length];
        bool[] revvisited = new bool[level.Length];
        level = new int[level.Length];

        lvl = new List<ArrayList>();
        lvl.Add(new ArrayList());
        lvl[0].Add(src);
        visited[src] = true;
        revvisited[dst] = true;
        q.Enqueue(src);
        d.Enqueue(dst);
        level[src] = 1;
        level[dst] = -1;

        bool intersected = false;
        int swaper = 1;
        int revswaper = -1;
        while (q.Count != 0 || d.Count != 0)
        {


            while (q.Count != 0)
            {
                if (swaper < level[q.Peek()] && !intersected)
                {
                    swaper++; break;
                }
                int v = q.Dequeue();
                if (level[v] >= level[dst] && level[dst] > 0)
                {
                    break;
                }
                for (int i = 0; i < adj[v].Count; i++)
                {
                    int index = (int)adj[v][i];
                    if (level[index] < 0 && revvisited[index] && !visited[index])
                    {
                        level[index] = level[v] + 1;
                        intersected = true;
                        visited[index] = true;
                        if (index == dst) { break; }
                        if (lvl.Count < ((level[v]) + 1)) { lvl.Add(new ArrayList()); }
                        lvl[level[v]].Add(index);
                        q.Enqueue(index);
                    }
                    else if (!visited[index] && !intersected)
                    {
                        if (lvl.Count < level[v] + 1) { lvl.Add(new ArrayList()); }
                        lvl[level[v]].Add(index);
                        level[index] = level[v] + 1;
                        visited[index] = true;
                        q.Enqueue(index);
                    }
                }

            }

            if (!intersected)
            {
                while (d.Count != 0)
                {

                    int v = d.Dequeue();

                    if (revswaper > level[v])
                    {
                        revswaper--;
                        break;
                    }
                    for (int i = 0; i < adj[v].Count; i++)
                    {
                        int index = (int)adj[v][i];
                        if (level[index] > 0)
                        {
                            intersected = true;
                            break;
                        }
                        if (!revvisited[index])
                        {
                            level[index] = level[v] - 1;
                            revvisited[index] = true;
                            d.Enqueue(index);
                        }
                    }

                }


            }
            else { break; }



        }


    }
    static void rs(int source, int dest)
    {
        chain = new int[map.Count + 1];
        Queue<int> q = new Queue<int>();
        List<int> visited = new List<int>();
        q.Enqueue(dest);
        while (q.Count != 0)
        {
            int v = q.Dequeue();
            if (v == source) { break; }
            //Console.WriteLine(lvl[level[v]].Count + " ");
            for (int i = 0; i < adj[v].Count; i++)//lvl[level[v] - 2].Count
            {
                /*int u = (lvl[level[v] - 2][i]);
                if (adj[v].Contains(u))
                {

                    if (!visited.Contains(u) && !q.Contains(u))
                    {
                        q.Enqueue(u);
                        visited.Add(u);
                    }
                    int x;
                    if (v > u) { x = rsw[v] + we[new Tuple<int, int>(u, v)]; }
                    else { x = rsw[v] + we[new Tuple<int, int>(v, u)]; }
                    if (rsw[u] < x)
                    {
                        rsw[u] = x;
                        Console.WriteLine();
                    }
                }
*/






                int index = (int)adj[v][i];

                if (lvl[level[v] - 2].Contains(index))
                {

                    if (!visited.Contains(index) && !q.Contains(index))
                    {
                        q.Enqueue(index);
                        visited.Add(index);
                    }

                    int x;
                    if (v > index) { x = rsw[v] + we[new Tuple<int, int>(index, v)]; }
                    else { x = rsw[v] + we[new Tuple<int, int>(v, index)]; }
                    if (rsw[index] < x)
                    {
                        chain[index] = v;
                        rsw[index] = x;
                        // Console.WriteLine(index + " " + x);
                    }
                }

            }
        }
        // Console.WriteLine(revMap[source] + " " + revMap[dest]);
        Queue<string> z = new Queue<string>();
        Queue<string> zt = new Queue<string>();
        int temp = source;
        Tuple<int, int> tempstring = new Tuple<int, int>(Math.Min(temp, chain[temp]), Math.Max(temp, chain[temp]));
        zt.Enqueue(revMap[source]);
        while (temp != dest)
        {
            tempstring = new Tuple<int, int>(Math.Min(temp, chain[temp]), Math.Max(temp, chain[temp]));
            z.Enqueue(movChain[tempstring]);
            zt.Enqueue(revMap[chain[temp]]);
            temp = chain[temp];
        }

        while (zt.Count != 0) { Console.Write(zt.Dequeue() + " => "); }
        Console.WriteLine();
        while (z.Count != 0) { Console.Write(z.Dequeue() + " -> "); }
        Console.WriteLine();
    }
}

struct edge
{
    public int v1;
    public int v2;
}
struct Pair
{
    public int v;
    public int i;
    private int dst;

    public Pair(int dst, int i) : this()
    {
        this.dst = dst;
        this.i = i;
    }
}