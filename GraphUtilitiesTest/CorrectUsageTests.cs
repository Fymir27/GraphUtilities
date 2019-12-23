using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GraphUtilities;

namespace GraphUtilitiesTest
{
    [TestClass]
    public class CorrectUsageTests
    {
        [TestMethod]
        public void TestCreateEmptyGraph()
        {
            var graph = new Graph<int, int>();
            Assert.IsNotNull(graph);
            Assert.IsNotNull(graph.Vertices);
            Assert.IsTrue(graph.Vertices.Count == 0);
        }

        [TestMethod]
        public void TestCreateVertex()
        {
            var graph = new Graph<int, int>();

            int vertexData = 42;
            var v1 = graph.AddVertex(vertexData);
            Assert.IsNotNull(v1);
            Assert.IsTrue(v1.Data == vertexData);
            Assert.IsNotNull(v1.Edges);
            Assert.IsTrue(v1.Edges.Count == 0);
            Assert.IsTrue(graph.Vertices.Count == 1);
        }

        [TestMethod]
        public void TestCreateEdge()
        {
            var graph = new Graph<int, int>();

            var v1 = graph.AddVertex(0);
            var v2 = graph.AddVertex(1);

            int edgeData = 69;
            var e = graph.AddEdge(v1, v2, edgeData);
            Assert.IsNotNull(e);
            Assert.IsTrue(e.Data == edgeData);
            Assert.IsTrue(v1.Edges.Contains(e));
            Assert.IsTrue(v2.Edges.Contains(e));
        }

        [TestMethod]
        public void TestRemoveVertex()
        {
            // (2)---(1)---(3)
            var graph = new Graph<int, int>();
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);
            var v3 = graph.AddVertex(3);
            var e1 = graph.AddEdge(v1, v2, 1);
            var e2 = graph.AddEdge(v1, v3, 2);

            graph.RemoveVertex(v1);
            Assert.IsFalse(graph.Vertices.Contains(v1));
            Assert.IsTrue(v1.Edges.Count == 0);
            Assert.IsFalse(v2.Edges.Contains(e1));
            Assert.IsFalse(v3.Edges.Contains(e2));

            // Assert.ThrowsException<ArgumentException>(() => graph.RemoveVertex(v1));
        }

        [TestMethod]
        public void TestRemovEdge()
        {
            // (2)---(1)
            var graph = new Graph<int, int>();
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);
            var e1 = graph.AddEdge(v1, v2, 1);

            graph.RemoveEdge(e1);
            Assert.IsFalse(v1.Edges.Contains(e1));
            Assert.IsFalse(v2.Edges.Contains(e1));
        }

        [TestMethod]
        public void TestGetNeighbours()
        {
            // (2)---(1)---(3)
            var graph = new Graph<int, int>();
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);
            var v3 = graph.AddVertex(3);
            var e1 = graph.AddEdge(v1, v2, 1);
            var e2 = graph.AddEdge(v1, v3, 2);

            var neighbours = graph.GetNeighbours(v1);
            Assert.IsNotNull(neighbours);
            Assert.IsTrue(neighbours.Count == 2);
            Assert.IsTrue(neighbours.Contains(v2));
            Assert.IsTrue(neighbours.Contains(v3));
        }

        [TestMethod]
        public void TestAdjacent()
        {
            // (2)---(1)---(3)
            var graph = new Graph<int, int>();
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);
            var v3 = graph.AddVertex(3);
            var e1 = graph.AddEdge(v1, v2, 1);
            var e2 = graph.AddEdge(v1, v3, 2);

            Assert.IsTrue(graph.Adjacent(v1, v2));
            Assert.IsTrue(graph.Adjacent(v2, v1));
            Assert.IsFalse(graph.Adjacent(v2, v3));
        }
    }
}
