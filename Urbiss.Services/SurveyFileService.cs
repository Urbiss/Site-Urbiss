using System.Collections.Generic;
using Urbiss.Domain.Constants;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;
using Urbiss.Services.Processors;

namespace Urbiss.Services
{
    public class SurveyFileService : ISurveyFileService
    {
        public SurveyFileService()
        {
        }

        private IEnumerable<SurveyFile> _files = null;

        public IEnumerable<SurveyFile> GetFiles() => _files ??= new List<SurveyFile>(new SurveyFile[] 
            {
                new SurveyFile(SurveyConsts.DTM_NAME, SurveyConsts.DTM_FILE, SurveyConsts.DTM_OUTPUT_FILE, SurveyFileTypeEnum.Raster, SurveyProductEnum.Dtm, SurveyStorageTypeEnum.File,
                    new RasterFileValidateProcessor[] { new RasterFileValidateProcessor() }, 
                    new RasterFileRequestDataProcessor[] { new RasterFileRequestDataProcessor() }),
                new SurveyFile(SurveyConsts.DSM_NAME, SurveyConsts.DSM_FILE, SurveyConsts.DSM_OUTPUT_FILE, SurveyFileTypeEnum.Raster, SurveyProductEnum.Dsm, SurveyStorageTypeEnum.File,
                    new RasterFileValidateProcessor[] { new RasterFileValidateProcessor() },
                    new RasterFileRequestDataProcessor[] { new RasterFileRequestDataProcessor() }),
                new SurveyFile(SurveyConsts.ORTHOPHOTO_NAME, SurveyConsts.ORTHOPHOTO_FILE, SurveyConsts.ORTHOPHOTO_OUTPUT_FILE, SurveyFileTypeEnum.Raster, SurveyProductEnum.OrthoPhoto, SurveyStorageTypeEnum.File,
                    new RasterFileValidateProcessor[] { new RasterFileValidateProcessor() },
                    new RasterFileRequestDataProcessor[] { new RasterFileRequestDataProcessor() }),
                new SurveyFile(SurveyConsts.CONTOURS_NAME, SurveyConsts.CONTOURS_FILE, SurveyConsts.CONTOURS_OUTPUT_FILE, SurveyFileTypeEnum.Vector, SurveyProductEnum.Contours, SurveyStorageTypeEnum.File,
                    new ShapeFileValidateProcessor[] { new ShapeFileValidateProcessor() },
                    new DxfVectorFileRequestDataProcessor[] { new DxfVectorFileRequestDataProcessor() }),
                new SurveyFile(SurveyConsts.AERIAL_PHOTO_NAME, SurveyConsts.AERIAL_PHOTO_FOLDER, SurveyConsts.AERIAL_PHOTO_OUTPUT_FILE, SurveyFileTypeEnum.Photo, SurveyProductEnum.AerialPhoto, SurveyStorageTypeEnum.Folder,
                    new AerialPhotoValidateProcessor[] { new AerialPhotoValidateProcessor() },
                    new AerialPhotoRequestDataProcessor[] { new AerialPhotoRequestDataProcessor() }),
                new SurveyFile(SurveyConsts.URBAN_ANALYSIS_NAME, SurveyConsts.URBAN_ANALYSIS_FILE, SurveyConsts.URBAN_ANALYSIS_OUTPUT_FILE, SurveyFileTypeEnum.SingleFile, SurveyProductEnum.UrbanAnalysis, SurveyStorageTypeEnum.File,
                    new ISurveyFileValidateAndProcess[] {  },
                    new SingleFileRequestDataProcessor[] { new SingleFileRequestDataProcessor() })
            });
    }
}
