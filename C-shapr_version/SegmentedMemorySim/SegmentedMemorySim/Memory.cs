using System;

namespace SegmentedMemorySim
{
    class Memory
    {
        private Node _head;
        private Node _lastPlacement;
        private int _currentTime;
        private int _timeToDepart;

        internal Memory(int size)
        {
            _head = new Hole(0, size, null);
            _lastPlacement = _head;
        }

        internal bool TryPlace(int size, int timeOfDay, int lifeTime)
        {
            _currentTime = timeOfDay;
            _timeToDepart = timeOfDay + lifeTime;
            return TryPlaceFromLastSegment(size) || TryPlaceFromHead(size);
        }

        private bool TryPlaceFromLastSegment(int size)
        {
            var current = _lastPlacement.Next;
            var previous = _lastPlacement;
            var placed = false;

            while (current != null && !placed)
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
            }
            return placed;
        }

        private bool TryPlaceFromHead(int size)
        {
            var current = _lastPlacement.Next;
            var previous = _lastPlacement;
            var placed = false;

            while (NotPlacedAndLastPlacementNotReached(current, placed))
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
            }
            return placed;
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
