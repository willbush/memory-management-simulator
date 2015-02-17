import org.junit.Before;
import org.junit.Test;

import java.io.ByteArrayInputStream;
import java.util.Random;

import static junit.framework.Assert.assertEquals;

public class UnitTests {
    int timeOfDay, placements;

    @Before
    public void start() {
        timeOfDay = placements = 0;
        System.out.println();
        System.out.println("======= Unit test starting =======");
    }

    @Test
    public void addExample1() {
        String input = "N\n" +
                "C 100\n" +
                "A 20 10\n" +
                "A 50 5\n" +
                "A 70 20\n" +
                "P\n" +
                "E";
        Main program = getWibup1(input);
        program.run();

        String expectedOutput = "William Bush\n" +
                "Segment of size   20 placed at time    1 at location    0, departs at   11\n" +
                "Segment of size   50 placed at time    2 at location   20, departs at    7\n" +
                "Segment of size   70 placed at time    7 at location   20, departs at   27\n" +
                "0\t20\t11\n" +
                "20\t70\t27";
        System.out.println("======= Expected output =======");
        System.out.println(expectedOutput);
    }

    @Test
    public void randomExample1() {
        String input = "N\n" +
                "R 100 5 25 20 10000\n" +
                "E";
        Main program = getWibup1(input);
        program.run();
        expectedOutputForRandom(10000, 74.86);
    }

    @Test
    public void randomExample2() {
        String input = "N\n" +
                "R 100 5 25 40 10000\n" +
                "E";
        Main program = getWibup1(input);
        program.run();
        expectedOutputForRandom(10000, 77.27);
    }

    @Test
    public void randomExample3() {
        String input = "N\n" +
                "R 1000 5 100 80 10000\n" +
                "E";
        Main program = getWibup1(input);
        program.run();
        expectedOutputForRandom(10000, 794.35);
    }

    private void expectedOutputForRandom(int placements, double meanOccupancy) {
        String expectedOutput = "William Bush\n" +
                "Number of placements made =  " + placements + "\n" +
                "Mean occupancy of memory =    " + meanOccupancy + "\n";
        System.out.println("======= Expected output =======");
        System.out.println(expectedOutput);
    }

    @Test
    public void placeSegmentTest() {
        Memory m = new Memory(100);

        for (int i = 0; i < 1000; i++) {
            insert(m, 10, 5, false);
        }
		assertEquals(1000, placements);
    }

	@Test
	public void randomPlacementTest() {
		Memory m = new Memory(1000);
		randomWalk(m, 5, 50, 20, 9000);
		assertEquals(9000, placements);
	}

    private Main getWibup1(String input) {
        ByteArrayInputStream in = new ByteArrayInputStream(input.getBytes());
        return new Main(in);
    }

	private void randomWalk(Memory m, int min, int max, int maxLifeTime, int limit) {
		Random r = new Random();
		while (placements < limit) {
			// random int on interval [min, max]
			int newSegSize = r.nextInt(max - min + 1) + min;
			// random int on interval [1, max]
			int newSegLifetime = r.nextInt(maxLifeTime) + 1;

			insert(m, newSegSize, newSegLifetime, false);
		}
	}

    private void insert(Memory m, int size, int lifeTime, boolean verbose) {
        timeOfDay++;
        m.removeSegmentsDueToDepart(timeOfDay);

        while (!m.place(size, timeOfDay, lifeTime, verbose)) {
            timeOfDay++;
            m.removeSegmentsDueToDepart(timeOfDay);
        }
        placements++;
    }
}
