using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Intelligence class manages all obstacles and provides methods to add obstacles and evaluate their effects on movement.
class Intelligence
{
    // List to store all obstacles.
    public List<Obstacle> obstacles = new List<Obstacle>();

    // Method to add an obstacle to the list
    public void AddObstacle(Obstacle obstacle)
    {
        obstacles.Add(obstacle);
    }

    // Method to display safe directions based on current location
    public void ShowSafeDirections(Location currentLocation)
    {
        List<char> safeDirections = GetSafeDirections(currentLocation);

        if (safeDirections.Count == 0)
        {
            Console.WriteLine("You cannot safely move in any direction. Abort mission.");
        }
        else
        {
            Console.Write("You can safely take any of the following directions: ");
            Console.WriteLine(string.Join("", safeDirections));
        }
    }


    // Helper method that returns a list of safe directions from a given location
    private List<char> GetSafeDirections(Location currentLocation)
    {
        List<char> safeDirections = new List<char>();

        // Check each direction (N, S, E, W)
        int[] xOffset = { 0, 0, 1, -1 };
        int[] yOffset = { -1, 1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            Location nextLocation = new Location(currentLocation.X + xOffset[i], currentLocation.Y + yOffset[i]);

            if (IsLocationSafe(nextLocation))
            {
                safeDirections.Add(GetDirectionCode(i));
            }
        }

        return safeDirections;
    }

    // Helper method that checks if a given locaiton is safe or not by checking against all existing obstacles.
    public bool IsLocationSafe(Location location)
    {
        foreach (var obstacle in obstacles)
        {
            if (!obstacle.IsPassable(location))
            {
                return false;
            }
        }

        return true;
    }



    // Helper method that returns direction cod ebased on index. 'N' for North, 'S' for South, 'E' for East and 'W' for West.
    private char GetDirectionCode(int index)
    {
        char[] directionCodes = { 'N', 'S', 'E', 'W'};
        return directionCodes[index];
    }

    
    // Method to dispaly obstacle map within specified top left and bottom right margin locations.
    // It prints characters representation different types of obstacles or '.' for safe squares.
    public void DisplayObstacleMap(Location topLeft, Location bottomRight)
    {
        if (topLeft.X > bottomRight.X || topLeft.Y > bottomRight.Y)
        {
            Console.WriteLine("Invalid map specification.");
            return;
        }

        for (int y = topLeft.Y; y <= bottomRight.Y; y++)
        {
            for (int x = topLeft.X; x <= bottomRight.X; x++)
            {
                char mapChar = GetMapCharacter(new Location(x, y));
                Console.Write(mapChar);
            }
            Console.WriteLine();
        }
    }


    // Helper method that returns character represetning type of obstacle at given location. If no obstacle or if it's passable then it returns '.'
    private char GetMapCharacter(Location location)
    {
        // Define obstacle priority order
        Type[] obstaclePriority = { typeof(Guard), typeof(Fence), typeof(Sensor), typeof(Camera), typeof(BottomlessPit) };

        foreach (var obstacleType in obstaclePriority)
        {
            var obstacle = obstacles.FirstOrDefault(o => o.GetType() == obstacleType && !o.IsPassable(location));

            if (obstacle != null)
            {
                if (obstacle is Guard)
                {
                    return 'g';
                }
                else if (obstacle is Fence)
                {
                    return 'f';
                }
                else if (obstacle is Sensor)
                {
                    return 's';
                }
                else if (obstacle is BottomlessPit)
                {
                    return 'b';
                }
                else if (obstacle is Camera)
                {
                    return 'c';
                }
            }
        }

        return '.'; // Default character for safe squares
    }









