// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Routing.Profiles;

namespace OsmSharp.Routing.Osm.Vehicles
{
    /// <summary>
    /// Represents a Car
    /// </summary>
    public class Car : MotorVehicle
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Car()
        {
            VehicleTypes.Add("motorcar");
        }

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "Car"; }
        }

        /// <summary>
        /// Gets all profiles for this vehicle.
        /// </summary>
        /// <returns></returns>
        public override Profile[] GetProfiles()
        {
            return new Profile[]
            {
                this.Fastest(),
                this.Shortest(),
                this.Classifications()
            };
        }

        /// <summary>
        /// Returns a profile specifically for cars that tries to follow road classifications as strict as possible-.
        /// </summary>
        /// <returns></returns>
        public Routing.Profiles.Profile Classifications()
        {
            return new Profiles.CarClassifications(this);
        }

    }
}
