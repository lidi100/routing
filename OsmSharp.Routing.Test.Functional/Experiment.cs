using OsmSharp.Math.Geo;
using OsmSharp.Routing.Osm;
using OsmSharp.Routing.Osm.Vehicles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharp.Routing.Test.Functional
{
    static class Experiment
    {
        public static void ExperimentHere()
        {
            var routerDb = new RouterDb();
            routerDb.LoadOsmData(File.OpenRead(@"D:\work\data\OSM\planet\europe\belgium-latest.osm.pbf"), Vehicle.Car, Vehicle.Bicycle);

            var router = new Router(routerDb);
            
            var loc1 = new GeoCoordinate(51.263875f, 4.785619f);
            var loc2 = new GeoCoordinate(51.310335f, 4.889088f);
            var shortest = router.Calculate(Vehicle.Bicycle.Shortest(), loc1, loc2);
            var shortestJson = shortest.ToGeoJson();
            var network = router.Calculate(Vehicle.Bicycle.Networks(), loc1, loc2);
            var networkJson = network.ToGeoJson();
        }
    }
}
