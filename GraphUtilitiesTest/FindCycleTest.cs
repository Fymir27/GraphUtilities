using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GraphUtilities;

namespace GraphUtilitiesTest
{
    [TestClass]
    public class FindCycleTest  
    {
        [TestMethod]
        public void Test1()
        {
            var graph = new Graph();

            var a = new StringVertex("A");
            var b = new StringVertex("B");
            var c = new StringVertex("C");
            var d = new StringVertex("D");

            graph.AddVertex(a);            
            graph.AddVertex(b);            
            graph.AddVertex(c);            
            graph.AddVertex(d);

            graph.AddEdge(new Edge(a, b));
            //graph.AddEdge(new Edge(a, c));
            graph.AddEdge(new Edge(a, d));
            graph.AddEdge(new Edge(b, c));
            graph.AddEdge(new Edge(b, d));
            graph.AddEdge(new Edge(c, d));

            Console.WriteLine(GraphPrinter.ToDot(graph));

            var cycles = graph.GetCycles(true);

            Assert.IsTrue(cycles.Count == 2);
        }

        [TestMethod]
        public void Test2()
        {
            var graph = new Graph();

            var a = new StringVertex("A");
            var b = new StringVertex("B");
            var c = new StringVertex("C");
            var d = new StringVertex("D");

            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);
            graph.AddVertex(d);

            graph.AddEdge(new Edge(a, b));
            graph.AddEdge(new Edge(a, c));
            graph.AddEdge(new Edge(a, d));
            graph.AddEdge(new Edge(b, c));
            graph.AddEdge(new Edge(b, d));
            graph.AddEdge(new Edge(c, d));

            Console.WriteLine(GraphPrinter.ToDot(graph));

            var cycles = graph.GetCycles(true);

            Assert.IsTrue(cycles.Count == 3);
        }

        [TestMethod]
        public void Test3()
        {
            var graph = new Graph();

            var a = new StringVertex("A");
            var b = new StringVertex("B");
            var c = new StringVertex("C");
            var d = new StringVertex("D");
            var e = new StringVertex("E");
            var f = new StringVertex("F");

            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);
            graph.AddVertex(d);
            graph.AddVertex(e);
            graph.AddVertex(f);

            graph.AddEdge(new Edge(a, b));
            graph.AddEdge(new Edge(a, d));            
            graph.AddEdge(new Edge(b, c));
            graph.AddEdge(new Edge(b, e));
            graph.AddEdge(new Edge(c, f));
            graph.AddEdge(new Edge(d, e));
            graph.AddEdge(new Edge(e, f));

            Console.WriteLine(GraphPrinter.ToDot(graph));

            var cycles = graph.GetCycles(true);

            Assert.IsTrue(cycles.Count == 2);
        }

    }
}
