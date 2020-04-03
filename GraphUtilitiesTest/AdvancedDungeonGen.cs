using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GraphUtilities;
using System.IO;

namespace GraphUtilitiesTest
{
    class Room : Vertex
    {
        public override string ToString()
        {
            return "Room";
        }
    }

    class Fragment : Vertex
    {
        public override string ToString()
        {
            return "Fragment";
        }
    }

    class Junction : Fragment
    {
        public override string ToString()
        {
            return "Junc.";
        }
    }

    class StartingRoom : Room
    {
        public override string ToString()
        {
            return "Start";
        }
    }

    class BasicRoom : Room { }

    class FinalRoom : Room { }

    [TestClass]
    public class AdvancedDungeonGen
    {
        Random rnd = new Random((int)System.DateTime.Now.Ticks);

        [TestMethod]
        public void Start()
        {
            var builder = new ReplacementRuleBuilder();

            builder.MappedVertex<StartingRoom>("start")
                .ReplacementVertexWithEdge<BasicRoom, Edge>().MoveToTag("start")
                .ReplacementVertexWithEdge<BasicRoom, Edge>().MoveToTag("start")
                .ReplacementVertexWithEdge<BasicRoom, Edge>();

            var initialRule = builder.GetResult();

            var dungeon = new Graph();
            dungeon.AddVertex(new StartingRoom());
            dungeon.Replace(initialRule, true);

            for (int i = 0; i < 15; i++)
            {
                var b = new BasicRoom();
                var b_ = new BasicRoom();

                builder.Reset()
                    .MappedVertex<BasicRoom>("a")
                    .PatternVertexWithEdge(b, new Edge())
                    .MoveToTag("a").ReplacementVertexWithEdge<Junction, Edge>("j")
                    .ReplacementVertexWithEdge<BasicRoom, Edge>().MoveToTag("j")
                    .ReplacementVertexWithEdge(b_, new Edge());

                var addJunction = builder.GetResult();
                addJunction.Mapping.Add(b, b_);

                builder.Reset()
                    .MappedVertex<BasicRoom>()
                    .ReplacementVertexWithEdge<BasicRoom, Edge>();

                var stretch = builder.GetResult();

                ReplacementRule[] rules = { addJunction, stretch };
                int ruleIndex = rnd.Next(rules.Length);
                dungeon.Replace(rules[ruleIndex], true);
            }

            File.WriteAllText("advancedDungeon.gv", GraphPrinter.ToDot(dungeon));
        }
    }
}
