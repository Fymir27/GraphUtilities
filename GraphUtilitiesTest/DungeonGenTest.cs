using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphUtilities;

namespace GraphUtilitiesTest
{
    [TestClass]
    public class DungeonGenTest
    {
        class RBasic : DataVertex<string> { public RBasic() : base("Basic") { } }
        class REssential : DataVertex<string> { public REssential() : base("Ess.") { }  }
        class RTreasure : DataVertex<string> { public RTreasure() : base("Treas.") { } }
        class RBoss : DataVertex<string> { public RBoss() : base("Boss") { } }

        public Graph Dungeon;

        [TestMethod]
        public void Test()
        {
            Dungeon = new Graph();

            int victoryPathLength = 10;
            REssential[] victoryPath = new REssential[victoryPathLength];
            for (int i = 0; i < victoryPathLength; i++)
            {
                victoryPath[i] = new REssential();
                Dungeon.AddVertex(victoryPath[i]);
                if(i > 0)
                {
                    Dungeon.AddEdge(new Edge(victoryPath[i], victoryPath[i - 1]));
                }
            }               

            for (int i = 0; i < 2; i++)
            {
                var builder = new ReplacementRuleBuilder();

                builder.MappedVertex<REssential>("loop")
                    .MappedVertexWithEdge<REssential, Edge>()
                    .MappedVertexWithEdge<REssential, Edge>()
                    .MappedVertexWithEdge<REssential, Edge>()
                    .ReplacementVertexWithEdge<RBasic, Edge>()
                    .ReplacementEdge<Edge>()
                    .MoveToTag("loop");

                ReplacementRule addShortcut = builder.GetResult();

                Assert.IsTrue(addShortcut.Pattern.Vertices.Count == 4);
                Assert.IsTrue(addShortcut.Mapping.Count == 4);
                Assert.IsTrue(addShortcut.Replacement.Vertices.Count == 5);

                bool success = Dungeon.Replace(addShortcut, randomMatch: true);
                Assert.IsTrue(success);
            }

            var bossTreasure = new ReplacementRule();

            var anchorP = new RBasic();
            bossTreasure.Pattern.AddVertex(anchorP);

            var anchorR = new RBasic();
            var boss = new RBoss();
            var treasure = new RTreasure();
            bossTreasure.Replacement.AddVertex(anchorR);
            bossTreasure.Replacement.AddVertex(boss);
            bossTreasure.Replacement.AddVertex(treasure);
            bossTreasure.Replacement.AddEdge(new Edge(anchorR, boss));
            bossTreasure.Replacement.AddEdge(new Edge(boss, treasure));
            bossTreasure.Mapping[anchorP] = anchorR;

            Assert.IsTrue(bossTreasure.Pattern.Vertices.Count == 1);
            Assert.IsTrue(bossTreasure.Mapping.Count == 1);
            Assert.IsTrue(bossTreasure.Replacement.Vertices.Count == 3);

            bool success2 = Dungeon.Replace(bossTreasure, randomMatch: true);
            Assert.IsTrue(success2);

            File.WriteAllText("dungeon.gv", GraphPrinter.ToDot(Dungeon));
        }
    }
}
