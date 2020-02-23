using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GraphUtilities;

namespace GraphUtilitiesTest
{
    [TestClass]
    public class WrongUsageTests
    {
        [TestMethod]
        public void TestCreateVertex()
        {
            var graph = new Graph();
            Assert.ThrowsException<System.ArgumentException>(() => graph.AddVertex<Vertex>(null));
        }

        [TestMethod]
        public void TestCreateEdge()
        {
            var graph = new Graph();

            var v1 = new StringVertex("a");
            graph.AddVertex(v1);
            var v2 = new StringVertex("b");
            graph.AddVertex(v2);            

            // should fail if vertices are the same
            Assert.ThrowsException<ArgumentException>(() => new Edge(v1, v1));

            // should fail if either vertex is null
            Assert.ThrowsException<ArgumentException>(() => new Edge(null, v1));
            Assert.ThrowsException<ArgumentException>(() => new Edge(v1, null));
            Assert.ThrowsException<ArgumentException>(() => new Edge(null, null));

            // should fail if either vertex is not in graph
            var v1_2 = new StringVertex("a");
            var v2_2 = new StringVertex("b");
            var e1 = new Edge(v1, v1_2);
            var e2 = new Edge(v1_2, v1);
            var e3 = new Edge(v1_2, v2_2);
            Assert.ThrowsException<ArgumentException>(() => graph.AddEdge(e1));
            Assert.ThrowsException<ArgumentException>(() => graph.AddEdge(e2));
            Assert.ThrowsException<ArgumentException>(() => graph.AddEdge(e3));
        }

        [TestMethod]
        public void TestRemoveVertex()
        {
            var graph = new Graph();
            var v1 = new IntVertex(1);
            graph.AddVertex(v1);

            var v1_2 = new IntVertex(1);

            // should fail if vertex is null or not in graph
            Assert.ThrowsException<ArgumentException>(() => graph.RemoveVertex(null));
            Assert.ThrowsException<ArgumentException>(() => graph.RemoveVertex(v1_2));
        }

        [TestMethod]
        public void TestRemovEdge()
        {
            var graph = new Graph();

            var v1 = new StringVertex("a");
            var v2 = new StringVertex("b");
            var e = new StringEdge(v1, v2, "c");

            // should fail if edge is not in graph
            Assert.ThrowsException<ArgumentException>(() => graph.RemoveEdge(e));
        }

        [TestMethod]
        public void TestGetNeighbours()
        {
            var graph = new Graph();

            var v = new IntVertex(1);

            // should fail if vertex is null or not in graph
            Assert.ThrowsException<ArgumentException>(() => graph.GetNeighbours(null));
            Assert.ThrowsException<ArgumentException>(() => graph.GetNeighbours(v));
        }

        [TestMethod]
        public void TestAdjacent()
        {
            var graph = new Graph();
            var v1 = new IntVertex(1);
            graph.AddVertex(v1);

            var v1_2 = new IntVertex(1);
            var v2_2 = new IntVertex(2);

            // should fail if either vertex is null
            Assert.ThrowsException<ArgumentException>(() => graph.Adjacent(v1, null));
            Assert.ThrowsException<ArgumentException>(() => graph.Adjacent(null, v1));
            Assert.ThrowsException<ArgumentException>(() => graph.Adjacent(null, null));

            // should fail if either vertex is not in graph
            Assert.ThrowsException<ArgumentException>(() => graph.Adjacent(v1, v1_2));
            Assert.ThrowsException<ArgumentException>(() => graph.Adjacent(v1_2, v1));
            Assert.ThrowsException<ArgumentException>(() => graph.Adjacent(v1_2, v2_2));
        }
    }
}
