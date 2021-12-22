using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Interfaces;

namespace Urbiss.Services.Helpers
{
    public class TransformGeometryHelper
    {
        public static Geometry ProjectGeometry(Geometry geom, string wktSrc, string wktTrg)
        {
            var sourceCoordSystem = new CoordinateSystemFactory().CreateFromWkt(wktSrc);
            var targetCoordSystem = new CoordinateSystemFactory().CreateFromWkt(wktTrg);
            var trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(sourceCoordSystem, targetCoordSystem);
            var projGeom = Transform(geom, trans.MathTransform);
            projGeom.SRID = Convert.ToInt32(targetCoordSystem.AuthorityCode);
            return projGeom;
        }

        private static Geometry Transform(Geometry geom, MathTransform transform)
        {
            geom = geom.Copy();
            geom.Apply(new MTF2d(transform));
            return geom;
        }

        sealed class MTF2d : ICoordinateSequenceFilter
        {
            private readonly MathTransform _mathTransform;

            public MTF2d(MathTransform mathTransform) => _mathTransform = mathTransform;

            public bool Done => false;
            public bool GeometryChanged => true;
            public void Filter(CoordinateSequence seq, int i)
            {
                double x = seq.GetX(i);
                double y = seq.GetY(i);
                _mathTransform.Transform(ref x, ref y);
                seq.SetX(i, x);
                seq.SetY(i, y);
            }
        }
    }
}
