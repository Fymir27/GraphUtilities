using System;
using System.Collections.Generic;
using System.Text;

namespace GraphUtilities
{
    public static class GraphPrinter
    {
        public static string ToDot(Graph G, bool printVertexData = true, bool printEdgeData = false)
        {
            var sb = new StringBuilder();

            sb.AppendLine("strict graph {");

            List<Edge> usedEdges = new List<Edge>();
            foreach (Vertex V in G.Vertices)
            {
                if(printVertexData)
                    sb.AppendFormat("{0} [label = \"{1}\"];\n", V.ID, V.ToString());

                foreach (Edge E in V.Edges)
                {
                    if (usedEdges.Contains(E))
                        continue;
                    usedEdges.Add(E);
                  
                    sb.AppendFormat("{0} -- {1}", E.V1.ID, E.V2.ID);
                    
                    if(printEdgeData)
                        sb.AppendFormat(" [label = \"{0}\"]", E.ToString());

                    sb.AppendLine(";");
                }
            }

            sb.Append("}");
            return sb.ToString();
        }
    }
}
