import java.util.Random;
import java.util.Scanner;

class Node {
    boolean isSegment; // equals false if this node represents a hole
    int location;      // position in memory of first byte
    int size;
    int timeToDepart;  // only valid when this node represents a Segment
    Node next;

    /*
    constructor for a Segment
     */
    Node(int location, int size, int timeToDepart, Node next) {
        isSegment = true;
        this.location = location;
        this.size = size;
        this.timeToDepart = timeToDepart;
        this.next = next;
    }

    /*
    constructor for a hole
     */
    Node(int location, int size, Node next) {
        isSegment = false;
        this.location = location;
        this.size = size;
        this.next = next;
    }
}

class Memory {
    private Node head; // First node in memory whether it is a hole or a segment.
    private Node lastPlacement; // The last Segment placed, or a hole if that segment is removed.
    private int currentTime; // simplifies recursiveRemove when current time is kept as a field.
    private int timeToDepart; // time for newly created segment to depart
    private boolean verbose; // print segment placement confirmation if true

    /*
    constructor for Memory, generates a single hole Node of the given size
     */
    Memory(int size) {
        head = new Node(0, size, null);
        lastPlacement = head;
    }

    /**
     * attempts to place segment using next fit policy.
     *
     * @param size      size of segment
     * @param timeOfDay current time of day
     * @param lifeTime  life time of the segment
     * @param verbose   will print placement confirmation if true
     * @return true if placement successful
     */
    boolean place(int size, int timeOfDay, int lifeTime, boolean verbose) {
        currentTime = timeOfDay;
        timeToDepart = currentTime + lifeTime;
        this.verbose = verbose;
        boolean placed = false;

        if (isPlacedFromLastPlacement(size))
            placed = true;
        else if (isPlacedFromHead(size))
            placed = true;
        return placed;
    }

    /**
     * attempts to place segment starting from lastPlacement
     *
     * @param size size of segment to place
     * @return true if placed
     */
    private boolean isPlacedFromLastPlacement(int size) {
        Node current = lastPlacement.next;
        Node previous = lastPlacement;
        return place(size, current, previous, true);
    }

    /**
     * attempts to place starting from the head
     *
     * @param size size of segment to place
     * @return true if placed
     */
    private boolean isPlacedFromHead(int size) {
        Node current = head;
        Node previous = head;
        boolean condition = current.location <= lastPlacement.location;
        return place(size, current, previous, condition);
    }

    /**
     * @param size      size of segment to place
     * @param current   current node to check
     * @param previous  previous node used for placement
     * @param condition parametric variable enabling the reuse of the while loop
     * @return true if placed
     */
    private boolean place(int size, Node current, Node previous, boolean condition) {
        boolean placed = false;
        while (current != null && !placed && condition) {
            if (isHoleAndBigEnough(size, current)) {
                placeSegment(current, previous, size);
                placed = true;
            } else {
                previous = current;
                current = current.next;
            }
        }
        return placed;
    }

    private boolean isHoleAndBigEnough(int size, Node current) {
        return !current.isSegment && size <= current.size;
    }

    /*
    places the segment next to a resized hole and removes previous hole
     */
    private void placeSegment(Node current, Node previous, int size) {
        Node next = getNext(size, current);
        Node segment = new Node(current.location, size, timeToDepart, next);
        lastPlacement = segment;
        previous.next = segment;

        if (segment.location == 0)
            head = segment;
        if (verbose)
            printConfirmation(size, segment.location, timeToDepart);
    }

    /**
     * prints a confirmation of placement
     *
     * @param size         node's size
     * @param location     node's location
     * @param timeToDepart node's Time to depart
     */
    private void printConfirmation(int size, int location, int timeToDepart) {
        String format = "Segment of size %4d placed at time %4d at location %4d, departs at %4d";
        System.out.printf(format, size, currentTime, location, timeToDepart);
        System.out.println();
    }

    /**
     * The extra space the segment does not fill is resized to a new hole.
     *
     * @param size    the size of the original hole
     * @param current the hole the segment is being placed
     * @return a resized hole
     */
    private Node getNext(int size, Node current) {
        Node next;
        int newHoleSize = current.size - size;

        if (newHoleSize > 0) {
            int newHoleLocation = current.location + size;
            next = new Node(newHoleLocation, newHoleSize, current.next);
        } else {
            next = current.next;
        }
        return next;
    }

    /**
     * remove segments whose time to depart has occurred
     *
     * @param timeOfDay current Time of day
     */
    void removeSegmentsDueToDepart(int timeOfDay) {
        currentTime = timeOfDay;
        head = recursiveRemove(head);
    }

