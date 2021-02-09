using System.Net;

namespace Covid19Radar.Model
{
	public class ApiResponse<T>
	{
		public T              Result     { get; }
		public HttpStatusCode StatusCode { get; }

		public ApiResponse(HttpStatusCode statusCode, T result)
		{
			this.Result     = result;
			this.StatusCode = statusCode;
		}
	}
}
