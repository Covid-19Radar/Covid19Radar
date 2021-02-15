using System.Net;

namespace Covid19Radar.Model
{
	public class ApiResponse<T>
	{
		public HttpStatusCode StatusCode { get; }
		public T              Result     { get; }

		public ApiResponse(HttpStatusCode statusCode, T result)
		{
			this.StatusCode = statusCode;
			this.Result     = result;
		}

		public ApiResponse(int statusCode, T result)
		{
			this.StatusCode = ((HttpStatusCode)(statusCode));
			this.Result     = result;
		}
	}
}
