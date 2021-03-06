﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GraphUtilities;
using System.Collections.Generic;

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
            Assert.IsTrue(cycles[0].EdgeCount == 3);
            Assert.IsTrue(cycles[1].EdgeCount == 3);

            List<List<Edge>> overlap = cycles[0].Overlap(cycles[1]);

            Assert.IsTrue(overlap.Count == 1);
            Assert.IsTrue(overlap[0].Count == 1);
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
            Assert.IsTrue(cycles[0].EdgeCount == 3);
            Assert.IsTrue(cycles[1].EdgeCount == 3);
            Assert.IsTrue(cycles[2].EdgeCount == 3);

            List<List<Edge>> overlap1 = cycles[0].Overlap(cycles[1]);
            List<List<Edge>> overlap2 = cycles[1].Overlap(cycles[2]);
            List<List<Edge>> overlap3 = cycles[0].Overlap(cycles[2]);

            Assert.IsTrue(overlap1.Count == 1);
            Assert.IsTrue(overlap2.Count == 1);
            Assert.IsTrue(overlap3.Count == 1);            
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
            Assert.IsTrue(cycles[0].EdgeCount == 4);
            Assert.IsTrue(cycles[1].EdgeCount == 4);

            List<List<Edge>> overlap = cycles[0].Overlap(cycles[1]);

            Assert.IsTrue(overlap.Count == 1);
            Assert.IsTrue(overlap[0].Count == 1);
        }
    }
}
