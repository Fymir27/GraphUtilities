using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphUtilities
{
    /// <summary>
    /// A rule by which to replace a subgraph defined by "Pattern"
    /// with a "Replacement" graph with the mapping between them beeing
    /// defined by "Mapping
    /// </summary>
    public class ReplacementRule
    {
        public Graph Pattern = new Graph();
        public Graph Replacement = new Graph();
        public Dictionary<Vertex, Vertex> Mapping = new Dictionary<Vertex, Vertex>();
    }

    /// <summary>
    /// Class that provides a fluent interface for creating a ReplacementRule
    /// </summary>
    public class ReplacementRuleBuilder
    {
        /// <summary>
        /// possible states of the builders 
        /// internal state machine
        /// </summary>
        enum State
        {
            Start,
            PatternVertex,
            ReplacementVertex,
            MatchedVertex,
            PatternEdge,
            ReplacementEdge,
            MatchedEdge,
            End
        }

        #region attributes
        /// <summary>
        /// internal instance of ReplacementRule that is beeing built
        /// </summary>
        ReplacementRule Result;

        /// <summary>
        /// last added pattern vertex
        /// </summary>
        Vertex currentPatternVertex = null;

        /// <summary>
        /// last added replacement vertex
        /// </summary>
        Vertex currentReplacementVertex = null;

        /// <summary>
        /// current uncompleted pattern edge
        /// </summary>
        Edge currentPatternEdge = null;

        /// <summary>
        /// current uncompleted replacement edge
        /// </summary>
        Edge currentReplacementEdge = null;

        /// <summary>
        /// maps tags to corresponding pattern vertices
        /// </summary>
        Dictionary<string, Vertex> taggedPatternVertices;

        /// <summary>
        /// maps tags to corresponding replacement vertices
        /// </summary>
        Dictionary<string, Vertex> taggedReplacementVertices;

        /// <summary>
        /// current internal state
        /// </summary>
        State currentState = State.Start;

        /// <summary>
        /// last valid state (before State.End was reached)
        /// </summary>
        State lastValidState = State.Start;

        /// <summary>
        /// if true state does not get changed by ChangeState()
        /// </summary>
        bool freezeState = false;

        /// <summary>
        /// maps a state to possible next states
        /// </summary>
        static Dictionary<State, State[]> possibleNextStates = new Dictionary<State, State[]>
            {
                {
                    State.Start, new[]
                    {
                        State.PatternVertex,
                        State.ReplacementVertex,
                        State.MatchedVertex
                    }
                },
                {
                    State.PatternVertex, new[]
                    {
                        State.PatternEdge,
                        State.End
                    }
                },
                {
                    State.ReplacementVertex, new[]
                    {
                        State.ReplacementEdge,
                        State.End
                    }
                },
                {
                    State.MatchedVertex, new[]
                    {
                        State.PatternEdge,
                        State.ReplacementEdge,
                        State.MatchedEdge,
                        State.End
                    }
                },
                {
                    State.PatternEdge, new[]
                    {
                        State.PatternVertex
                    }
                },
                {
                    State.ReplacementEdge, new[]
                    {
                        State.ReplacementVertex
                    }
                },
                {
                    State.MatchedEdge, new[]
                    {
                        State.MatchedVertex
                    }
                },
                {
                    State.End, new State[0]
                }
            };
        #endregion

        /// <summary>
        /// basic constructor - initializes attributes and state
        /// </summary>
        public ReplacementRuleBuilder()
        {
            currentState = State.Start;
            Result = new ReplacementRule();
            taggedPatternVertices = new Dictionary<string, Vertex>();
            taggedReplacementVertices = new Dictionary<string, Vertex>();
        }

        #region basic vertex adding methods
        /// <summary>
        /// Adds a pattern vertex to the rule 
        /// and finishes the current uncomplete pattern edge 
        /// </summary>
        /// <param name="vertex">pattern vertex to add</param>
        /// <param name="tag">string to identify vertex later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder PatternVertex(Vertex vertex, string tag = null)
        {
            ChangeState(State.PatternVertex);

            if (vertex == null)
                throw new ArgumentNullException("vertex");

            Result.Pattern.AddVertex(vertex);
            FinalizePatternEdge(vertex);
            currentPatternVertex = vertex;

            if (tag != null)
            {
                taggedPatternVertices[tag] = vertex;
            }

            return this;
        }

        /// <summary>
        /// Adds a pattern vertex to the rule 
        /// and finishes the current uncomplete pattern edge 
        /// </summary>
        /// <typeparam name="TVertex">type of pattern vertex to create</typeparam>
        /// <param name="tag">string to identify vertex later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder PatternVertex<TVertex>(string tag = null) where TVertex : Vertex, new()
        {
            return PatternVertex(new TVertex(), tag);
        }

        /// <summary>
        /// Adds a replacement vertex to the rule 
        /// and finishes the current uncomplete replacement edge 
        /// </summary>
        /// <param name="vertex">replacement vertex to add</param>
        /// <param name="tag">string to identify vertex later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder ReplacementVertex(Vertex vertex, string tag = null)
        {
            ChangeState(State.ReplacementVertex);

            Result.Replacement.AddVertex(vertex);
            FinalizeReplacementEdge(vertex);
            currentReplacementVertex = vertex;

            if (tag != null)
                taggedReplacementVertices[tag] = vertex;

            return this;
        }

        /// <summary>
        /// Adds a replacement vertex to the rule 
        /// and finishes the current uncomplete replacement edge 
        /// </summary>
        /// <typeparam name="TVertex">type of replacement vertex to create</typeparam>
        /// <param name="tag">string to identify vertex later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder ReplacementVertex<TVertex>(string tag = null) where TVertex : Vertex, new()
        {
            return ReplacementVertex(new TVertex(), tag);
        }

        /// <summary>
        /// Adds a pattern and replacement vertex to the rule, maps one to the other
        /// and finishes the current uncomplete pattern and replacement edge 
        /// </summary>
        /// <param name="patternVertex">pattern vertex to add</param>
        /// <param name="replacementVertex">replacement vertex to add</param>
        /// <param name="tag">string to identify the vertices later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder MappedVertex<TVertex>(TVertex patternVertex, TVertex replacementVertex, string tag = null) where TVertex : Vertex
        {
            ChangeState(State.MatchedVertex);
            freezeState = true;
            PatternVertex(patternVertex, tag);
            ReplacementVertex(replacementVertex, tag);
            freezeState = false;
            Result.Mapping.Add(currentPatternVertex, currentReplacementVertex);
            return this;
        }

        /// <summary>
        /// Adds a pattern and replacement vertex to the rule, maps one to the other
        /// and finishes the current uncomplete pattern and replacement edge 
        /// </summary>
        /// <typeparam name="TVertex">type of pattern/replacement vertex to create</typeparam>
        /// <param name="tag">string to identify the vertices later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder MappedVertex<TVertex>(string tag = null) where TVertex : Vertex, new()
        {
            return MappedVertex(new TVertex(), new TVertex(), tag);
        }
        #endregion

        #region basic edge adding methods
        /// <summary>
        /// Adds a new (incomplete) edge to the last added pattern vertex  
        /// </summary>
        /// <param name="edge">edge to add</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder PatternEdge(Edge edge)
        {
            ChangeState(State.PatternEdge);

            currentPatternEdge = edge;
            return this;
        }

        /// <summary>
        /// Adds a new (incomplete) edge to the last added pattern vertex  
        /// </summary>
        /// <typeparam name="TEdge">type of edge to add</typeparam>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder PatternEdge<TEdge>() where TEdge : Edge, new()
        {
            return PatternEdge(new TEdge());
        }

        /// <summary>
        /// Adds a new (incomplete) edge to the last added replacement vertex  
        /// </summary>
        /// <param name="edge">edge to add</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder ReplacementEdge(Edge edge)
        {
            ChangeState(State.ReplacementEdge);

            currentReplacementEdge = edge;
            return this;
        }

        /// <summary>
        /// Adds a new (incomplete) edge to the last added replacement vertex  
        /// </summary>
        /// <typeparam name="TEdge">type of edge to add</typeparam>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder ReplacementEdge<TEdge>() where TEdge : Edge, new()
        {
            return ReplacementEdge(new TEdge());
        }

        /// <summary>
        /// Adds a new (incomplete) edge to the last added pattern AND replacement (=mapped) vertex  
        /// </summary>
        /// <typeparam name="TEdge">type of edges to add (should be implicit)</typeparam>
        /// <param name="patternEdge">edge to add to param vertex</param>
        /// <param name="replacementEdge">edge to add to replacement vertex</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder MappedEdge<TEdge>(TEdge patternEdge, TEdge replacementEdge) where TEdge : Edge
        {
            ChangeState(State.MatchedEdge);
            freezeState = true;
            PatternEdge(patternEdge);
            ReplacementEdge(replacementEdge);
            freezeState = false;
            return this;
        }

        /// <summary>
        /// Adds a new (incomplete) edge to the last added pattern AND replacement (=mapped) vertex  
        /// </summary>
        /// <typeparam name="TEdge">type of edges to add</typeparam>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder MappedEdge<TEdge>() where TEdge : Edge, new()
        {
            return MappedEdge(new TEdge(), new TEdge());
        }
        #endregion

        /// <summary>
        /// moves the builders state to the vertex with a specific tag (if mapped: the vertices)
        /// </summary>
        /// <param name="tag">tag of the vertex/vertices to move to</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder MoveToTag(string tag)
        {
            bool patternTagged = taggedPatternVertices.TryGetValue(tag, out Vertex patternVertex);
            bool replacementTagged = taggedReplacementVertices.TryGetValue(tag, out Vertex replacementVertex);

            if (!patternTagged && !replacementTagged)
            {
                throw new ArgumentException(tag + " doesnt tag a Vertex!");
            }

            // if we have any dangling edges, they MUST be finshed:
            if (currentPatternEdge != null && !patternTagged)
            {
                throw new ArgumentException(tag + " doesnt tag a patternVertex! -> dangling Edge!");
            }
            if (currentReplacementEdge != null && !replacementTagged)
            {
                throw new ArgumentException(tag + " doesnt tag a replacementVertex! -> dangling Edge!");
            }

            switch (currentState)
            {
                case State.PatternEdge:
                    FinalizePatternEdge(patternVertex);
                    break;
                case State.ReplacementEdge:
                    FinalizeReplacementEdge(replacementVertex);
                    break;
                case State.MatchedEdge:
                    FinalizePatternEdge(patternVertex);
                    FinalizeReplacementEdge(replacementVertex);
                    break;
                case State.End:
                    throw new InvalidOperationException("Please call Continue(tag) instead!");
                default:
                    // all other states are fine with just moving to the new vertex
                    break;
            }

            if (patternTagged)
            {
                if (replacementTagged)
                {
                    currentState = State.MatchedVertex;
                    currentPatternVertex = patternVertex;
                    currentReplacementVertex = replacementVertex;
                }
                else
                {
                    currentState = State.PatternVertex;
                    currentPatternVertex = patternVertex;
                }
            }
            else if (replacementTagged)
            {
                currentState = State.ReplacementVertex;
                currentReplacementVertex = replacementVertex;
            }

            return this;
        }

        /// <summary>
        /// finalizes the building of the rule and returns it
        /// </summary>
        /// <returns>ReplacementRule that was built</returns>
        public ReplacementRule GetResult()
        {
            ChangeState(State.End);
            return Result;
        }

        /// <summary>
        /// allows the builder to continue after it has already finished by calling GetResult() (USE WITH CAUTION)
        /// </summary>
        /// <param name="tag">tag to continue from</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder Continue(string tag = null)
        {
            if (currentState != State.End)
            {
                throw new InvalidOperationException("Continue() should only be called on a Builder in [End] State!");
            }

            if (tag == null)
            {
                currentState = lastValidState;
            }
            else
            {
                currentState = State.Start;
                MoveToTag(tag);
            }

            return this;
        }

        /// <summary>
        /// allows reuse of builder instance by starting a new empty ReplacementRule
        /// </summary>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder Reset()
        {
            if (currentState != State.End)
            {
                throw new InvalidOperationException("Reset() should only be called on a Builder in [End] State! (did you forget to GetResult()?)");
            }

            Result = new ReplacementRule();
            currentPatternVertex = null;
            currentReplacementVertex = null;
            currentReplacementEdge = null;
            currentPatternEdge = null;
            taggedPatternVertices.Clear();
            taggedReplacementVertices.Clear();
            freezeState = false;
            lastValidState = State.Start;
            currentState = State.Start;
            return this;
        }

        #region shortcut methods
        /// <summary>
        /// starts a new pattern edge and finishes it with a pattern vertex
        /// </summary>
        /// <param name="vertex">vertex to add</param>
        /// <param name="edge">edge to add</param>
        /// <param name="tag">string to identify vertex later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder PatternVertexWithEdge(Vertex vertex, Edge edge, string tag = null)
        {
            PatternEdge(edge);
            PatternVertex(vertex, tag);
            return this;
        }

        /// <summary>
        /// starts a new pattern edge and finishes it with a pattern vertex
        /// </summary>
        /// <typeparam name="TVertex">type of vertex to add</typeparam>
        /// <typeparam name="TEdge">type of edge to add</typeparam>
        /// <param name="tag">string to identify vertex later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder PatternVertexWithEdge<TVertex, TEdge>(string tag = null)
            where TVertex : Vertex, new()
            where TEdge : Edge, new()
        {
            PatternEdge<TEdge>();
            PatternVertex<TVertex>(tag);
            return this;
        }

        /// <summary>
        /// starts a new replacement edge and finishes it with a replacement vertex
        /// </summary>
        /// <param name="vertex">vertex to add</param>
        /// <param name="edge">edge to add</param>
        /// <param name="tag">string to identify vertex later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder ReplacementVertexWithEdge(Vertex vertex, Edge edge, string tag = null)
        {
            ReplacementEdge(edge);
            ReplacementVertex(vertex, tag);
            return this;
        }

        /// <summary>
        /// starts a new replacement edge and finishes it with a replacement vertex
        /// </summary>
        /// <typeparam name="TVertex">type of vertex to add</typeparam>
        /// <typeparam name="TEdge">type of edge to add</typeparam>
        /// <param name="tag">string to identify vertex later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder ReplacementVertexWithEdge<TVertex, TEdge>(string tag = null)
            where TVertex : Vertex, new()
            where TEdge : Edge, new()
        {
            ReplacementEdge<TEdge>();
            ReplacementVertex<TVertex>(tag);
            return this;
        }

        /// <summary>
        /// Starts a pattern and a replacment edge and finishes them with a pattern and replacement vertex
        /// </summary>
        /// <typeparam name="TVertex">type of vertex (implicit)</typeparam>
        /// <typeparam name="TEdge">type of edge (implicit)</typeparam>
        /// <param name="patternVertex">pattern vertex to add</param>
        /// <param name="replacementVertex">replacement vertex to add</param>
        /// <param name="patternEdge">edge to add to pattern vertex</param>
        /// <param name="replacementEdge">edge to add to replacment vertex</param>
        /// <param name="tag">string to identify vertex later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder MappedVertexWithEdge<TVertex, TEdge>(TVertex patternVertex, TVertex replacementVertex,
            TEdge patternEdge, TEdge replacementEdge, string tag = null)
            where TVertex : Vertex
            where TEdge : Edge
        {
            MappedEdge(patternEdge, replacementEdge);
            MappedVertex(patternVertex, replacementVertex, tag);
            return this;
        }

        /// <summary>
        /// Starts a pattern and a replacment edge and finishes them with a pattern and replacement vertex
        /// </summary>
        /// <typeparam name="TVertex">type of vertices to add</typeparam>
        /// <typeparam name="TEdge">type of edges to add</typeparam>
        /// <param name="tag">string to identify vertex later</param>
        /// <returns>builder instance</returns>
        public ReplacementRuleBuilder MappedVertexWithEdge<TVertex, TEdge>(string tag = null)
            where TVertex : Vertex, new()
            where TEdge : Edge, new()
        {
            MappedEdge<TEdge>();
            MappedVertex<TVertex>(tag);
            return this;
        }
        #endregion

        #region private methods
        /// <summary>
        /// finalizes current uncompleted pattern edge
        /// </summary>
        /// <param name="vertex">vertex to complete edge with</param>
        private void FinalizePatternEdge(Vertex vertex)
        {
            if (currentPatternEdge != null)
            {
                currentPatternEdge.Init(currentPatternVertex, vertex);
                Result.Pattern.AddEdge(currentPatternEdge);
                currentPatternEdge = null;
            }
        }

        /// <summary>
        /// finalizes current uncompleted replacment edge
        /// </summary>
        /// <param name="vertex">vertex to complete edge with</param>
        private void FinalizeReplacementEdge(Vertex vertex)
        {
            if (currentReplacementEdge != null)
            {
                currentReplacementEdge.Init(currentReplacementVertex, vertex);
                Result.Replacement.AddEdge(currentReplacementEdge);
                currentReplacementEdge = null;
            }
        }

        /// <summary>
        /// Changes internal state to a new one
        /// </summary>
        /// <param name="newState">new stat to change to</param>
        private void ChangeState(State newState)
        {
            if (freezeState)
            {
                return;
            }

            if (possibleNextStates[currentState].Contains(newState))
            {
                currentState = newState;
            }
            else
            {
                throw new InvalidOperationException(String.Format("Cannot go from {0} to {1}", currentState.ToString(), newState.ToString()));
            }

            if (currentState != State.End)
            {
                lastValidState = currentState;
            }
        }
    }
    #endregion
}
