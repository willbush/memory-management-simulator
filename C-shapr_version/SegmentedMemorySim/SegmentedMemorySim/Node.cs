namespace SegmentedMemorySim
{

    internal abstract class Node
    {
        internal bool IsSegment;
        internal int TimeToDepart;
        internal int Location;
        internal int Size;
        internal Node Next;
    }

    internal class Segment : Node
    {

        internal Segment(int location, int size, int timeToDepart, Node next)
        {
            IsSegment = true;
            Location = location;
            Size = size;
            TimeToDepart = timeToDepart;
            Next = next;
        }
    }

    internal class Hole : Node
    {
        internal Hole(int location, int size, Node next)
        {
            Location = location;
            Size = size;
            Next = next;
        }
    }
}