    // Take two Loction objects as parameters to find a safe path between the two locations.
    public string FindSafePath(Location startLocation, Location endLocation)
    {
        // Initialise both open and closed lists
        List<Location> open = new List<Location>() { startLocation };
        List<Location> close = new List<Location>(); // Contains nodes that have been evaluated

        // Ensure every node has an entry in 'parents'
        SetParent(startLocation, null);

        SetGCost(startLocation, 0); // G cost set to 0 because it is the starting point.

        while (open.Any())
        {
            // Get lowest F cost location
            Location currentLocation = GetLowestFCost(open);

            // Switch current location from open to closed list
            open.Remove(currentLocation);
            close.Add(currentLocation);

            // If node has arrived at target location, based on X and Y, return path directions as a string.
            if (currentLocation.Equals(endLocation))
            {
                List<Location> pathLocations = GetPath(currentLocation);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < pathLocations.Count - 1; i++)
                {
                    Location current = pathLocations[i];
                    Location next = pathLocations[i + 1];

                    if (next.X > current.X) sb.Append("E");
                    else if (next.X < current.X) sb.Append("W");
                    else if (next.Y > current.Y) sb.Append("S");
                    else if (next.Y < current.Y) sb.Append("N");
                }

                return sb.ToString();
            }

            foreach (Location neighbour in GetNeighbours(currentLocation))
            {
                if (!IsLocationSafe(neighbour) || close.Contains(neighbour))
                {
                    continue;
                }

                int tentativeGCost = GetGCost(currentLocation) + 1;

                if (!open.Contains(neighbour))
                {
                    SetParent(neighbour, currentLocation);
                    SetGCost(neighbour, tentativeGCost);
                    SetHCost(neighbour, Math.Abs(neighbour.X - endLocation.X) + Math.Abs(neighbour.Y - endLocation.Y));
                    SetFCost(neighbour);

                    open.Add(neighbour);
                }
                else
                {
                    // Check if going through the current node we found a better path.
                    if (tentativeGCost < GetGCost(neighbour))
                    {
                        SetParent(neighbour, currentLocation);
                        SetGCost(neighbour, tentativeGCost);
                        SetFCost(neighbour);
                    }
                }
            }
        }

        return null; // No path found.
    }



    // Dictionaries used by A* algorithm to store cost values and parent-child relationships between nodes (locations).
    private Dictionary<Location, int> gCosts = new Dictionary<Location, int>();
    private Dictionary<Location, int> fCosts = new Dictionary<Location, int>();
    private Dictionary<Location, Location> parents = new Dictionary<Location, Location>();


    private Location GetLowestFCost(List<Location> locations)
    {
        // Assume that all locations in 'locations' are keys in 'fCosts'.
        return locations.OrderBy(location => fCosts.ContainsKey(location) ? fCosts[location] : Int32.MaxValue).First();
    }


    private List<Location> GetNeighbours(Location location)
    {
        // For simplicity's sake let's assume your map is infinite.
        List<Location> neighbours = new List<Location>
    {
        new Location(location.X + 1 , location.Y),
        new Location(location.X - 1 , location.Y),
        new Location(location.X ,     location.Y + 1),
        new Location(location.X ,     location.Y - 1)
    };

        return neighbours;
    }

    private void SetParent(Location childLocation, Location parentLocation)
    {
        if (parents.ContainsKey(childLocation))
        {
            parents[childLocation] = parentLocation;
        }
        else
        {
            parents.Add(childLocation, parentLocation);
        }
    }

    private int GetGCost(Location locaiton)
    {
        if (gCosts.ContainsKey(locaiton))
            return gCosts[locaiton];

        // If we don't have information about this locaiton yet,
        // let's assume it has infinite G cost.
        return Int32.MaxValue;
    }

    private void SetGCost(Location location, int g_cost)
    {
        if (gCosts.ContainsKey(location))
        {
            gCosts[location] = g_cost;
        }
        else
        {
            gCosts.Add(location, g_cost);
        }
    }


    private int GetHcost(Location neighbour, Location endLocation)
    {
        // For simplicity let's use Manhattan distance as our heuristic
        return Math.Abs(neighbour.X - endLocation.X) + Math.Abs(neighbour.Y - endLocation.Y);
    }


    private List<Location> GetPath(Location endLocation)
    {
        var path = new List<Location>();
        var currentLocation = endLocation;

        while (currentLocation != null && parents.ContainsKey(currentLocation))
        {
            path.Add(currentLocation);
            currentLocation = parents[currentLocation];
        }

        // The above gives us a path from goal to start; we want from start to goal.
        path.Reverse();

        return path;
    }


    // Add missing dictionaries and methods to your Intelligence class
    private Dictionary<Location, int> hCosts = new Dictionary<Location, int>();

    private void SetHCost(Location location, int h_cost)
    {
        if (hCosts.ContainsKey(location))
        {
            hCosts[location] = h_cost;
        }
        else
        {
            hCosts.Add(location, h_cost);
        }
    }

    private void SetFCost(Location location)
    {
        if (!gCosts.ContainsKey(location))
        {
            // Handle missing entries in gCosts.
            // This might involve setting a default value or throwing an exception.
            SetGCost(location, Int32.MaxValue);
        }

        if (!hCosts.ContainsKey(location))
        {
            // Handle missing entries in hCosts.
            // This might involve setting a default value or throwing an exception.
            SetHCost(location, Int32.MaxValue);
        }

        int newFCost = gCosts[location] + hCosts[location];

        if (fCosts.ContainsKey(location))
        {
            fCosts[location] = newFCost;
        }
        else
        {
            fCosts.Add(location, newFCost);
        }
    }



}
