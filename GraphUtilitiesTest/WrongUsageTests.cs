using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GraphUtilities;

namespace GraphUtilitiesTest
{
    [TestClass]
    public class WrongUsageTests
    {     
        [TestMethod]
        public void TestCreateEdge()
        {
            var graph = new Graph<string, string>();

            var v1 = graph.AddVertex("a");
            var v2 = graph.AddVertex("b");

            var graph2 = new Graph<string, string>();

            var v1_2 = graph2.AddVertex("a");
            var v2_2 = graph2.AddVertex("b");

            // should fail if vertices are the same
            Assert.ThrowsException<ArgumentException>(() => graph.AddEdge(v1, v1, "Test"));

            // should fail if either vertex is null
            Assert.ThrowsException<ArgumentException>(() => graph.AddEdge(null, v1, "Test"));
            Assert.ThrowsException<ArgumentException>(() => graph.AddEdge(v1, null, "Test"));
            Assert.ThrowsException<ArgumentException>(() => graph.AddEdge(null, null, "Test"));

            // should fail if either vertex is not in graph
            Assert.ThrowsException<ArgumentException>(() => graph.AddEdge(v1, v1_2, "Test"));
            Assert.ThrowsException<ArgumentException>(() => graph.AddEdge(v1_2, v1, "Test"));
            Assert.ThrowsException<ArgumentException>(() => graph.AddEdge(v1_2, v2_2, "Test"));
        }       

        [TestMethod]
        public void TestRemoveVertex()
        {
            var graph = new Graph<int, int>();
            var v1 = graph.AddVertex(1);

            var graph2 = new Graph<int, int>();
            var v1_2 = graph2.AddVertex(1);

            // should fail if vertex is null or not in graph
            Assert.ThrowsException<ArgumentException>(() => graph.RemoveVertex(null));
            Assert.ThrowsException<ArgumentException>(() => graph.RemoveVertex(v1_2));
        }

        [TestMethod]
        public void TestRemovEdge()
        {
            var graph = new Graph<string, string>();

            var graph2 = new Graph<string, string>();

            var v1_2 = graph2.AddVertex("a");
            var v2_2 = graph2.AddVertex("b");
            var e_2 = graph2.AddEdge(v1_2, v2_2, "c");

            // should fail if edge is not in graph
            Assert.ThrowsException<ArgumentException>(() => graph.RemoveEdge(e_2));
        }

        [TestMethod]
        public void TestGetNeighbours()
        {
            var graph = new Graph<int, int>();
            var v1 = graph.AddVertex(1);

            var graph2 = new Graph<int, int>();
            var v1_2 = graph2.AddVertex(1);

            // should fail if vertex is null or not in graph
            Assert.ThrowsException<ArgumentException>(() => graph.GetNeighbours(null));
            Assert.ThrowsException<ArgumentException>(() => graph.GetNeighbours(v1_2));
        }

        [TestMethod]
        public void TestAdjacent()
        {
            var graph = new Graph<int, int>();
            var v1 = graph.AddVertex(1);

            var graph2 = new Graph<int, int>();
            var v1_2 = graph2.AddVertex(1);
            var v2_2 = graph2.AddVertex(2);

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
