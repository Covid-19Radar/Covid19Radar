﻿using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Extensions;
using Covid19Radar.Api.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

[assembly: FunctionsStartup(typeof(Covid19Radar.Api.Startup))]

namespace Covid19Radar.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddCosmosClient();
            builder.Services.AddSingleton<ICryptionService, CryptionService>();
            builder.Services.AddSingleton<DataStore.ICosmos, DataStore.Cosmos>();
            builder.Services.AddSingleton<IValidationUserService, ValidationUserService>();
            builder.Services.AddSingleton<IAuthorizedAppRepository, ConfigAuthorizedAppRepository>();
            builder.Services.AddSingleton<IUserRepository, CosmosUserRepository>();
            builder.Services.AddSingleton<ISequenceRepository, CosmosSequenceRepository>();
            builder.Services.AddSingleton<IDiagnosisRepository, CosmosDiagnosisRepository>();
            builder.Services.AddSingleton<ITemporaryExposureKeyRepository, CosmosTemporaryExposureKeyRepository>();
            builder.Services.AddSingleton<ITemporaryExposureKeyExportRepository, CosmosTemporaryExposureKeyExportRepository>();
#if DEBUG
            builder.Services.AddSingleton<IDeviceValidationService, DeviceValidationDebugService>();
#else
            builder.Services.AddSingleton<IDeviceValidationService, DeviceValidationService>();
#endif
        }
    }
}
