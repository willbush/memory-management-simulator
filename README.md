Memory Management Simulator
======

This was the first project assigned in my intro to data structures and algorithms class. The project was completed individually and made to specification. Briefly, the specification called for simulating arrivals, departures, and placements of segments of memory using a singly linked list. A node in the list represents either a “hole” (free memory block) or a segment and the size simply an integer, where 1 = 1 byte. The specification also called for using “next fit” placement policy.

For placement I used iteration and for removal I decided to use recursion, which seemed to simplify the problem. When removing, care was taken to not allow two holes to exist in memory while next to each other. Also, last placement always has to point to the last segment placed or the hole that took it's place when it departed.

