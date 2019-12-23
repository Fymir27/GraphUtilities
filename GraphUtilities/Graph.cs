using System.Collections.Generic;

namespace GraphUtilities
{
    /// <summary>
    /// Represents an UNDIRECTED mathematical graph consisting of vertices and edges
    /// Vertices and edges are saved as objects and can each contain arbitrary data
    /// Allows having vertices that are not connected to anything
    /// Does not yet support vertices having an edge to themselves (no loops!)
    /// </summary>
    /// <typeparam name="TVertexData">Type of vertex data</typeparam>
    /// <typeparam name="TEdgeData">Type of edge data</typeparam>
    public class Graph<TVertexData, TEdgeData>
    {
        // --- Nested classes --- //

        /// <summary>
        /// Represents a vertex in the graph
        /// Keeps a list of incident edges which are NOT freely modifyable
        /// Data of type TVertexData is freely accessible/modifyable
        /// An instance of this can only be created by calling Graph.AddVertex(TVertexData)
        /// </summary>
        public class Vertex
        {
            public TVertexData Data { get; set; }
            public List<Edge> Edges { get; internal set; } = new List<Edge>();

            internal Vertex() { }
        }

        /// <summary>
        /// Represents an edge in the graph
        /// Keeps track of start/end vertices which are NOT freely modifyable
        /// Data of type TEdgeData is freely accessible/modifyable
        /// An instance of this can only be created by calling Graph.AddEdge(Vertex, Vertex, TEdgeData)
        /// </summary>
        public class Edge
        {
            public TEdgeData Data { get; set; }

            public Vertex V1 { get; internal set; }
            public Vertex V2 { get; internal set; }

            public Vertex GetOtherVertex(Vertex vertex)
            {
                if (vertex == V1)
                    return V2;
                if (vertex == V2)
                    return V1;
                throw new System.ArgumentException("Vertex is not adjacent to this edge!");
            }

            internal Edge() { }
        }


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
        public Vertex AddVertex(TVertexData data)
        {
            var vertex = new Vertex() { Data = data };
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
        public Edge AddEdge(Vertex first, Vertex second, TEdgeData data)
        {
            AssertVertex(first);
            AssertVertex(second);

            // TODO: allow loops?
            if(first == second)
            {
                throw new System.ArgumentException("No edges with same start and end vertex allowed!");
            }

            var edge = new Edge()
            {
                V1 = first,
                V2 = second,
                Data = data
            };

            first.Edges.Add(edge);
            second.Edges.Add(edge);
            return edge;
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

        private void AssertVertex(Vertex vertex)
        {
            if(vertex == null)
            {
                throw new System.ArgumentException("Vertex is null!");
            }

            if(!Vertices.Contains(vertex))
            {
                throw new System.ArgumentException("Vertex doesn't exist in this graph!");
            }           
        }

        private void AssertEdge(Edge edge)
        {
            AssertVertex(edge.V1);
            AssertVertex(edge.V2);

            // should never be possible
            //if(!(edge.V1.Edges.Contains(edge) && edge.V2.Edges.Contains(edge)))
            //{
            //    throw new System.ArgumentException("Edge Vertices don't contain edge!");
            //}
        }
    }
}
