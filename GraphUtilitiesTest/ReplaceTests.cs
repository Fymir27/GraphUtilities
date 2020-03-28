using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphUtilities;

namespace GraphUtilitiesTest
{
    [TestClass]
    public class ReplaceTests
    {
        [TestMethod]
        public void Test1()
        {
            //   1
            //  /  \
            // 3 -- 2
            Graph graph = new Graph();
            var v1 = new Vertex();
            var v2 = new Vertex();
            var v3 = new Vertex();
            graph.AddVertex(v1);
            graph.AddVertex(v2);
            graph.AddVertex(v3);
            graph.AddEdge(new Edge(v1, v2));
            graph.AddEdge(new Edge(v2, v3));
            graph.AddEdge(new Edge(v3, v1));

            string initialGraphString = GraphPrinter.ToDot(graph);

            // 1 -- 2
            Graph pattern = new Graph();
            var vp1 = new Vertex();
            var vp2 = new Vertex();
 
            pattern.AddVertex(vp1);
            pattern.AddVertex(vp2);

            pattern.AddEdge(new Edge(vp1, vp2));

            // 1 -- 2 -- 3
            Graph replacement = new Graph();
            var vr1 = new Vertex();
            var vr2 = new Vertex();
            var vr3 = new Vertex();

            replacement.AddVertex(vr1);
            replacement.AddVertex(vr2);
            replacement.AddVertex(vr3);

            replacement.AddEdge(new Edge(vr1, vr2));
            replacement.AddEdge(new Edge(vr2, vr3));

            var mapping = new Dictionary<Vertex, Vertex>
            {
                { vp1, vr1 },
                { vp2, vr3 }
            };

            graph.Replace(pattern, replacement, mapping, true);

            Assert.IsTrue(graph.Vertices.Count == 4);
            Assert.IsTrue(graph.Vertices.TrueForAll(v => v.Edges.Count == 2));

            Assert.IsTrue(graph.Vertices.Contains(vr1));
            Assert.IsTrue(graph.Vertices.Contains(vr2));
            Assert.IsTrue(graph.Vertices.Contains(vr3));

            string afterReplaceString = GraphPrinter.ToDot(graph);

            File.WriteAllText("before.gv", initialGraphString);
            File.WriteAllText("after.gv", afterReplaceString);
        }
    }

}
