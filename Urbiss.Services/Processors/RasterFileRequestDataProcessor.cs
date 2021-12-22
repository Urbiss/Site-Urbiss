using MaxRev.Gdal.Core;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using System;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Services.Processors
{
    public class RasterFileRequestDataProcessor : ISurveyFileRequestData
    {
        //TODO: retirar os parâmetros desnecessários (que já estão dentro do objeto ordem)
        public void Process(IServiceProvider serviceProvider, Order order, string fileName, string outputFileName, string csvFileName, Geometry area, SurveyConfigDto config)
        {
            GdalBase.ConfigureAll();
            var cmdOptions = Gdal.ParseCommandLine($"-overwrite -of GTiff -cutline {csvFileName} -multi -wo NUM_THREADS=ALL_CPUS -crop_to_cutline {fileName} {outputFileName}");
            GDALWarpAppOptions options = new GDALWarpAppOptions(cmdOptions);
            using Dataset inputDataset = Gdal.Open(fileName, Access.GA_ReadOnly);
            using Dataset result = Gdal.Warp(outputFileName, new Dataset[] { inputDataset }, options, null, "CropTiff");
        }
    }
}
