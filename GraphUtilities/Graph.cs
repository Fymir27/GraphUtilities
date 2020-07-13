using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GraphUtilities
{
    /// <summary>
    /// Gives out unique IDs by using a unique counter for every class
    /// </summary>
    /// <typeparam name="T">Class for which to give IDs</typeparam>
    public class UniqueID<T>
    {
        private static int counter = 0;

        public int Value { get; } = counter++;

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// Represents a vertex in the graph
    /// Keeps a list of incident edges which are NOT freely modifiable
    /// Data of type TVertexData is freely accessible/modifiable
    /// An instance of this can only be created by calling Graph.AddVertex(TVertexData)
    /// </summary>
    public class Vertex
    {
        public List<Edge> Edges { get; private set; } = new List<Edge>();

        public UniqueID<Vertex> ID = new UniqueID<Vertex>();

        public override string ToString()
        {
            return $"V{ID}";
        }

        public virtual bool SameType(Vertex other)
        {
            return GetType() == other.GetType();
        }

        public virtual Vertex Clone()
        {
            var original = this;
            return new Vertex()
            {
                Edges = original.Edges.ToList()
            };
        }

        public Edge GetEdgeTo(Vertex other)
        {
            return Edges.FirstOrDefault(e => e.GetOtherVertex(this) == other);
        }
    }

    /// <summary>
    /// Vertex that also contains data
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    public class DataVertex<TData> : Vertex
    {
        public TData Data { get; set; }
        public DataVertex(TData data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return /*base.ToString() + ":" + */ Data.ToString();
        }

        public override Vertex Clone()
        {
            return new DataVertex<TData>(this.Data);
        }
    }

    /// <summary>
    /// Represents an edge in the graph
    /// Keeps track of start/end vertices which are NOT freely modifiable
    /// </summary>
    public class Edge
    {
        public Vertex V1 { get; internal set; } = null;
        public Vertex V2 { get; internal set; } = null;

        public UniqueID<Edge> ID = new UniqueID<Edge>();

        public Edge()
        {

        }

        public Edge(Vertex first, Vertex second)
        {
            Init(first, second);
        }

        public void Init(Vertex first, Vertex second)
        {
            if (first == null || second == null)
            {
                throw new System.ArgumentException("Edges with null Vertices are not allowed!");
            }

            if (first == second)
            {
                throw new System.ArgumentException("Edges with same start and end vertex are not allowed!");
            }

            V1 = first;
            V2 = second;
        }

        public bool AttachedTo(Vertex v)
        {
            return V1 == v || V2 == v;
        }

        public Vertex GetOtherVertex(Vertex vertex)
        {
            if (vertex == V1)
                return V2;
            if (vertex == V2)
                return V1;
            throw new System.ArgumentException("Vertex is not adjacent to this edge!");
        }

        public void ReplaceVertex(Vertex oldVertex, Vertex newVertex)
        {
            if (oldVertex == V1)
            {
                V1.Edges.Remove(this);
                V1 = newVertex;
            }
            else if (oldVertex == V2)
            {
                V2.Edges.Remove(this);
                V2 = newVertex;
            }
            else
                throw new System.ArgumentException("Vertex is not adjacent to this edge!");
        }

        public override string ToString()
        {
            return $"E{ID}";
        }

        public virtual bool SameType(Edge other)
        {
            return GetType() == other.GetType();
        }

        public virtual Edge Clone()
        {
            return new Edge();
        }
    }

    /// <summary>
    /// Edge that also contains data
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    public class DataEdge<TData> : Edge
    {
        public TData Data { get; set; }

        public DataEdge(TData data)
        {
            Data = data;
        }

        public DataEdge(Vertex first, Vertex second, TData data) : base(first, second)
        {
            Data = data;
        }
        public override string ToString()
        {
            return Data.ToString();
        }

        public override Edge Clone()
        {
            return new DataEdge<TData>(this.Data);
        }
    }

    public static class BitArrayExtensions
    {
        public static List<int> TrueBitsIndices(this BitArray bits)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < bits.Length; i++)
            {
                if(bits[i])
                {
                    result.Add(i);
                }
            }
            return result;
        }
    }

    // DISCLAIMER: if you use this class, don't change the graph
    // while instances are still used, otherwise you might get problems
    public struct Cycle
    {
        public readonly Vertex StartingVertex;
        public int EdgeCount { get => edges.Count; }

        private Graph graph;
        private List<Edge> edges;
        private BitArray fingerprint;
        private Vertex lastAddedVertex;

        public Cycle(Graph graph, Vertex startingVertex)
        {
            this.graph = graph;
            StartingVertex = startingVertex;
            lastAddedVertex = startingVertex;
            edges = new List<Edge>();
            fingerprint = new BitArray(graph.Edges.Count);
        }

        public void AddEdge(Edge e)
        {
            if (!graph.Edges.Contains(e))
            {
                throw new ArgumentException("Can't add edge from a different graph!");
            }
            if (edges.Contains(e))
            {
                throw new ArgumentException("Edge already in cycle!");
            }
            if (!e.AttachedTo(lastAddedVertex))
            {
                throw new ArgumentException("Edge doesn't continue cycle!");
            }
            edges.Add(e);
            fingerprint.Set(graph.Edges.IndexOf(e), true);
            lastAddedVertex = e.GetOtherVertex(lastAddedVertex);
        }

        public List<Vertex> Vertices()
        {
            var result = new List<Vertex>();
            var v = StartingVertex;
            edges.ForEach(e =>
            {
                result.Add(v);
                v = e.GetOtherVertex(v);
            });
            return result;
        }

        public Cycle MergeWith(Cycle other)
        {
            BitArray newFingerprint = new BitArray(fingerprint);
            newFingerprint.Xor(other.fingerprint);

            List<int> edgeIndices = newFingerprint.TrueBitsIndices();

            if(edgeIndices.Count == 0)
            {
                return new Cycle(graph, null);
            }

            var edgesToAdd = new List<Edge>();
            foreach (var index in edgeIndices)
            {
                edgesToAdd.Add(graph.Edges[index]);
            }
           
            Vertex v = edgesToAdd[0].V1;
            Cycle newCycle = new Cycle(graph, v);
            while (edgesToAdd.Count > 0)
            {
                Edge next = v.Edges.First(edgesToAdd.Contains);
                newCycle.AddEdge(next);
                edgesToAdd.Remove(next);
                v = next.GetOtherVertex(v);
            }

            return newCycle;
        }

        public List<List<Edge>> Overlap(Cycle other)
        {
            BitArray overlapFingerprint = new BitArray(fingerprint);
            overlapFingerprint.And(other.fingerprint);
            var overlappingEdgeIndices = overlapFingerprint.TrueBitsIndices();

            if(overlappingEdgeIndices.Count == 0)
            {
                return new List<List<Edge>>();
            }

            var overlappingEdges = new List<Edge>();
            foreach (var index in overlappingEdgeIndices)
            {
                overlappingEdges.Add(graph.Edges[index]);
            }

            var result = new List<List<Edge>>();
            var curSegment = new List<Edge>();
            Vertex v = overlappingEdges[0].V1;

            while(overlappingEdges.Count > 0)
            {
                Edge next = v.Edges.FirstOrDefault(overlappingEdges.Contains);

                if(next == null)
                {
                    result.Add(curSegment);
                    curSegment = new List<Edge>();
                    v = overlappingEdges[0].V1;
                    continue;
                }

                curSegment.Add(next);
                overlappingEdges.Remove(next);
                v = next.GetOtherVertex(v);
            }

            result.Add(curSegment);
            return result;
        }

        public override bool Equals(object obj)
        {           
            if (obj == null || !(obj is Cycle))
                return false;

            var other = (Cycle)obj;

            if (graph != other.graph)
                return false;

            // if they're from the same graph, they should have same length bitarrays
            for (int i = 0; i < fingerprint.Length; i++)
            {
                if (fingerprint[i] != other.fingerprint[i])
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// represents an element-wise mapping from query graph (pattern) to matched sub-graph
    /// </summary>
    public class MatchResult
    {
        /// <summary>
        /// Maps from pattern Vertex to matched Vertex in the searched graph
        /// </summary>
        public Dictionary<Vertex, Vertex> Vertices;

        /// <summary>
        /// Maps from pattern Edge to matched Edge in the searched graph
        /// </summary>
        public Dictionary<Edge, Edge> Edges;

        public MatchResult()
        {
            Vertices = new Dictionary<Vertex, Vertex>();
            Edges = new Dictionary<Edge, Edge>();
        }
    }

    /// <summary>
    /// Represents an UNDIRECTED mathematical graph consisting of vertices and edges
    /// Vertices and edges are saved as objects and can each contain arbitrary data
    /// Allows having vertices that are not connected to anything
    /// Does not yet support vertices having an edge to themselves (no loops!)
    /// </summary>
    /// <typeparam name="TVertexData">Type of vertex data</typeparam>
    /// <typeparam name="TEdgeData">Type of edge data</typeparam>
    public class Graph
    {
        /// <summary>
        /// List of vertices in the graph
        /// </summary>
        public List<Vertex> Vertices { get; private set; } = new List<Vertex>();

        /// <summary>
        /// Set of Edges in the graph
        /// </summary>
        public List<Edge> Edges { get; private set; } = new List<Edge>();

        /// <summary>
        /// Random number generator
        /// Set this if you want a custom seed
        /// </summary>
        public Random Random { get; set; } = new Random();

        // ----------------------------------------------------------------------------///

        /// <summary>
        /// Adds a vertex to the graph
        /// </summary>
        /// <param name="data">Data saved in vertex</param>
        /// <returns>Reference to the created vertex</returns>
        public VType AddVertex<VType>(VType vertex) where VType : Vertex
        {
            if (vertex == null)
            {
                throw new System.ArgumentException("Can't add a vertex that is null!");
            }
            if (Vertices.Contains(vertex))
            {
                throw new System.ArgumentException("Can't add a vertex twice! " + vertex);
            }
            Vertices.Add(vertex);
            return vertex;
        }

        /// <summary>
        /// Adds an edge to the graph
        /// </summary>
        /// <param name="first">First vertex</param>
        /// <param name="second">Second vertex</param>
        /// <param name="data">Data saved in edge</param>
        /// <returns>Reference to the created edge</returns>
        public void AddEdge(Edge edge)
        {
            AssertEdge(edge);
            if(Edges.Contains(edge))
            {
                throw new System.ArgumentException("Edge already present in Graph!");
            }
            Edges.Add(edge);
            edge.V1.Edges.Add(edge);
            edge.V2.Edges.Add(edge);
        }

        /// <summary>
        /// removes a vertex from the graph (and it's edges)
        /// </summary>
        /// <param name="vertex">Vertex to remove</param>
        public void RemoveVertex(Vertex vertex)
        {
            AssertVertex(vertex);

            Vertices.Remove(vertex);

            // remove all edges that would now be dangling
            foreach (var edge in vertex.Edges)
            {
                edge.GetOtherVertex(vertex).Edges.Remove(edge);
            }

            vertex.Edges.Clear();
        }

        /// <summary>
        /// removes an edge from the graph
        /// </summary>
        /// <param name="edge">Edge to remove</param>
        public void RemoveEdge(Edge edge)
        {
            AssertEdge(edge);
            if (!Edges.Contains(edge))
            {
                throw new System.ArgumentException("Edge not in Graph!");
            }
            Edges.Remove(edge);
            // remove the edge from both end vertices
            edge.V1.Edges.Remove(edge);
            edge.V2.Edges.Remove(edge);
        }

        /// <summary>
        /// Calculates the neighbouring vertices of a vertex
        /// </summary>
        /// <param name="vertex">Query vertex</param>
        /// <returns>List of neighbouring vertices</returns>
        public List<Vertex> GetNeighbours(Vertex vertex)
        {
            AssertVertex(vertex);

            var result = new List<Vertex>();

            foreach (var edge in vertex.Edges)
            {
                result.Add(edge.GetOtherVertex(vertex));
            }

            return result;
        }

        /// <summary>
        /// Checks if two vertices are adjacent (connected by an edge)
        /// </summary>
        /// <param name="first">First vertex</param>
        /// <param name="second">Second vertex</param>
        /// <returns>True if the vertices are connected</returns>
        public bool Adjacent(Vertex first, Vertex second)
        {
            AssertVertex(first);
            AssertVertex(second);

            foreach (var edge in first.Edges)
            {
                if (edge.GetOtherVertex(first) == second)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Finds a vertex of the same type as the query vertex in the graph
        /// </summary>
        /// <param name="queryVertex">vertex of type you want to find</param>
        /// <returns>vertex of the same type in graph</returns>
        public Vertex FindVertexLike(Vertex queryVertex)
        {
            AssertVertex(queryVertex);
            return Vertices.Find(v => queryVertex.SameType(v));
        }

        /// <summary>
        /// Finds ALL vertices of the same type as query vertex in the graph
        /// </summary>
        /// <param name="queryVertex">vertex of type you want to find</param>
        /// <returns>all vertices of type you want to find</returns>
        public IEnumerable<Vertex> FindVerticesLike(Vertex queryVertex)
        {
            System.Type type = queryVertex.GetType();
            return Vertices.Where(v => queryVertex.SameType(v));
        }

        /// <summary>
        /// Finds a pattern (subgraph) in the host graph (this graph)
        /// Returns null on failing to match
        /// </summary>
        /// <param name="pattern">pattern graph to look for</param>
        /// <param name="verbose">set true for writing trace to console</param>
        /// <returns>a mapping from pattern to host graph for every vertex + edge</returns>
        public MatchResult FindPattern(Graph pattern, bool randomMatch = false, bool verbose = false)
        {
            if (pattern == null || pattern.Vertices.Count == 0)
            {
                throw new System.ArgumentException("Invalid pattern!");
            }

            var patternVertex = pattern.Vertices[0]; // TODO: make it random?
            return MatchRecursively(patternVertex, randomMatch, verbose);
        }

        /// <summary>
        /// Validates a vertex. Throws an exception if it is null or not in the graph
        /// </summary>
        /// <param name="vertex">vertex to validate</param>
        public void AssertVertex(Vertex vertex)
        {
            if (vertex == null)
            {
                throw new System.ArgumentException("Vertex is null!");
            }

            if (!InGraph(vertex))
            {
                throw new System.ArgumentException("Vertex not contained in this graph!");
            }
        }

        /// <summary>
        /// Validates an edge. Throws an exception if either vertex is invalid
        /// </summary>
        /// <param name="edge">edge to validate</param>
        public void AssertEdge(Edge edge)
        {            
            AssertVertex(edge.V1);
            AssertVertex(edge.V2);
        }

        /// <summary>
        /// checks if vertex is in graph
        /// </summary>
        /// <param name="vertex">vertex to check</param>
        /// <returns>wether vertex is in graph</returns>
        private bool InGraph(Vertex vertex)
        {
            return Vertices.Contains(vertex);
        }

        /// <summary>
        /// recursive strategy for FindPattern()
        /// </summary>
        /// <param name="startVertex">pattern vertex to start with</param>
        /// <param name="verbose">set true for writing trace to console</param>
        /// <returns>a mapping from pattern to host graph for every vertex + edge</returns>
        private MatchResult MatchRecursively(Vertex startVertex, bool randomMatch, bool verbose)
        {
            var possibleFirstMatches = FindVerticesLike(startVertex);

            if (randomMatch)
            {
                Shuffle(ref possibleFirstMatches);
            }

            List<Edge> visitedPatternEdges = new List<Edge>();
            List<Edge> matchedEdges = new List<Edge>();
            MatchResult result = new MatchResult();


            foreach (var firstMatch in possibleFirstMatches)
            {
                if (IterateVertex(startVertex, firstMatch))
                {
                    return result;
                }
                visitedPatternEdges.Clear();
                matchedEdges.Clear();
                result = new MatchResult();
            }

            return null;

            bool IterateVertex(Vertex patternVertex, Vertex matchVertex)
            {
                if (verbose)
                {
                    System.Console.WriteLine($"Pattern: {EnumerableToString(result.Vertices.Keys.Select(v => v.ID))} -> {patternVertex.ID}");
                    System.Console.WriteLine($"Matched: {EnumerableToString(result.Vertices.Values.Select(v => v.ID))} -> {matchVertex.ID}");
                    System.Console.WriteLine("---");
                }

                if (!result.Vertices.ContainsKey(patternVertex))
                {
                    if (result.Vertices.Values.Contains(matchVertex))
                        return false;

                    result.Vertices.Add(patternVertex, matchVertex);
                }
                else if (result.Vertices[patternVertex] != matchVertex)
                {
                    return false;
                }
                
                var unvisitedPatternEdges = patternVertex.Edges
                    .Where(e => !visitedPatternEdges.Contains(e));

                if (randomMatch)
                {
                    Shuffle(ref unvisitedPatternEdges);
                }

                foreach (var patternEdge in unvisitedPatternEdges)
                {
                    if (visitedPatternEdges.Contains(patternEdge))
                        continue;

                    visitedPatternEdges.Add(patternEdge);

                    var nextPatternVertex = patternEdge.GetOtherVertex(patternVertex);

                    var unvisitedEdges = matchVertex.Edges
                        .Where(e =>
                            !matchedEdges.Contains(e) &&
                            patternEdge.SameType(e) &&
                            nextPatternVertex.SameType(e.GetOtherVertex(matchVertex))
                        );

                    if (unvisitedEdges.Count() == 0)
                    {
                        visitedPatternEdges.Remove(patternEdge);
                        result.Vertices.Remove(patternVertex);
                        return false;
                    }

                    if (randomMatch)
                    {
                        Shuffle(ref unvisitedEdges);
                    }

                    bool success = false;
                    foreach (var e in unvisitedEdges)
                    {
                        //could in the meantime already be matched
                        if (matchedEdges.Contains(e))
                            continue;

                        Vertex nextMatchedVertex = e.GetOtherVertex(matchVertex);
                        matchedEdges.Add(e);
                        success = IterateVertex(nextPatternVertex, nextMatchedVertex);
                        if (success == true)
                        {
                            result.Edges.Add(patternEdge, e);
                            break;
                        }
                        else
                        {
                            matchedEdges.Remove(e);
                        }
                    }
                    if (!success)
                    {
                        visitedPatternEdges.Remove(patternEdge);
                        result.Vertices.Remove(patternVertex);
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// finds a pattern in the graph and replaces it with another graph
        /// </summary>
        /// <param name="pattern">pattern to find</param>
        /// <param name="replacement">replacement graph</param>
        /// <param name="directMapping">identity mapping from pattern to replacement Vertices</param>
        /// <param name="randomMatch">set true to find pattern randomly in graph</param>
        /// <returns>true on success; false otherwise</returns>
        public bool Replace(Graph pattern, Graph replacement, Dictionary<Vertex, Vertex> directMapping, bool randomMatch = false)
        {
            var matchResult = FindPattern(pattern, randomMatch);

            if (matchResult == null)
                return false;

            // go through all matched vertices to find which ones are to be directly replaced by another
            foreach (var vertexPair in matchResult.Vertices)
            {
                Vertex patternVertex = vertexPair.Key;
                Vertex matchVertex = vertexPair.Value;

                bool mappedDirectly = directMapping.TryGetValue(patternVertex, out Vertex replacementVertex);

                if (!mappedDirectly)
                {
                    RemoveVertex(matchVertex);
                    continue;
                }

                // tie edges of host graphs to replacement vertex
                var unmatchedEdges = matchVertex.Edges.Where(e => !matchResult.Edges.Values.Contains(e)).ToArray();
                replacementVertex.Edges.AddRange(unmatchedEdges);
                foreach (var e in unmatchedEdges)
                {
                    e.ReplaceVertex(matchVertex, replacementVertex);
                }

                // don't use this.RemoveVertex(), because it would remove lose edges
                // however, they aren't lose, because we just reconnected them to the replacement vertex
                Vertices.Remove(matchVertex);
                AddVertex(replacementVertex);
            }

            // Add the rest of the replacement edges (that aren't directly replacing anything)
            foreach (var vertex in replacement.Vertices)
            {
                if (!Vertices.Contains(vertex))
                {
                    AddVertex(vertex);
                }
            }

            return true;
        }

        /// <summary>
        /// finds a pattern in the graph and replaces it with another graph according to the rule
        /// </summary>
        /// <param name="rule">Rule by which to replace</param>
        /// <param name="randomMatch">set true to find pattern randomly in graph</param>
        /// <returns>true on success; false otherwise</returns>
        public bool Replace(ReplacementRule rule, bool randomMatch = false)
        {
            return Replace(rule.Pattern, rule.Replacement, rule.Mapping, randomMatch);
        }

        public static string EnumerableToString(System.Collections.IEnumerable enumerable)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder("[");
            foreach (var elem in enumerable)
            {
                result.Append(elem.ToString()).Append(",");
            }
            if (result.Length == 1)
                return "[]";
            return result.Replace(',', ']', result.Length - 1, 1).ToString();
        }

        // based on Fisher-Yates shuffle
        public void Shuffle<T>(ref IEnumerable<T> enumerable)
        {
            List<T> list = enumerable.ToList();
            for (int n = list.Count() - 1; n >= 0; n--)
            {
                int r = Random.Next(n + 1);
                T value = list[r];
                list[r] = list[n];
                list[n] = value;
            }
            enumerable = list;
        }      

        public List<Cycle> GetCycles(bool simplifyToSmallest = false)
        {
            if(Vertices.Count < 3)
            {
                return new List<Cycle>();
            }

            var result = new List<Cycle>();
            var used = new List<Vertex>();
            var parent = new Dictionary<Vertex, Vertex>();
            var todo = new Stack<Vertex>();

            todo.Push(Vertices[0]);
            parent.Add(Vertices[0], Vertices[0]);

            while(todo.Count > 0)
            {
                var cur = todo.Pop();
                used.Add(cur);

                foreach(var edge in cur.Edges)
                {
                    var neighbour = edge.GetOtherVertex(cur);

                    if (parent[cur] == neighbour)
                        continue;

                    if(used.Contains(neighbour))
                    {
                        var cycle = new Cycle(this, parent[cur]);

                        cycle.AddEdge(parent[cur].GetEdgeTo(cur));
                        cycle.AddEdge(cur.GetEdgeTo(neighbour));

                        var v = neighbour;
                        while (v != parent[cur])
                        {
                            var e = v.GetEdgeTo(parent[v]);
                            cycle.AddEdge(e);
                            v = parent[v];
                        }      

                        result.Add(cycle);
                    }
                    else if(!parent.ContainsKey(neighbour))
                    {
                        parent.Add(neighbour, cur);
                        todo.Push(neighbour);
                    }
                }
            }

            if(simplifyToSmallest && result.Count > 0)
            {
                return SmallestBase(result);
            }

            return result;
        }              

        private List<Cycle> SmallestBase(List<Cycle> initialBase)
        {            
            List<Cycle> smallestBase = new List<Cycle>(initialBase);

            var toRemove = new HashSet<Cycle>(); // indexes

            int baseCycleCount = initialBase.Count;
            for (int outer = 0; outer < baseCycleCount; outer++)
            {
                for (int inner = outer + 1; inner < baseCycleCount; inner++)
                {
                    var a = initialBase[inner];
                    var b = initialBase[outer];
                    var newCycle = a.MergeWith(b);
                    if (newCycle.EdgeCount != 0 && !smallestBase.Contains(newCycle))
                    {
                        Console.WriteLine($"New cycle found: {newCycle}");
                        if (newCycle.EdgeCount > a.EdgeCount || newCycle.EdgeCount > b.EdgeCount)
                        {
                            Console.WriteLine("Ignoring... (more edges than a and b)");
                            continue;
                        }

                        if (a.EdgeCount > b.EdgeCount)
                        {
                            Console.WriteLine($"Removing inner cycle: {a}");
                            toRemove.Add(a);
                        }
                        else
                        {
                            Console.WriteLine($"Removing outer cycle: {b}");
                            toRemove.Add(b);
                        }

                        smallestBase.Add(newCycle);
                    }
                }
            }

            smallestBase.RemoveAll(toRemove.Contains);

            return smallestBase;
        }
    }
}
