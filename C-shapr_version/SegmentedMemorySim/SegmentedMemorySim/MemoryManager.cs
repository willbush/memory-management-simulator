using System;

namespace SegmentedMemorySim
{
    internal class MemoryManager
    {
        private Memory _memory;
        private int _timeOfDay;
        private int _numOfPlacements;
        private long _totalSpaceTime;

        internal MemoryManager()
        { }

        internal void run()
        {
            string[] tokens;

            do
            {
                var line = Console.ReadLine();
                tokens = line.Split();
                PerformCommands(tokens);
            } while (HasInput(tokens[0]));
        }

        private bool HasInput(string command)
        {
            return !command.Equals("E");
        }

        private void PerformCommands(string[] tokens)
        {
            int size;
            switch (tokens[0])
            {
                case "N":
                    Console.WriteLine("William Bush");
                    break;

                case "C":
                    size = Int32.Parse(tokens[1]);
                    _memory = new Memory(size);
                    break;

                case "A":
                    size = Int32.Parse(tokens[1]);
                    int lifeTime = Int32.Parse(tokens[2]);
                    AddSegment(size, lifeTime);
                    break;

                case "R":
                    RandomMemoryWalk(tokens);
                    break;

                case "P":
                    _memory.PrintLayout();
                    break;
            }
        }

        private void RandomMemoryWalk(string[] tokens)
        {
            _numOfPlacements = _timeOfDay = 0; // reset
            _totalSpaceTime = 0;

            int size = Int32.Parse(tokens[1]);

            _memory = new Memory(size);

            int minSize = Int32.Parse(tokens[2]);
            int maxSize = Int32.Parse(tokens[3]);
            int maxLifeTime = Int32.Parse(tokens[4]);
            int placementLimit = Int32.Parse(tokens[5]);

            Random r = new Random();


            while (_numOfPlacements < placementLimit)
            {
                int newSegSize = r.Next(minSize, maxSize);
                int newSegLifeTime = r.Next(1, maxLifeTime);
                _totalSpaceTime += newSegSize * newSegLifeTime;

                AddSegment(newSegSize, newSegLifeTime);
            }
            PrintRandomResults();
        }

        private void AddSegment(int size, int lifeTime)
        {
            _timeOfDay++;
            _memory.RemoveSegmentsDueToDepart(_timeOfDay);

            while (!_memory.TryPlace(size, _timeOfDay, lifeTime))
            {
                _timeOfDay++;
                _memory.RemoveSegmentsDueToDepart(_timeOfDay);
            }
            _numOfPlacements++;
        }

        private void PrintRandomResults()
        {
            double meanOccupancy = _totalSpaceTime / _timeOfDay;
            Console.WriteLine("Number of placements made = {F6}", _numOfPlacements);
            Console.WriteLine("Mean Occupancy of memory = {F2}", meanOccupancy);
        }
    }
}
