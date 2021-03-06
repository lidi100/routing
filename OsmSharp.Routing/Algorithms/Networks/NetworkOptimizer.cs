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

using OsmSharp.Geo;
using OsmSharp.Routing.Data;
using OsmSharp.Routing.Network;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Networks
{
    /// <summary>
    /// An algorithm that optimizes a network by removing obsolete vertices.
    /// </summary>
    public class NetworkOptimizer : AlgorithmBase
    {
        private readonly Network.RoutingNetwork _network;
        private readonly MergeDelegate _merge;

        /// <summary>
        /// A delegate to control the merging of two edges.
        /// </summary>
        public delegate bool MergeDelegate(Network.Data.EdgeData edgeData1, bool inverted1,
            Network.Data.EdgeData edgeData2, bool inverted2, out Network.Data.EdgeData mergedEdgeData, out bool mergedInverted);

        /// <summary>
        /// Creates a new network optimizer algorithm.
        /// </summary>
        public NetworkOptimizer(Network.RoutingNetwork network, MergeDelegate merge)
        {
            _network = network;
            _merge = merge;
        }

        /// <summary>
        /// Executes the actual algorithm.
        /// </summary>
        protected override void DoRun()
        {
            var edges = new List<Network.RoutingEdge>();
            for (uint vertex = 0; vertex < _network.VertexCount; vertex++)
            {
                edges.Clear();
                edges.AddRange(_network.GetEdgeEnumerator(vertex));
                if (edges.Count == 2)
                {
                    bool inverted;
                    Network.Data.EdgeData edgeData;
                    if (edges[0].To != edges[1].To &&
                        _merge(edges[0].Data, !edges[0].DataInverted, edges[1].Data, edges[1].DataInverted,
                            out edgeData, out inverted))
                    { // targets can be merged.
                        if (edgeData.Distance < _network.MaxEdgeDistance &&
                            !_network.ContainsEdge(edges[0].To, edges[1].To))
                        { // network does not contain edge yet and new edge < MAX_DISTANCE.
                            var shape = new List<ICoordinate>();
                            var shape1 = edges[0].Shape;
                            if (shape1 != null)
                            { // add coordinates of first shape.
                                if (!edges[0].DataInverted)
                                { // data is not inverted.
                                    shape.AddRange(shape1.Reverse());
                                }
                                else
                                { // data is inverted.
                                    shape.AddRange(shape1);
                                }
                            }
                            shape.Add(_network.GetVertex(vertex));
                            var shape2 = edges[1].Shape;
                            if (shape2 != null)
                            { // add coordinates of first shape.
                                if (!edges[1].DataInverted)
                                { // data is not inverted.
                                    shape.AddRange(shape2);
                                }
                                else
                                { // data is inverted.
                                    shape.AddRange(shape2.Reverse());
                                }
                            }

                            // remove edges.
                            _network.RemoveEdges(vertex);

                            // add edges.
                            if (!inverted)
                            { // just add 0->1
                                _network.AddEdge(edges[0].To, edges[1].To, edgeData, shape);
                            }
                            else
                            { // add the reverse 1->0
                                shape.Reverse();
                                _network.AddEdge(edges[1].To, edges[0].To, edgeData, shape);
                            }
                        }
                    }
                }
            }
        }
    }
}