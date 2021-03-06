﻿// OsmSharp - OpenStreetMap (OSM) SDK
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

using OsmSharp.Geo;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graphs.Geometric.Shapes
{
    /// <summary>
    /// An implementation of a shape based on a coordinate enumerable.
    /// </summary>
    public class ShapeEnumerable : ShapeBase
    {
        private readonly List<ICoordinate> _coordinates;
        private readonly bool _reversed;

        /// <summary>
        /// Creates a new shape based on a coordinate enumerable.
        /// </summary>
        public ShapeEnumerable(IEnumerable<ICoordinate> coordinates)
        {
            _coordinates = new List<ICoordinate>(coordinates);
            _reversed = false;
        }

        /// <summary>
        /// Creates a new shape based on a coordinate enumerable.
        /// </summary>
        public ShapeEnumerable(IEnumerable<ICoordinate> coordinates, bool reversed)
        {
            _coordinates = new List<ICoordinate>(coordinates);
            _reversed = reversed;
        }

        /// <summary>
        /// Returns the number of coordinates.
        /// </summary>
        public override int Count
        {
            get { return _coordinates.Count; }
        }

        /// <summary>
        /// Gets or sets the coordinate.
        /// </summary>
        public override ICoordinate this[int i]
        {
            get
            {
                if(_reversed)
                {
                    return _coordinates[_coordinates.Count - i - 1];
                }
                return _coordinates[i];
            }
        }

        /// <summary>
        /// Returns the same shape but with the order of the coordinates reversed.
        /// </summary>
        public override ShapeBase Reverse()
        {
            return new ShapeEnumerable(_coordinates, !_reversed);
        }
    }
}