using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GraphUtilities;

namespace GraphUtilitiesTest
{
    [TestClass]
    public class ReplacementRuleTests
    {
        [TestMethod]
        public void Control()
        {
            var builder = new ReplacementRuleBuilder();

            // invalid operations in "start" state
            Assert.ThrowsException<InvalidOperationException>(() => builder.GetResult());
            Assert.ThrowsException<InvalidOperationException>(() => builder.Continue());
            Assert.ThrowsException<InvalidOperationException>(() => builder.Reset());

            builder.MappedVertex<Vertex>("test");
       
            builder.MappedEdge<Edge>();
            // can't get result if edge is unfinished
            Assert.ThrowsException<InvalidOperationException>(() => builder.GetResult());

            builder.MappedVertex<Vertex>();
            builder.PatternEdge<Edge>();
            // can't get result if edge is unfinished
            Assert.ThrowsException<InvalidOperationException>(() => builder.GetResult());

            // test continuing from end state 
            builder.PatternVertex<Vertex>().GetResult();
            builder.Continue("test");

            builder.ReplacementEdge<Edge>();
            // can't get result if edge is unfinished
            Assert.ThrowsException<InvalidOperationException>(() => builder.GetResult());

            builder.ReplacementVertex<Vertex>();

            // valid control:
            var rule = builder.GetResult();
            builder.Continue();
            builder.MoveToTag("test");
            builder.GetResult();
            builder.Reset();

            Assert.IsTrue(rule.Pattern.Vertices.Count == 3);
            Assert.IsTrue(rule.Replacement.Vertices.Count == 3);
            Assert.IsTrue(rule.Mapping.Count == 2);
        }

        [TestMethod]
        public void NonExitentTag()
        {
            var builder = new ReplacementRuleBuilder();

            // don't move to nonexistent tag
            Assert.ThrowsException<ArgumentException>(() => builder.MoveToTag("test"));
        }

        [TestMethod]
        public void EdgeStart()
        {
            var builder = new ReplacementRuleBuilder();

            // don't start with edge
            Assert.ThrowsException<InvalidOperationException>(() => builder.PatternEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.ReplacementEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.MappedEdge<Edge>());
        }

        [TestMethod]
        public void InvalidPatternVertexContinuation()
        {
            var builder = new ReplacementRuleBuilder();

            // check impossible next states after adding pattern vertex
            builder.PatternVertex<Vertex>();
            Assert.ThrowsException<InvalidOperationException>(() => builder.ReplacementEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.MappedEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.PatternVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.ReplacementVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.MappedVertex<Vertex>());
        }

        [TestMethod]
        public void InvalidReplacementVertexContinuation()
        {
            var builder = new ReplacementRuleBuilder();

            // check impossible next states after adding replacment vertex
            builder.ReplacementVertex<Vertex>();
            Assert.ThrowsException<InvalidOperationException>(() => builder.PatternEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.MappedEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.PatternVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.ReplacementVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.MappedVertex<Vertex>());
        }

        [TestMethod]
        public void InvalidMappedVertexContinuation()
        {
            var builder = new ReplacementRuleBuilder();

            // check impossible next states after adding mapped vertex
            builder.MappedVertex<Vertex>();
            Assert.ThrowsException<InvalidOperationException>(() => builder.PatternVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.ReplacementVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.MappedVertex<Vertex>());
        }

        [TestMethod]
        public void InvalidPatternEdgeContinuation()
        {
            var builder = new ReplacementRuleBuilder();

            // check impossible next states after adding pattern edge
            builder.PatternVertex<Vertex>().PatternEdge<Edge>();
            Assert.ThrowsException<InvalidOperationException>(() => builder.ReplacementVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.MappedVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.PatternEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.ReplacementEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.MappedEdge<Edge>());
        }

        [TestMethod]
        public void InvalidReplacementEdgeContinuation()
        {
            var builder = new ReplacementRuleBuilder();

            // check impossible next states after adding replacement edge
            builder.ReplacementVertex<Vertex>().ReplacementEdge<Edge>();
            Assert.ThrowsException<InvalidOperationException>(() => builder.PatternVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.MappedVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.PatternEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.ReplacementEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.MappedEdge<Edge>());
        }

        [TestMethod]
        public void InvalidMappedEdgeContinuation()
        {
            var builder = new ReplacementRuleBuilder();

            // check impossible next states after adding mapped edge
            builder.MappedVertex<Vertex>().MappedEdge<Edge>();
            Assert.ThrowsException<InvalidOperationException>(() => builder.ReplacementVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.PatternVertex<Vertex>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.PatternEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.ReplacementEdge<Edge>());
            Assert.ThrowsException<InvalidOperationException>(() => builder.MappedEdge<Edge>());
        }
    }
}
