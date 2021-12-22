using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;

namespace Urbiss.Domain.Models
{
    public class SurveyFile
    {
        public SurveyFile(string productName, string fileName, string outputFileName, SurveyFileTypeEnum fileType, SurveyProductEnum product, SurveyStorageTypeEnum storageType,
            ISurveyFileValidateAndProcess[] validate, ISurveyFileRequestData[] requestData)
        {
            ProductName = productName;
            FileName = fileName;
            OutputFileName = outputFileName;
            FileType = fileType;
            Product = product;
            StorageType = storageType;
            _validate = new List<ISurveyFileValidateAndProcess>(validate);
            _requestData = new List<ISurveyFileRequestData>(requestData);
        }

        public string ProductName { get; set; }
        public string FileName { get; }
        public string OutputFileName { get; }
        public SurveyFileTypeEnum FileType { get; }
        public SurveyProductEnum Product { get; }
        public SurveyStorageTypeEnum StorageType { get; }

        private List<ISurveyFileValidateAndProcess> _validate { get; }
        public void Validate(IServiceProvider serviceProvider, string fileName, Geometry area, SurveyConfigDto config)
        {
            _validate.ForEach(v => v.Validate(serviceProvider, fileName, area, config));
        }
        public void Process(IServiceProvider serviceProvider, Survey survey, string fileName, Geometry area, SurveyConfigDto config)
        {
            _validate.ForEach(v => v.Process(serviceProvider, survey, fileName, area, config));
        }

        public List<ISurveyFileRequestData> _requestData { get; }

        public void RequestData(IServiceProvider serviceProvider, Order order, string fileName, string outputFileName, string csvFileName, Geometry area, SurveyConfigDto config)
        {
            _requestData.ForEach(rd => rd.Process(serviceProvider, order, fileName, outputFileName, csvFileName, area, config));
        }
    }
}

