Memory Management Simulator
======

This was the first project assigned in my intro to data structures and algorithms class. The project was completed individually and made to specification. In short, the specification called for simulating arrivals, departures, and placements of segments of memory using a singly linked list. A node in the list represents either a “hole” (free memory block) or a segment and the size simply an integer, where 1 = 1 byte. The specification also called for using “next fit” placement policy.

For placement I used iteration and for removal I decided to use recursion, which seemed to simplify the issue of not allowing two holes to neighbor each other. Also, last placement always has to point to the last segment placed or the hole that took it's place when it departed.

Since no input validation was required in the spec, I have not implemented it. Input given to the program must be "perfect", see examples below.

Input commands and arguments are as follows:

```
Command | arguments | action
----------------------------
N       |           | Prints student name
C       | s         | Create a Memory object of size s
A       | u v       | Add a segment of size u placed at time v
        |           | and print a confirmation record
P       |           | Prints location, size, and timeOfDeparture of all
        |           | segments in memory.
R       | s u v w x | Creates a new Memory object of size s. Simulate
        |           | x randomly generated placements.
E       |           | Ends the run loop, prints a newline and exits.
```

Example input:
```
N
C 100
A 20 10
A 50 5
A 70 20
P
E
```
Example output for given input:
```
John Doe
Segment of size   20 placed at time    1 at location    0, departs at   11
Segment of size   50 placed at time    2 at location   20, departs at    7
Segment of size   70 placed at time    7 at location   20, departs at   27
0	20	11
20	70	27
```
Example input 2:
```
N
R 100 5 25 20 10000
E
```
Example output 2:
```
John Doe
Number of placements made =  10000
Mean occupancy of memory =    74.86
```