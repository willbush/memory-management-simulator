using System;

namespace SegmentedMemorySim
{
    internal class Memory
    {
        private int _currentTime;
        private Node _head;
        private Node _lastPlacement;
        private int _timeToDepart;
        private bool _verbose;

        internal Memory(int size)
        {
            _head = new Hole(0, size, null);
            _lastPlacement = _head;
        }

        internal bool TryPlace(int size, int timeOfDay, int lifeTime, bool verbose)
        {
            _currentTime = timeOfDay;
            _timeToDepart = timeOfDay + lifeTime;
            _verbose = verbose;
            return TryPlaceFromLastPlacement(size) || TryPlaceFromHead(size);
        }

        private bool TryPlaceFromLastPlacement(int size)
        {
            var current = _lastPlacement.Next;
            var previous = _lastPlacement;
            var placed = false;

            while (current != null && !placed)
                current = TryPlaceCurrent(size, current, ref previous, ref placed);

            return placed;
        }

        private bool TryPlaceFromHead(int size)
        {
            var current = _head;
            var previous = _head;
            var placed = false;

            while (NotPlacedAndLastPlacementNotReached(current, placed))
                current = TryPlaceCurrent(size, current, ref previous, ref placed);

            return placed;
        }

        private Node TryPlaceCurrent(int size, Node current, ref Node previous, ref bool placed)
        {
            if (HoleIsBigEnough(size, current))
            {
                PlaceSegment(current, previous, size);
                placed = true;
            }
            else
            {
                previous = current;
                current = current.Next;
            }
            return current;
        }

        private bool NotPlacedAndLastPlacementNotReached(Node current, bool placed)
        {
            return current != null && !placed && current.Location <= _lastPlacement.Location;
        }

        private bool HoleIsBigEnough(int size, Node current)
        {
            return !current.IsSegment && size <= current.Size;
        }

        private void PlaceSegment(Node current, Node previous, int size)
        {
            var next = GetNext(size, current);
            var segment = new Segment(current.Location, size, _timeToDepart, next);
            _lastPlacement = segment;
            previous.Next = segment;

            if (segment.Location == 0)
                _head = segment;

            if (_verbose)
                PrintConfirmation(size, segment.Location, _timeToDepart);
        }

        private Node GetNext(int size, Node current)
        {
            Node next;
            var newHoleSize = current.Size - size;

            if (newHoleSize > 0)
            {
                var newHoleLocation = current.Location + size;
                next = new Hole(newHoleLocation, newHoleSize, current.Next);
            }
            else
            {
                next = current.Next;
            }
            return next;
        }

        private void PrintConfirmation(int size, int location, int timeToDepart)
        {
            var s = string.Format("Segment of size {0,4} placed at time {1,4} at location {2,4}, departs at {3,4}",
                size, _currentTime, location, timeToDepart);
            Console.WriteLine(s);
        }

        internal void RemoveSegmentsDueToDepart(int timeOfDay)
        {
            _currentTime = timeOfDay;
            _head = RecursiveRemove(_head);
        }

        private Node RecursiveRemove(Node x)
        {
            if (x == null)
                return null;

            if (IsReadyToDepartAndNearAHole(x))
                return RecursiveRemove(GetCombinedHole(x));

            if (IsReadyToDepart(x))
            {
                var hole = GetHole(x);
                hole.Next = RecursiveRemove(x.Next);
                return hole;
            }
            x.Next = RecursiveRemove(x.Next);
            return x;
        }

        private bool IsReadyToDepartAndNearAHole(Node x)
        {
            return IsHoleOrReady(x) && IsHoleOrReady(x.Next);
        }

        private Hole GetCombinedHole(Node x)
        {
            var combinedHoleSize = x.Size + x.Next.Size;
            var combinedHole = new Hole(x.Location, combinedHoleSize, x.Next.Next);
            FixLastPlacement(x, combinedHole);
            return combinedHole;
        }

        private bool IsHoleOrReady(Node x)
        {
            return x != null && (!x.IsSegment || (x.TimeToDepart <= _currentTime));
        }

        private bool IsReadyToDepart(Node x)
        {
            return x != null && x.IsSegment && x.TimeToDepart <= _currentTime;
        }

        private Hole GetHole(Node x)
        {
            var hole = new Hole(x.Location, x.Size, x.Next);
            FixLastPlacement(x, hole);
            return hole;
        }

        private void FixLastPlacement(Node x, Hole hole)
        {
            if (_lastPlacement == x || _lastPlacement == x.Next)
                _lastPlacement = hole;
        }

        public void PrintLayout()
        {
            var current = _head;

            while (current != null)
            {
                if (current.IsSegment)
                {
                    Console.WriteLine(current.Location + "\t" + current.Size + "\t" + current.TimeToDepart);
                }
                current = current.Next;
            }
        }
    }
}