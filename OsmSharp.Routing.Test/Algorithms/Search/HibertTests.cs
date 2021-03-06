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

using NUnit.Framework;
using OsmSharp.Math.Algorithms;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Algorithms.Search;
using OsmSharp.Routing.Graphs.Geometric;
using System.Collections.Generic;

namespace OsmSharp.Routing.Test.Algorithms.Search
{
    /// <summary>
    /// Contains tests for the hilbert sort/search algorithms.
    /// </summary>
    [TestFixture]
    class HibertTests
    {
        /// <summary>
        /// Tests the sort hilbert function with order #4.
        /// </summary>
        [Test]
        public void SortHilbertTestSteps4()
        {
            var n = 4;

            // build locations.
            var locations = new List<GeoCoordinate>();
            locations.Add(new GeoCoordinate(-90, -180));
            locations.Add(new GeoCoordinate(-90, -60));
            locations.Add(new GeoCoordinate(-90, 60));
            locations.Add(new GeoCoordinate(-90, 180));
            locations.Add(new GeoCoordinate(-30, -180));
            locations.Add(new GeoCoordinate(-30, -60));
            locations.Add(new GeoCoordinate(-30, 60));
            locations.Add(new GeoCoordinate(-30, 180));
            locations.Add(new GeoCoordinate(30, -180));
            locations.Add(new GeoCoordinate(30, -60));
            locations.Add(new GeoCoordinate(30, 60));
            locations.Add(new GeoCoordinate(30, 180));
            locations.Add(new GeoCoordinate(90, -180));
            locations.Add(new GeoCoordinate(90, -60));
            locations.Add(new GeoCoordinate(90, 60));
            locations.Add(new GeoCoordinate(90, 180));

            // build graph.
            var graph = new GeometricGraph(1);
            for (var vertex = 0; vertex < locations.Count; vertex++)
            {
                graph.AddVertex((uint)vertex, (float)locations[vertex].Latitude,
                    (float)locations[vertex].Longitude);
            }

            // build a sorted version in-place.
            graph.Sort(n);

            // test if sorted.
            for (uint vertex = 1; vertex < graph.VertexCount - 1; vertex++)
            {
                Assert.IsTrue(
                    graph.Distance(n, vertex) <=
                    graph.Distance(n, vertex + 1));
            }

            // sort locations.
            locations.Sort((x, y) =>
            {
                return HilbertCurve.HilbertDistance((float)x.Latitude, (float)x.Longitude, n).CompareTo(
                     HilbertCurve.HilbertDistance((float)y.Latitude, (float)y.Longitude, n));
            });

            // confirm sort.
            for (uint vertex = 0; vertex < graph.VertexCount; vertex++)
            {
                float latitude, longitude;
                graph.GetVertex(vertex, out latitude, out longitude);
                Assert.AreEqual(latitude, locations[(int)vertex].Latitude);
                Assert.AreEqual(longitude, locations[(int)vertex].Longitude);
            }
        }

        /// <summary>
        /// Tests searching the closest vertex.
        /// </summary>
        [Test]
        public void SearchClosestVertexTest()
        {
            var graph = new GeometricGraph(1);
            graph.AddVertex(0, 1, 1);
            graph.AddVertex(1, 2, 2);

            graph.Sort();

            Assert.AreEqual(0, graph.SearchClosest(1, 1, 1, 1));
            Assert.AreEqual(1, graph.SearchClosest(2, 2, 1, 1));
            Assert.AreEqual(Constants.NO_VERTEX, graph.SearchClosest(3, 3, .5f, .5f));
        }

