using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;

class Program
{
    static void Main()
    {
        // Define menu prompts as a string constant
        const string menuPrompts = "Select one of the following options\n" +
                "g) Add 'Guard' obstacle\n" +
                "f) Add 'Fence' obstacle\n" +
                "s) Add 'Sensor' obstacle\n" +
                "c) Add 'Camera' obstacle\n" +
                "b) Add 'Bottomless Pit' obstacle\n" +
                "d) Show safe directions\n" +
                "m) Display obstacle map\n" +
                "p) Find safe path\n" +
                "Enter code:";
        
        // Create a new instance of the Intelligence class
        Intelligence intelligence = new Intelligence();
        
        
        // Main program loop
        while (true)
        {
            Console.WriteLine(menuPrompts);

            char InputCode;
            string userInput = Console.ReadLine();

            // Check if userInput is valid
            if (userInput == "g"
                 || userInput == "f"
                 || userInput == "s"
                 || userInput == "c"
                 || userInput == "b"
                 || userInput == "d"
                 || userInput == "m"
                 || userInput == "p"
                 || userInput == "x") 
            {
                InputCode = char.ToLower(userInput[0]);
            }
            else
            {
                Console.WriteLine("Invalid option.");
                continue; // Skip to the next iteration of the loop if invalid input received.
            }


            // Switch statement to handle different cases based on user input.
            switch (InputCode)
            {
                case 'g': // Add Guard Obstacle
                    Console.WriteLine("Enter the guard's location (X,Y):");
                    intelligence.AddObstacle(new Guard(GetLocation())); // Create a new Guard object and add it to obstacles list in intelligence object.
                    break;
                case 'f': // Add Fence Obstacle
                    Console.WriteLine("Enter the location where the fence starts (X,Y):");
                    Location startPoint = GetLocation();
                    Console.WriteLine("Enter the location where the fence ends (X,Y):");
                    Location endPoint = GetLocation();
                    intelligence.AddObstacle(new Fence(startPoint, endPoint)); // Create a new Fence object and add it to obstacles list in intelligence object.
                    break;
                case 's': // Add Sensor Obsacle
                    Console.WriteLine("Enter the sensor's location (X,Y):");
                    Location sensorLocation = GetLocation();

                    Console.WriteLine("Enter the sensor's range (in klicks):");
                    double sensorRange;
                    while (!double.TryParse(Console.ReadLine(), out sensorRange) || sensorRange <= 0)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid positive number for the range."); // CHANGE LATER
                    }
                    intelligence.AddObstacle(new Sensor(sensorLocation, sensorRange));
                    break;
                case 'c': // Add Camera Obstacle
                    Console.WriteLine("Enter the camera's location (X,Y):");
                    Location cameraLocation = GetLocation();
                    Console.WriteLine("Enter the direction the camera is facing (n, s, e, w):");
                    char cameraDirection;
                    while (!char.TryParse(Console.ReadLine(), out cameraDirection) || (cameraDirection != 'n' && cameraDirection != 's' && cameraDirection != 'e' && cameraDirection != 'w'))
                    {
                        Console.WriteLine("Invalid direction. Please enter n, s, e, or w.");
                    }
                    intelligence.AddObstacle(new Camera(cameraLocation, cameraDirection));
                    break;
                case 'b':
                    Console.WriteLine("Each 'Bottomless Pit' has a size of 3x3. Enter the bottomless pit's centre location (X,Y):");
                    intelligence.AddObstacle(new BottomlessPit(GetLocation()));
                    break;
                case 'd': // Show Safe Directions
                    Console.WriteLine("Enter your current location (X,Y):");
                    intelligence.ShowSafeDirections(GetLocation());
                    break;
                case 'm': // Show obstacle map
                    Console.WriteLine("Enter the location of the top-left cell of the map (X,Y):");
                    Location topLeft = GetLocation();

                    Console.WriteLine("Enter the location of the bottom-right cell of the map (X,Y):");
                    Location bottomRight = GetLocation();

                    intelligence.DisplayObstacleMap(topLeft, bottomRight);
                    break;
                case 'p':
                    {
                        Console.WriteLine("Enter your current location (X,Y):");
                        var startlocation = GetLocation();
                        Console.WriteLine("Enter the location of your objective (X,Y):");
                        var endlocation = GetLocation();


                        if (startlocation.Equals(endlocation)) // Checks if the agent is already at the location of the mission objective.

                        {
                            Console.WriteLine("Agent, you are already at the objective.");
                        }
                        if (!intelligence.IsLocationSafe(endlocation)) // Check whether the endLocation is safe and not obstructed by obstacles.
                        {
                            Console.WriteLine("The objective is blocked by an obstacle and cannot be reached.");
                            break;
                        }
                        
                        // Implement an algorithm to determine path finding to the agent's mission objective.
                        string safePath = intelligence.FindSafePath(startlocation, endlocation);

                        if (string.IsNullOrEmpty(safePath))
                        {
                            Console.WriteLine("There is no safe path to the objective.");
                        }
                        else
                        {
                            Console.WriteLine("The following path will take you to the objective:");
                            Console.WriteLine(safePath);
                        }
                    }
                    break;
                case 'x': // Close the program
                    return;
                default:
                    Console.WriteLine("Invalid Input.");
                    break;
            }
        }
    }
    /* Method GetLocation is used throughout main method whenever it needs a location from the user.
     * It reads line from console, splits it by comma into X and Y parts and creates a new Location object with those coordinates. */
    static Location GetLocation()
    {
        while (true)
        {
            try
            {
                string[] coordinates = Console.ReadLine().Split(',');
                int x = int.Parse(coordinates[0]);
                int y = int.Parse(coordinates[1]);
                return new Location(x, y);
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid input.");
                continue;
            }
        }
    }
}

