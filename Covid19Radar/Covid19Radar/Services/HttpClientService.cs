﻿using System.Net.Http;

namespace Covid19Radar.Services
{
	public interface IHttpClientService
	{
		public HttpClient Create();
	}

	public class HttpClientService : IHttpClientService
	{
		public HttpClientService() { }

		public HttpClient Create()
		{
			return new HttpClient();
		}
	}
}
