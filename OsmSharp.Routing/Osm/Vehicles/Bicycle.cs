﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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

using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Profiles;
using OsmSharp.Units.Speed;

namespace OsmSharp.Routing.Osm.Vehicles
{

    /// <summary>
    /// Represents a bicycle
    /// </summary>
    public class Bicycle : Vehicle
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Bicycle()
        {
            AccessibleTags.Add("steps", string.Empty); // only when there is a ramp.
            AccessibleTags.Add("service", string.Empty);
            AccessibleTags.Add("cycleway", string.Empty);
            AccessibleTags.Add("path", string.Empty);
            AccessibleTags.Add("road", string.Empty);
            AccessibleTags.Add("track", string.Empty);
            AccessibleTags.Add("living_street", string.Empty);
            AccessibleTags.Add("residential", string.Empty);
            AccessibleTags.Add("unclassified", string.Empty);
            AccessibleTags.Add("secondary", string.Empty);
            AccessibleTags.Add("secondary_link", string.Empty);
            AccessibleTags.Add("primary", string.Empty);
            AccessibleTags.Add("primary_link", string.Empty);
            AccessibleTags.Add("tertiary", string.Empty);
            AccessibleTags.Add("tertiary_link", string.Empty);

            VehicleTypes.Add("vehicle"); // a bicycle is a generic vehicle.
            VehicleTypes.Add("bicycle");
        }

        /// <summary>
        /// Returns true if the vehicle is allowed on the way represented by these tags
        /// </summary>
        protected override bool IsVehicleAllowed(TagsCollectionBase tags, string highwayType)
        {
            if (!tags.InterpretAccessValues(VehicleTypes, "access"))
            {
                return false;
            }

            // do the designated tags.
            if (tags.ContainsKey("bicycle"))
            {
                if (tags["bicycle"] == "designated")
                {
                    return true; // designated bicycle
                }
                if (tags["bicycle"] == "yes")
                {
                    return true; // yes for bicycle
                }
                if (tags["bicycle"] == "no")
                {
                    return false; //  no for bicycle
                }
            }
            if (highwayType == "steps")
            {
                if(tags.ContainsKeyValue("ramp", "yes"))
                {
                    return true;
                }
                return false;
            }
            return AccessibleTags.ContainsKey(highwayType);
        }

        /// <summary>
        /// Returns the Max Speed for the highwaytype in Km/h.
        /// 
        /// This does not take into account how fast this vehicle can go just the max possible speed.
        /// </summary>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        public override KilometerPerHour MaxSpeedAllowed(string highwayType)
        {
            switch (highwayType)
            {
                case "services":
                case "proposed":
                case "cycleway":
                case "pedestrian":
                case "steps":
                case "path":
                case "footway":
                case "living_street":
                    return this.MaxSpeed();
                case "track":
                case "road":
                    return 30;
                case "residential":
                case "unclassified":
                    return 50;
                case "motorway":
                case "motorway_link":
                    return 120;
                case "trunk":
                case "trunk_link":
                case "primary":
                case "primary_link":
                    return 90;
                default:
                    return 70;
            }
        }

        /// <summary>
        /// Returns true if the given key is relevant.
        /// </summary>
        public override bool IsRelevant(string key)
        {
            if (base.IsRelevant(key))
            {
                return true;
            }

            if (key.StartsWith("cyclenetwork"))
            { // also make sure to include all cyclenetwork tags!
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given key is valid for profile.
        /// </summary>
        public override bool IsRelevantForProfile(string key)
        {
            if (base.IsRelevantForProfile(key))
            {
                return true;
            }

            if (key.StartsWith("cyclenetwork"))
            { // also make sure to include all cyclenetwork tags!
                return true;
            }
            return key == "ramp";
        }

        /// <summary>
        /// Returns true if the edge is one way forward, false if backward, null if bidirectional.
        /// </summary>
        public override bool? IsOneWay(TagsCollectionBase tags)
        {
            string oneway;
            if (tags.TryGetValue("oneway:bicycle", out oneway))
            {
                if (oneway == "yes")
                {
                    return true;
                }
                else if (oneway == "no")
                {
                    return null;
                }
                return false;
            }

            if (tags.TryGetValue("oneway", out oneway))
            {
                if (oneway == "yes")
                {
                    return true;
                }
                else if (oneway == "no")
                {
                    return null;
                }
                return false;
            }

            string junction;
            if (tags.TryGetValue("junction", out junction))
            {
                if (junction == "roundabout")
                {
                    return true;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the maximum possible speed this vehicle can achieve.
        /// </summary>
        /// <returns></returns>
        public override KilometerPerHour MaxSpeed()
        {
            return 15;
        }

        /// <summary>
        /// Returns the minimum speed.
        /// </summary>
        /// <returns></returns>
        public override KilometerPerHour MinSpeed()
        {
            return 3;
        }

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "Bicycle"; }
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
                this.Balanced(),
                this.Networks()
            };
        }

        /// <summary>
        /// Returns a profile specifically for bicycle that tries to balance between bicycle infrastructure and fastest route.
        /// </summary>
        /// <returns></returns>
        public Routing.Profiles.Profile Balanced()
        {
            return new Profiles.BicycleBalanced(this);
        }

        /// <summary>
        /// Returns a profile specifically for bicycles that uses cycling networks as much as possible. Behaves as balanced in the absences of cycling network data.
        /// </summary>
        /// <returns></returns>
        public Routing.Profiles.Profile Networks()
        {
            return new Profiles.BicycleNetworks(this);
        }
    }
}