        /// <summary>
        /// Tests searching the vertices.
        /// </summary>
        [Test]
        public void SearchVerticesTest()
        {
            var graph = new GeometricGraph(1);
            graph.AddVertex(0, .00f, .00f);
            graph.AddVertex(1, .02f, .00f);
            graph.AddVertex(2, .04f, .00f);
            graph.AddVertex(3, .06f, .00f);
            graph.AddVertex(4, .08f, .00f);
            graph.AddVertex(5, .00f, .02f);
            graph.AddVertex(6, .02f, .02f);
            graph.AddVertex(7, .04f, .02f);
            graph.AddVertex(8, .06f, .02f);
            graph.AddVertex(9, .08f, .02f);
            graph.AddVertex(10, .00f, .04f);
            graph.AddVertex(11, .02f, .04f);
            graph.AddVertex(12, .04f, .04f);
            graph.AddVertex(13, .06f, .04f);
            graph.AddVertex(14, .08f, .04f);
            graph.AddVertex(15, .00f, .06f);
            graph.AddVertex(16, .02f, .06f);
            graph.AddVertex(17, .04f, .06f);
            graph.AddVertex(18, .06f, .06f);
            graph.AddVertex(19, .08f, .06f);
            graph.AddVertex(20, .00f, .08f);
            graph.AddVertex(21, .02f, .08f);
            graph.AddVertex(22, .04f, .08f);
            graph.AddVertex(23, .06f, .08f);
            graph.AddVertex(24, .08f, .08f);

            graph.Sort();

            // test 0.009, 0.009, 0.019, 0.019
            var vertices = graph.Search(0.009f, 0.009f, 0.029f, 0.029f);
            Assert.AreEqual(1, vertices.Count);
            Assert.IsTrue(vertices.Contains(3));
            var location = graph.GetVertex(3);
            Assert.AreEqual(.02f, location.Latitude);
            Assert.AreEqual(.02f, location.Longitude);

            // test 0.009, 0.009, 0.099, 0.019
            vertices = graph.Search(0.009f, 0.009f, 0.099f, 0.029f);
            Assert.AreEqual(4, vertices.Count);
            Assert.IsTrue(vertices.Contains(3));
            location = graph.GetVertex(3);
            Assert.AreEqual(.02f, location.Latitude);
            Assert.AreEqual(.02f, location.Longitude);
            Assert.IsTrue(vertices.Contains(13));
            location = graph.GetVertex(13);
            Assert.AreEqual(.04f, location.Latitude);
            Assert.AreEqual(.02f, location.Longitude);
            Assert.IsTrue(vertices.Contains(16));
            location = graph.GetVertex(16);
            Assert.AreEqual(.06f, location.Latitude);
            Assert.AreEqual(.02f, location.Longitude);
            Assert.IsTrue(vertices.Contains(19));
            location = graph.GetVertex(19);
            Assert.AreEqual(.08f, location.Latitude);
            Assert.AreEqual(.02f, location.Longitude);

            // test -0.001, -0.001, 0.09, 0.09
            vertices = graph.Search(-0.001f, -0.001f, 0.09f, 0.09f);
            Assert.AreEqual(25, vertices.Count);
            Assert.IsTrue(vertices.Contains(0));
            location = graph.GetVertex(0);
            Assert.AreEqual(.0f, location.Latitude);
            Assert.AreEqual(.0f, location.Longitude);
            Assert.IsTrue(vertices.Contains(1));
            location = graph.GetVertex(1);
            Assert.AreEqual(.0f, location.Latitude);
            Assert.AreEqual(.02f, location.Longitude);
            Assert.IsTrue(vertices.Contains(2));
            location = graph.GetVertex(2);
            Assert.AreEqual(.02f, location.Latitude);
            Assert.AreEqual(.0f, location.Longitude);
            Assert.IsTrue(vertices.Contains(3));
            location = graph.GetVertex(3);
            Assert.AreEqual(.02f, location.Latitude);
            Assert.AreEqual(.02f, location.Longitude);
            Assert.IsTrue(vertices.Contains(4));
            location = graph.GetVertex(4);
            Assert.AreEqual(.02f, location.Latitude);
            Assert.AreEqual(.04f, location.Longitude);
            Assert.IsTrue(vertices.Contains(5));
            location = graph.GetVertex(5);
            Assert.AreEqual(.00f, location.Latitude);
            Assert.AreEqual(.04f, location.Longitude);
            Assert.IsTrue(vertices.Contains(6));
            location = graph.GetVertex(6);
            Assert.AreEqual(.00f, location.Latitude);
            Assert.AreEqual(.06f, location.Longitude);
            Assert.IsTrue(vertices.Contains(7));
            location = graph.GetVertex(7);
            Assert.AreEqual(.00f, location.Latitude);
            Assert.AreEqual(.08f, location.Longitude);
            Assert.IsTrue(vertices.Contains(8));
            location = graph.GetVertex(8);
            Assert.AreEqual(.02f, location.Latitude);
            Assert.AreEqual(.08f, location.Longitude);
            Assert.IsTrue(vertices.Contains(9));
            location = graph.GetVertex(9);
            Assert.AreEqual(.02f, location.Latitude);
            Assert.AreEqual(.06f, location.Longitude);
            Assert.IsTrue(vertices.Contains(10));
            location = graph.GetVertex(10);
            Assert.AreEqual(.04f, location.Latitude);
            Assert.AreEqual(.08f, location.Longitude);
            Assert.IsTrue(vertices.Contains(11));
            location = graph.GetVertex(11);
            Assert.AreEqual(.04f, location.Latitude);
            Assert.AreEqual(.06f, location.Longitude);
            Assert.IsTrue(vertices.Contains(12));
            location = graph.GetVertex(12);
            Assert.AreEqual(.04f, location.Latitude);
            Assert.AreEqual(.04f, location.Longitude);
            Assert.IsTrue(vertices.Contains(13));
            location = graph.GetVertex(13);
            Assert.AreEqual(.04f, location.Latitude);
            Assert.AreEqual(.02f, location.Longitude);
            Assert.IsTrue(vertices.Contains(14));
            location = graph.GetVertex(14);
            Assert.AreEqual(.04f, location.Latitude);
            Assert.AreEqual(.00f, location.Longitude);
            Assert.IsTrue(vertices.Contains(15));
            location = graph.GetVertex(15);
            Assert.AreEqual(.06f, location.Latitude);
            Assert.AreEqual(.04f, location.Longitude);
            Assert.IsTrue(vertices.Contains(16));
            location = graph.GetVertex(16);
            Assert.AreEqual(.06f, location.Latitude);
            Assert.AreEqual(.02f, location.Longitude);
            Assert.IsTrue(vertices.Contains(17));
            location = graph.GetVertex(17);
            Assert.AreEqual(.06f, location.Latitude);
            Assert.AreEqual(.00f, location.Longitude);
            Assert.IsTrue(vertices.Contains(18));
            location = graph.GetVertex(18);
            Assert.AreEqual(.08f, location.Latitude);
            Assert.AreEqual(.00f, location.Longitude);
            Assert.IsTrue(vertices.Contains(19));
            location = graph.GetVertex(19);
            Assert.AreEqual(.08f, location.Latitude);
            Assert.AreEqual(.02f, location.Longitude);
            Assert.IsTrue(vertices.Contains(20));
            location = graph.GetVertex(20);
            Assert.AreEqual(.08f, location.Latitude);
            Assert.AreEqual(.04f, location.Longitude);
            Assert.IsTrue(vertices.Contains(21));
            location = graph.GetVertex(21);
            Assert.AreEqual(.08f, location.Latitude);
            Assert.AreEqual(.06f, location.Longitude);
            Assert.IsTrue(vertices.Contains(22));
            location = graph.GetVertex(22);
            Assert.AreEqual(.08f, location.Latitude);
            Assert.AreEqual(.08f, location.Longitude);
            Assert.IsTrue(vertices.Contains(23));
            location = graph.GetVertex(23);
            Assert.AreEqual(.06f, location.Latitude);
            Assert.AreEqual(.08f, location.Longitude);
            Assert.IsTrue(vertices.Contains(24));
            location = graph.GetVertex(24);
            Assert.AreEqual(.06f, location.Latitude);
            Assert.AreEqual(.06f, location.Longitude);
        }
    }
}
