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
            using (var stream = File.OpenRead(@"D:\work\data\OSM\planet\europe\belgium-latest.osm.pbf"))
            {
                var source = new OsmSharp.Osm.PBF.Streams.PBFOsmStreamSource(stream);
                var progress = new OsmSharp.Osm.Streams.Filters.OsmStreamFilterProgress();
                progress.RegisterSource(source);
                routerDb.LoadOsmData(progress, Vehicle.Car, Vehicle.Bicycle, Vehicle.Pedestrian);
            }

            routerDb.AddContracted(Vehicle.Car.Classifications());
            routerDb.AddContracted(Vehicle.Pedestrian.Fastest());
            routerDb.AddContracted(Vehicle.Bicycle.Fastest());
            routerDb.AddContracted(Vehicle.Bicycle.Networks());

            using (var stream = File.OpenWrite(@"belgium.a.ccpfbfbn.routing"))
            {
                routerDb.Serialize(stream);
            }

            var router = new Router(routerDb);
            
            // 50.779132,3.291435&loc=50.868270,3.197021
            var loc1 = new GeoCoordinate(50.869678F, 3.551331f);
            var loc2 = new GeoCoordinate(50.810057F, 3.388596f);
            var shortest = router.Calculate(Vehicle.Car.Fastest(), loc1, loc2);
            var shortestJson = shortest.ToGeoJson();
            var network = router.Calculate(Vehicle.Car.Classifications(), loc1, loc2);
            var networkJson = network.ToGeoJson();
        }
    }
}
