using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GraphUtilities;

namespace GraphUtilitiesTest
{
    [TestClass]
    public class PatternMatchTests
    {
        [TestMethod]
        public void Test1()
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

            var pattern = new Graph();
            var pv1 = new IntVertex(1);
            pattern.AddVertex(pv1);

            MatchResult result = graph.FindPattern(pattern);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Vertices.Count == pattern.Vertices.Count);
            Assert.IsTrue(result.Vertices[pv1] == v1);

            var pv3 = new IntVertex(3);
            pattern.AddVertex(pv3);
            var e = new Edge(pv1, pv3);
            pattern.AddEdge(e);

            Assert.IsNull(graph.FindPattern(pattern));
            pattern.RemoveEdge(e);

            var pv2 = new IntVertex(2);
            pattern.AddVertex(pv2);
            var pv4 = new IntVertex(2);
            pattern.AddVertex(pv4);
            var ep1 = new Edge(pv1, pv2);
            pattern.AddEdge(ep1);
            var ep2 = new Edge(pv2, pv3);
            pattern.AddEdge(ep2);
            var ep3 = new Edge(pv3, pv4);
            pattern.AddEdge(ep3);
            var ep4 = new Edge(pv1, pv4);
            pattern.AddEdge(ep4);

            result = graph.FindPattern(pattern);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Vertices.Count == pattern.Vertices.Count);
            Assert.IsTrue(result.Edges.Count == 4);

            var pv5 = new IntVertex(3);
            pattern.AddVertex(pv5);
            var ep5 = new Edge(pv2, pv5);
            pattern.AddEdge(ep5);

            result = graph.FindPattern(pattern);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Vertices.Count == pattern.Vertices.Count);
            Assert.IsTrue(result.Edges.Count == 5);

            // see if vertices/edges are there
            Assert.IsTrue(result.Vertices.ContainsKey(pv1));
            Assert.IsTrue(result.Vertices.ContainsKey(pv3));
            Assert.IsTrue(result.Vertices[pv1] == v1);
            Assert.IsTrue(result.Vertices[pv3] == v5);

            Assert.IsTrue(result.Edges.ContainsKey(ep2));
            Assert.IsTrue(result.Edges.ContainsKey(ep4));


            var pv6 = new IntVertex(2);
            pattern.AddVertex(pv6);
            var ep6 = new Edge(pv2, pv6);
            pattern.AddEdge(ep6);

            Assert.IsNull(graph.FindPattern(pattern));
        }

        [TestMethod]
        public void Test2()
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

            var pattern = new Graph();

            var vp1 = new IntVertex(2);
            pattern.AddVertex(vp1);
            var vp2 = new IntVertex(3);
            pattern.AddVertex(vp2);
            var vp3 = new IntVertex(2);
            pattern.AddVertex(vp3);
            var vp4 = new IntVertex(1);
            pattern.AddVertex(vp4);

            var ep1 = new Edge(vp1, vp2);
            pattern.AddEdge(ep1);
            var ep2 = new Edge(vp2, vp3);
            pattern.AddEdge(ep2);
            var ep3 = new Edge(vp3, vp4);
            pattern.AddEdge(ep3);

            Assert.IsNotNull(graph.FindPattern(pattern));

            var ef = new Edge(vp1, vp3);
            pattern.AddEdge(ef);

            Assert.IsNull(graph.FindPattern(pattern));
        }

        [TestMethod]
        public void Test3()
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

            var pattern = new Graph();

            var vp1 = new StringVertex("yellow");
            pattern.AddVertex(vp1);
            var vp2 = new StringVertex("blue");
            pattern.AddVertex(vp2);
            var vp3 = new StringVertex("green");
            pattern.AddVertex(vp3);

            pattern.AddEdge(new StringEdge(vp1, vp2, "green"));
            pattern.AddEdge(new StringEdge(vp2, vp3, "yellow"));
            pattern.AddEdge(new StringEdge(vp3, vp1, "blue"));

            Assert.IsNotNull(graph.FindPattern(pattern));

            var vp4 = new StringVertex("green");
            pattern.AddVertex(vp4);
            // wrong edge color
            var ef = new StringEdge(vp2, vp4, "blue");
            pattern.AddEdge(ef);

            Assert.IsNull(graph.FindPattern(pattern));
            pattern.RemoveEdge(ef);

            Assert.ThrowsException<System.ArgumentException>(() => pattern.AddVertex(vp4));
            var ef2 = new DataEdge<float>(vp2, vp4, 4.2f);
            pattern.AddEdge(ef2);
            Assert.IsNull(graph.FindPattern(pattern));
        }

        [TestMethod]
        public void Test4()
        {
            var graph = new Graph();

            var v1 = new Vertex();
            graph.AddVertex(v1);
            var v2 = new Vertex();
            graph.AddVertex(v2);
            var v3 = new Vertex();
            graph.AddVertex(v3);

            var e1 = new Edge(v1, v2);
            graph.AddEdge(e1);
            var e2 = new Edge(v2, v3);
            graph.AddEdge(e2);
            var e3 = new Edge(v1, v3);
          

            var pattern = new Graph();
            var pv1 = new Vertex();
            var pv2 = new Vertex();
            var pv3 = new Vertex();
            var pv4 = new Vertex();
            pattern.AddVertex(pv1);
            pattern.AddVertex(pv2);
            pattern.AddVertex(pv3);
            pattern.AddVertex(pv4);

            pattern.AddEdge(new Edge(pv1, pv2));
            pattern.AddEdge(new Edge(pv2, pv3));
            pattern.AddEdge(new Edge(pv3, pv4));

            MatchResult result = graph.FindPattern(pattern);
            Assert.IsNull(result);
        }
    }
}
