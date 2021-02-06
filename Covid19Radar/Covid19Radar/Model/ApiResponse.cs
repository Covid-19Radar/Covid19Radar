namespace Covid19Radar.Model
{
	public class ApiResponse<T>
	{
		public T   Result     { get; }
		public int StatusCode { get; }

		public ApiResponse(int statusCode = 0, T result = default)
		{
			this.Result     = result!;
			this.StatusCode = statusCode;
		}
	}
}
