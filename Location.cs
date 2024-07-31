using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Grid
{
    public enum LocationState
    {
        Check,
        NotChecked,
        Obstructed
    }
    public enum ObstacleType
    {
        Undefined,
        Camera
    }
}
// Location class represents a point in 2D space with X and Y coordinates. Inherits from Grid class.
class Location : Grid
{
    public int X { get; }
    public int Y { get; }
    //public LocationState State { get; set; }
    //ObstacleType ObstacleType { get; set; }
    public Location(int x, int y)
    {
        X = x;
        Y = y;
        //State = LocationState.NotChecked;
        //ObstacleType = ObstacleType.Undefined;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Location l = (Location)obj;
        return (X == l.X) && (Y == l.Y);
    }

}


