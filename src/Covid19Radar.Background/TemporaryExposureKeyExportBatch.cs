using Covid19Radar.Background.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Covid19Radar.Background
{
    public class TemporaryExposureKeyExportBatch
    {
        public readonly ITemporaryExposureKeyExportBatchService BatchService;

        public TemporaryExposureKeyExportBatch(ITemporaryExposureKeyExportBatchService batchService)
        {
            BatchService = batchService;
        }

        /// <summary>
        /// Four times a day, 0, 6, 12, 18 o'clock.
        /// </summary>
        /// <param name="myTimer">timer information</param>
        /// <param name="log">logger</param>
        /// <returns></returns>
        [FunctionName("TemporaryExposureKeyExportBatch")]
        public async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer, ILogger log)
        //public async Task Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{nameof(TemporaryExposureKeyExportBatch)} Timer trigger function executed at: {DateTime.Now}");
            await BatchService.RunAsync();
        }
    }
}