    /*
    recursively removes segments due to depart and combines neighboring holes
     */
    private Node recursiveRemove(Node x) {
        if (x == null) // end of list
            return null;
        if (isReadyToDepartAndNearAHole(x)) {
            Node combinedHole = getCombinedHole(x);
            return recursiveRemove(combinedHole);
        }
        if (isReadyToDepart(x)) {
            Node hole = getHole(x);
            hole.next = recursiveRemove(x.next);
            return hole;
        }
        x.next = recursiveRemove(x.next);
        return x;
    }

    private boolean isReadyToDepartAndNearAHole(Node x) {
        return isHoleOrReady(x) && isHoleOrReady(x.next);
    }

    private boolean isHoleOrReady(Node x) {
        return x != null && (!x.isSegment || (x.timeToDepart <= currentTime));
    }

    private boolean isReadyToDepart(Node x) {
        return x != null && x.isSegment && x.timeToDepart <= currentTime;
    }

    private Node getCombinedHole(Node x) {
        int combinedHoleSize = x.size + x.next.size;
        Node combinedHole = new Node(x.location, combinedHoleSize, x.next.next);
        fixLastPlacement(x, combinedHole);
        return combinedHole;
    }

    private Node getHole(Node x) {
        Node hole = new Node(x.location, x.size, x.next);
        fixLastPlacement(x, hole);
        return hole;
    }

    private void fixLastPlacement(Node x, Node hole) {
        if (lastPlacement == x || lastPlacement == x.next)
            lastPlacement = hole;
    }

    /**
     * prints a 3-column tab-separated list of all segments in Memory.
     */
    void printLayout() {
        Node current = head;
        while (current != null) {
            if (current.isSegment) {
                System.out.println(current.location + "\t" + current.size
                        + "\t" + current.timeToDepart);
            }
            current = current.next;
        }
    }
}

public class Main {
    private Memory memory;
    private int timeOfDay; // The simulated wall clock, begins with zero private
    int placements; // number of placements completed, begins with zero
    private long totalSpaceTime; // the sum of the placed segmentSize(i) x segmentLifetime(i)
    private Scanner input;

    /* constructor that takes an InputStream to make this program testable with Junit */
    Main(java.io.InputStream in) {
        input = new Scanner(in);
    }

    void run() {
        boolean done = false;

        while (!done) {
            String line = input.nextLine();
            String[] tokens = line.split(" ");

            done = handleInput(tokens);
        }
    }

    /**
     * @param tokens input tokens
     * @return true if done
     */
    private boolean handleInput(String[] tokens) {
        boolean done = false;
        switch (tokens[0]) {

            case "N": {
                System.out.println("William Bush");
                break;
            }

            case "C": {
                int size = Integer.parseInt(tokens[1]);
                memory = new Memory(size);
                break;
            }

            case "A": {
                int size = Integer.parseInt(tokens[1]);
                int lifeTime = Integer.parseInt(tokens[2]);
                addSegment(size, lifeTime, true);
                break;
            }

            case "R": {
                randomMemoryWalk(tokens);
                break;
            }

            case "P": {
                memory.printLayout();
                break;
            }

            case "E": {
                done = true;
                break;
            }
        }
        return done;
    }

    private void randomMemoryWalk(String[] tokens) {
        placements = timeOfDay = 0; // reset
        totalSpaceTime = 0;
        int size = Integer.parseInt(tokens[1]);
        memory = new Memory(size);
        int minSize = Integer.parseInt(tokens[2]);
        int maxSize = Integer.parseInt(tokens[3]);
        int maxLifeTime = Integer.parseInt(tokens[4]);
        int placementLimit = Integer.parseInt(tokens[5]);

        Random r = new Random();
        while (placements < placementLimit) {
            // random int on interval [min, max]
            int newSegSize = r.nextInt(maxSize - minSize + 1) + minSize;
            // random int on interval [1, max]
            int newSegLifetime = r.nextInt(maxLifeTime) + 1;
            totalSpaceTime += newSegSize * newSegLifetime;

            addSegment(newSegSize, newSegLifetime, false);
        }
        printRandomWalkResults();
    }

    private void addSegment(int size, int lifeTime, boolean verbose) {
        timeOfDay++;
        memory.removeSegmentsDueToDepart(timeOfDay);

        while (!memory.place(size, timeOfDay, lifeTime, verbose)) {
            timeOfDay++;
            memory.removeSegmentsDueToDepart(timeOfDay);
        }
        placements++;
    }

    private void printRandomWalkResults() {
        double meanOccupancy = totalSpaceTime / timeOfDay;
        System.out.printf("Number of placements made = %6d\n", placements);
        System.out.printf("Mean occupancy of memory = %8.2f\n", meanOccupancy);
    }

    public static void main(String[] args) {
        Main program = new Main(System.in);
        program.run();
    }
}
