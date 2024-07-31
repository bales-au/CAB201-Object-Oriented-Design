using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The abstract class Obstacle is a blueprint for all types of obstacles.
/// It contains a Location property and an abstract method IsPassable that must be implemented by all subclasses.
/// </summary>
abstract class Obstacle
{
    // The Location property stores the position of the obstacle on a grid.
    public Location Location { get; }

    // The constructor sets the location of the obsatcle when an instance is created.
    public Obstacle(Location location)
    {
        Location = location;
    }

    // Abstract method that needs to be implemented by each class.
    // IsPassable determines whether a given location is passable or not, based off obstacles.
    public abstract bool IsPassable(Location currentLocation);
}

// Guard is a specific type of obstacle. It inherits from the base class.
class Guard : Obstacle
{
    // Constructor class base constructor to set guard's location
    public Guard(Location location) : base(location) { }

    // Overrised IsPassable method set in the base class. A location is passable if it's not in the same row or column as guard's position.
    public override bool IsPassable(Location currentLocation)
    {
        // if the guard is not in the same column or not in the same row as the currentLocation, the location is passable
        return Location.X != currentLocation.X || Location.Y != currentLocation.Y; 
    }
}


// Fence represents another type of obstacle, defined between two points (StartPoint and EndPoint).
class Fence : Obstacle
{
    public Location StartPoint { get; }
    public Location EndPoint { get; }
    public Fence(Location startPoint, Location endPoint) : base(startPoint)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
    }

    public override bool IsPassable(Location currentLocation)
    {

        if (currentLocation.X == StartPoint.X && currentLocation.Y >= StartPoint.Y && currentLocation.Y <= EndPoint.Y
            || currentLocation.X == StartPoint.X && currentLocation.Y >= EndPoint.Y && currentLocation.Y <= StartPoint.Y
            || currentLocation.Y == StartPoint.Y && currentLocation.X >= StartPoint.X && currentLocation.X <= EndPoint.X
            || currentLocation.Y == StartPoint.Y && currentLocation.X >= EndPoint.X && currentLocation.X <= StartPoint.X)
        {
            return false;
        }

        return true;
    }
}

// Sensor represents another type of obstacle with specific range. Any point within this range isn't passable.
class Sensor : Obstacle
{
    public double Range { get; }

    public Sensor(Location location, double range) : base(location)
    {
        if (range <= 0)
        {
            throw new ArgumentException("Range must be greater than 0.");
        }

        Range = range;
    }

    public override bool IsPassable(Location currentLocation)
    {
        double distance = CalculateDistance(Location, currentLocation);
        return distance > Range;
    }
    
    // Private helper method to calculate Euclidean distance between two points using Pythagorean theorem.
    private double CalculateDistance(Location point1, Location point2)
    {
        double deltaX = point1.X - point2.X;
        double deltaY = point1.Y - point2.Y;

        // Using Euclidean distance formula
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
}


// Camera represents another type of obstacle that has vision in one direction (n,s,e,w).
class Camera : Obstacle
{
    public char Direction { get; }

    public Camera(Location location, char direction) : base(location)
    {
        if (!IsValidDirection(direction))
        {
            throw new ArgumentException("Invalid direction.");
        }

        Direction = direction;
    }

    public override bool IsPassable(Location currentLocation)
    {
        return  (Location.X != currentLocation.X || Location.Y != currentLocation.Y) && (!OutsideVision(currentLocation));
    }

    // Private helper method used to determine if certain locations are outside camera's vision based on its direction.
    private bool OutsideVision(Location targetLocation)
    {
        int dx = targetLocation.X - Location.X;
        int dy = targetLocation.Y - Location.Y;

        switch (Direction)
        {
            case 'n':
                return dy < 0 && Math.Abs(dx) <= Math.Abs(dy);
            case 's':
                return dy > 0 && Math.Abs(dx) <= Math.Abs(dy);
            case 'e':
                return dx > 0 && Math.Abs(dy) <= Math.Abs(dx); 
            case 'w':
                return dx < 0 && Math.Abs(dy) <= Math.Abs(dx); 
            default:
                throw new InvalidOperationException("Invalid direction.");
        }
    }

    // Private helper method used to validate direction input.
    private bool IsValidDirection(char direction)
    {
        return direction == 'n' || direction == 's' || direction == 'e' || direction == 'w';
    }
}


// Bottomless Pit is a specific type of obstacle. It inherits from the base class.
class BottomlessPit : Obstacle
{
    // Constructor class base constructor to set pit's location
    public BottomlessPit(Location location) : base(location) { }

    // Overrised IsPassable method set in the base class. A location is passable if it's not in a 3x3 'Bottomless Pit' grid, where currentLocation is the centre origin.
     public override bool IsPassable(Location currentLocation)
    {
        if (Location.X == currentLocation.X && Location.Y == currentLocation.Y
            || Location.X == currentLocation.X && Location.Y == currentLocation.Y + 1 // mid-bot
            || Location.X == currentLocation.X && Location.Y == currentLocation.Y - 1 // mid-top
            || Location.Y == currentLocation.Y && Location.X == currentLocation.X + 1 // right-mid
            || Location.Y == currentLocation.Y && Location.X == currentLocation.X - 1 // left-mid
            || Location.X == currentLocation.X + 1 && Location.Y == currentLocation.Y + 1 // right-bot
            || Location.X == currentLocation.X - 1 && Location.Y == currentLocation.Y - 1 // left-top
            || Location.Y == currentLocation.Y + 1 && Location.X == currentLocation.X - 1 // left-bot
            || Location.Y == currentLocation.Y - 1 && Location.X == currentLocation.X + 1) // right-top
        {
            return false;
        }
        return true;
    }
}