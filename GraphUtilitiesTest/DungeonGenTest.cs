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
        //class Connection : Edge { }

        class ReplacementRule
        {
            public Graph Pattern = new Graph();
            public Graph Replacement = new Graph();
            public Dictionary<Vertex, Vertex> Mapping = new Dictionary<Vertex, Vertex>();
        }

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

            ReplacementRule shortcut = new ReplacementRule();
            var vp1 = new REssential();
            var vp2 = new REssential();
            var vp3 = new REssential();
            shortcut.Pattern.AddVertex(vp1);
            shortcut.Pattern.AddVertex(vp2);
            shortcut.Pattern.AddVertex(vp3);
            shortcut.Pattern.AddEdge(new Edge(vp1, vp2));
            shortcut.Pattern.AddEdge(new Edge(vp2, vp3));

            var vr1 = new REssential();
            var vr2 = new REssential();
            var vr3 = new REssential();
            var vr4 = new RBasic();
            shortcut.Replacement.AddVertex(vr1);
            shortcut.Replacement.AddVertex(vr2);
            shortcut.Replacement.AddVertex(vr3);
            shortcut.Replacement.AddVertex(vr4);
            shortcut.Replacement.AddEdge(new Edge(vr1, vr2));
            shortcut.Replacement.AddEdge(new Edge(vr2, vr3));
            shortcut.Replacement.AddEdge(new Edge(vr1, vr4));
            shortcut.Replacement.AddEdge(new Edge(vr3, vr4));

            shortcut.Mapping.Add(vp1, vr1);
            shortcut.Mapping.Add(vp2, vr2);          
            shortcut.Mapping.Add(vp3, vr3);

            bool success = Dungeon.Replace(shortcut.Pattern, shortcut.Replacement, shortcut.Mapping, true);
            success = Dungeon.Replace(shortcut.Pattern, shortcut.Replacement, shortcut.Mapping, true);

            File.WriteAllText("dungeon.gv", GraphPrinter.ToDot(Dungeon));
        }
    }
}
