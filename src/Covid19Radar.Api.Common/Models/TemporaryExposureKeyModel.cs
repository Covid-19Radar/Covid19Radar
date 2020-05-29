﻿using Covid19Radar.Api.Common;
using Covid19Radar.Api.Protobuf;
using Google.Protobuf;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Api.Models
{
    public class TemporaryExposureKeyModel
    {
        public string id { get; set; } = Guid.NewGuid().ToString("N");
        public string PartitionKey { get; set; }
        public byte[] KeyData { get; set; }
        public int RollingPeriod { get; set; }
        public int RollingStartIntervalNumber { get; set; }
        public int TransmissionRiskLevel { get; set; }
        public long GetRollingStartUnixTimeSeconds() => RollingStartIntervalNumber * 10 * 60;
        public long GetRollingPeriodSeconds() => RollingPeriod * 10 * 60;

        public ulong Timestamp { get; set; }
        public string ExportId { get; set; }

        public TemporaryExposureKey ToKey()
        {
            return new TemporaryExposureKey()
            {
                KeyData = ByteString.CopyFrom(KeyData),
                RollingStartIntervalNumber = RollingStartIntervalNumber,
                RollingPeriod = RollingPeriod,
                TransmissionRiskLevel = TransmissionRiskLevel
            };
        }
    }

}
