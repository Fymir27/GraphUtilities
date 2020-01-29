using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphUtilities;
using System.IO;

namespace GraphUtilitiesTest
{
    [TestClass]
    public class PrintTests
    {
        [TestMethod]
        public void Print1()
        {
            var graph = new Graph();

            var v1 = new IntVertex(1);
            graph.AddVertex(v1);
            var v2 = new IntVertex(2);
            graph.AddVertex(v2);
            var v3 = new IntVertex(3);
            graph.AddVertex(v3);
            var v4 = new IntVertex(2);
            graph.AddVertex(v4);
            var v5 = new IntVertex(3);
            graph.AddVertex(v5);
            var v6 = new IntVertex(2);
            graph.AddVertex(v6);

            var e1 = new Edge(v1, v2);
            graph.AddEdge(e1);
            var e2 = new Edge(v2, v3);
            graph.AddEdge(e2);
            var e3 = new Edge(v1, v4);
            graph.AddEdge(e3);
            var e4 = new Edge(v2, v5);
            graph.AddEdge(e4);
            var e5 = new Edge(v3, v6);
            graph.AddEdge(e5);
            var e6 = new Edge(v4, v5);
            graph.AddEdge(e6);
            var e7 = new Edge(v5, v6);
            graph.AddEdge(e7);

            string graphString = GraphPrinter.ToDot(graph);

            File.WriteAllText("print1.gv", graphString);
        }


        [TestMethod]
        public void Print2()
        {
            var graph = new Graph();

            var v1 = new StringVertex("blue");
            graph.AddVertex(v1);
            var v2 = new StringVertex("green");
            graph.AddVertex(v2);
            var v3 = new StringVertex("yellow");
            graph.AddVertex(v3);
            var v4 = new StringVertex("yellow");
            graph.AddVertex(v4);
            var v5 = new StringVertex("blue");
            graph.AddVertex(v5);
            var v6 = new StringVertex("green");
            graph.AddVertex(v6);

            graph.AddEdge(new StringEdge(v1, v4, "green"));
            graph.AddEdge(new StringEdge(v2, v5, "yellow"));
            graph.AddEdge(new StringEdge(v3, v6, "blue"));
            graph.AddEdge(new StringEdge(v4, v5, "green"));
            graph.AddEdge(new StringEdge(v5, v6, "yellow"));
            graph.AddEdge(new StringEdge(v6, v4, "blue"));

            string graphString = GraphPrinter.ToDot(graph, true, true);

            File.WriteAllText("print2.gv", graphString);
        }
    }
}
