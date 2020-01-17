using System.Collections.Generic;
using System.Linq;
using System;

namespace GraphUtilities
{
    /// <summary>
    /// Gives out unique IDs by using a unique counter for every class
    /// </summary>
    /// <typeparam name="T">Class for which to give IDs</typeparam>
    public abstract class Unique<T>
    {
        public int ID { get; }

        private static int counter = 0;

        protected Unique() { ID = counter++; }
    }

    /// <summary>
    /// Represents a vertex in the graph
    /// Keeps a list of incident edges which are NOT freely modifiable
    /// Data of type TVertexData is freely accessible/modifiable
    /// An instance of this can only be created by calling Graph.AddVertex(TVertexData)
    /// </summary>
    public class Vertex : Unique<Vertex>
    {
        public List<Edge> Edges { get; internal set; }

        public Vertex()
        {
            Edges = new List<Edge>();
        }

        public override string ToString()
        {
            return $"Vertex {ID}: {Graph.EnumerableToString(Edges)}";
        }

        public virtual bool SameType(Vertex other)
        {
            return GetType() == other.GetType();
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
    }

    /// <summary>
    /// Represents an edge in the graph
    /// Keeps track of start/end vertices which are NOT freely modifiable
    /// </summary>
    public class Edge : Unique<Edge>
    {
        public Vertex V1 { get; internal set; }
        public Vertex V2 { get; internal set; }

        public Edge(Vertex first, Vertex second)
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

        public Vertex GetOtherVertex(Vertex vertex)
        {
            if (vertex == V1)
                return V2;
            if (vertex == V2)
                return V1;
            throw new System.ArgumentException("Vertex is not adjacent to this edge!");
        }

        public override string ToString()
        {
            return $"{ID}: {V1.ID}--{V2.ID}";
        }

        public virtual bool SameType(Edge other)
        {
            return GetType() == other.GetType();
        }
    }

    /// <summary>
    /// Edge that also contains data
    /// </summary>
    /// <typeparam name="TData">Type of data</typeparam>
    public class DataEdge<TData> : Edge
    {
        public TData Data { get; set; }

