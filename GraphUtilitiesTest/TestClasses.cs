using GraphUtilities;

namespace GraphUtilitiesTest
{
    class IntVertex : DataVertex<int>
    {
        public override bool SameType(Vertex other)
        {
            if (other is IntVertex otherIntV)
            {
                return Data == otherIntV.Data;
            }
            else
            {
                return false;
            }
        }

        public IntVertex(int value) : base(value)
        {

        }
    }

    class StringVertex : DataVertex<string>
    {
        public override bool SameType(Vertex other)
        {
            if (other is StringVertex otherIntV)
            {
                return Data == otherIntV.Data;
            }
            else
            {
                return false;
            }
        }

        public StringVertex(string value) : base(value)
        {

        }
    }

    class StringEdge : DataEdge<string>
    {
        public override bool SameType(Edge other)
        {
            if (other is StringEdge otherIntV)
            {
                return Data == otherIntV.Data;
            }
            else
            {
                return false;
            }
        }

        public StringEdge(Vertex first, Vertex second, string value) : base(first, second, value)
        {

        }
    }
}
