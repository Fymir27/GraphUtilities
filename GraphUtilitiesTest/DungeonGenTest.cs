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

                bool success = Dungeon.Replace(builder.GetResult(), randomMatch: true);

             
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

            //Dungeon.Replace(bossTreasure.Pattern, bossTreasure.Replacement, bossTreasure.Mapping, true);


            File.WriteAllText("dungeon.gv", GraphPrinter.ToDot(Dungeon));
        }
    }
}