        public DataEdge(Vertex first, Vertex second, TData data) : base(first, second)
        {
            Data = data;
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
        // --- Properties ----//

        /// <summary>
        /// List of vertices in the graph
        /// </summary>
        public List<Vertex> Vertices { get; private set; } = new List<Vertex>();

        // --- Methods ---///

        /// <summary>
        /// Adds a vertex to the graph
        /// </summary>
        /// <param name="data">Data saved in vertex</param>
        /// <returns>Reference to the created vertex</returns>
        public void AddVertex(Vertex vertex)
        {
            if (vertex == null)
            {
                throw new System.ArgumentException("Can't add a vertex that is null!");
            }
            Vertices.Add(vertex);
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

        public Vertex FindVertexLike(Vertex queryVertex)
        {
            AssertVertex(queryVertex);
            return Vertices.Find(v => queryVertex.SameType(v));
        }

        public IEnumerable<Vertex> FindVerticesLike(Vertex queryVertex)
        {
            System.Type type = queryVertex.GetType();
            return Vertices.Where(v => queryVertex.SameType(v));
        }

        public MatchResult FindPattern(Graph pattern, bool verbose = false)
        {
            if (pattern == null || pattern.Vertices.Count == 0)
            {
                throw new System.ArgumentException("Invalid pattern!");
            }

            var patternVertex = pattern.Vertices[0]; // TODO: make it random
            return MatchRecursively(patternVertex, verbose);

            //foreach (var startVertex in FindVerticesLike(patternVertex))
            // {
            //    MatchResult result = new MatchResult();
            //    bool success = IterateVertex(patternVertex, startVertex, new List<Edge>(), new List<Edge>(), result, verbose);
            //    if(success == true)
            //    {
            //        //result.Vertices.Add(patternVertex, startVertex);
            //        return result;
            //    }
            //}
            //return null;
        }

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

        public void AssertEdge(Edge edge)
        {
            AssertVertex(edge.V1);
            AssertVertex(edge.V2);
        }

        private bool InGraph(Vertex vertex)
        {
            return Vertices.Contains(vertex);
        }

        class Match<T>
        {
            public T Pattern;
            public T Matched;

            public Match(T pattern, T matched)
            {
                Pattern = pattern;
                Matched = matched;
            }
        }

        class SearchState
        {
            public Match<Vertex> Vertex;
            public SearchState Prev;
            public List<Edge> FailedEdges;
        }

        /*private MatchResult MatchDepthFirst(Vertex startVertex)
        {
            Stack<Vertex> possibleFirstMatches = new Stack<Vertex>(FindVerticesLike(startVertex));

            foreach (var firstMatch in possibleFirstMatches)
            {
                // list of pattern vertices match
                Stack<SearchState> todo = new Stack<SearchState>();

                List<Edge> visitedPatternEdges = new List<Edge>();
                List<Edge> visitedHostEdges = new List<Edge>();

                todo.Push(new SearchState()
                {
                    Vertex = new Match<Vertex>(startVertex, firstMatch)
                });

                bool failed = false;

                while (todo.Count > 0 && !failed)
                {
                    var currentState = todo.Pop();

                    var patternVertex = currentState.Vertex.Pattern;
                    var matchVertex = currentState.Vertex.Matched;

                    var unvisitedPatternEdges = patternVertex.Edges
                        .Where(e => !visitedPatternEdges.Contains(e));

                    foreach (var outgoingPatternEdge in unvisitedPatternEdges)
                    {
                        var nextPatternVertex = outgoingPatternEdge.GetOtherVertex(patternVertex);

                        var unvisitedEdges = matchVertex.Edges
                            .Where(e => !visitedHostEdges.Contains(e) &&
                                !currentState.FailedEdges.Contains(e) &&
                                outgoingPatternEdge.SameType(e) &&
                                nextPatternVertex.SameType(e.GetOtherVertex(matchVertex)));

                        if (unvisitedEdges.Count() == 0)
                        {
                            currentState->FailedEdges->Add()
                        }

                        visitedPatternEdges.Push(outgoingPatternEdge);

                        // TODO: remove loop
                        foreach (var nextEdge in unvisitedEdges)
                        {
                            var nextVertex = nextEdge.GetOtherVertex(matchVertex);
                            visitedHostEdges.Push(nextEdge);
                            todo.Enqueue(Tuple.Create(nextPatternVertex, nextVertex));
                            result.Edges.Add(outgoingPatternEdge, nextEdge);
                        }
                    }
                }
                if (!failed) // means todo is empty
                {
                    return result;
                }
            }
            // if we reach this, that means we have failed to find a match
            // because we wouldve already returned the result
            return null;
        }*/

        private MatchResult MatchRecursively(Vertex startVertex, bool verbose)
        {
            Stack<Vertex> possibleFirstMatches = new Stack<Vertex>(FindVerticesLike(startVertex));

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
                //return null; // TODO: remove
            }

            return null;

            bool IterateVertex(Vertex patternVertex, Vertex matchVertex)
            {
                if (verbose)
                {
                    System.Console.WriteLine($"Pattern: {EnumerableToString(visitedPatternEdges)} -> {patternVertex.ID}");
                    System.Console.WriteLine($"Matched: {EnumerableToString(matchedEdges)} -> {matchVertex.ID}\n---");
                }

                var unvisitedPatternEdges = patternVertex.Edges
                    .Where(e => !visitedPatternEdges.Contains(e)).ToArray();

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
                        ).ToArray();

                    if (unvisitedEdges.Count() == 0)
                    {
                        visitedPatternEdges.Remove(patternEdge);
                        return false;
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
                    if(!success)
                    {
                        visitedPatternEdges.Remove(patternEdge);
                        return false;
                    }
                }

                if (!result.Vertices.ContainsKey(patternVertex))
                {
                    result.Vertices.Add(patternVertex, matchVertex);
                }
                else if (result.Vertices[patternVertex] != matchVertex)
                {
                    throw new Exception("Vertex matched again, but differently!");
                }

                return true;
            }
        }

        public static string EnumerableToString(System.Collections.IEnumerable enumerable)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder("[");
            foreach (var elem in enumerable)
            {
                result.Append(elem.ToString()).Append(",");
            }
            return result.Replace(',', ']', result.Length - 1, 1).ToString();
        }
    }
}
