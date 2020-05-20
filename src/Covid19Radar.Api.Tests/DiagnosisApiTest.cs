﻿using Covid19Radar.DataAccess;
using Covid19Radar.Models;
using Covid19Radar.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests
{
    [TestClass]
    [TestCategory("Api")]
    public class DiagnosisApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            var validation = new Mock<IValidationUserService>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.DiagnosisApi>();
            var diagnosisApi = new Covid19Radar.Api.DiagnosisApi(diagnosisRepo.Object, validation.Object, logger);
        }

        [DataTestMethod]
        [DataRow(true, "xxxxx", "UserUuid")]
        [DataRow(false, "xxxxx", "UserUuid")]
        public async Task RunAsyncMethod(bool isValid, string submissionNumber, string userUuid)
        {
            // preparation
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            diagnosisRepo.Setup(_ => _.SubmitDiagnosisAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TemporaryExposureKeyModel[]>()))
                .Returns(Task.CompletedTask);
            var validation = new Mock<IValidationUserService>();
            var validationResult = new IValidationUserService.ValidateResult()
            {
                IsValid = isValid
            };
            validation.Setup(_ => _.ValidateAsync(It.IsAny<HttpRequest>(), It.IsAny<IUser>())).ReturnsAsync(validationResult);
            var logger = new Mock.LoggerMock<Covid19Radar.Api.DiagnosisApi>();
            var diagnosisApi = new Covid19Radar.Api.DiagnosisApi(diagnosisRepo.Object, validation.Object, logger);
            var context = new Mock.HttpContextMock();
            var bodyJson = new DiagnosisSubmissionParameter()
            {
                SubmissionNumber = submissionNumber,
                UserUuid = userUuid,
                Keys = new DiagnosisSubmissionParameter.Key[] {
                    new DiagnosisSubmissionParameter.Key() { KeyData = "", RollingPeriod = 1, RollingStartNumber = 1 } }
            };
            var bodyString = Newtonsoft.Json.JsonConvert.SerializeObject(bodyJson);
            using var stream = new System.IO.MemoryStream();
            using (var writer = new System.IO.StreamWriter(stream, leaveOpen: true))
            {
                await writer.WriteAsync(bodyString);
                await writer.FlushAsync();
            }
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            context._Request.Body = stream;
            // action
            await diagnosisApi.RunAsync(context.Request);
            // assert
        }
    }
}
