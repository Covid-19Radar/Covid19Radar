namespace Covid19Radar.Common
{
	public static class JumpHash
	{
		private const ulong  constant  = 2862933555777941757L;
		private const double constant2 = 1L << 31;

		public static int JumpConsistentHash(object key, int buckets)
		{
			return JumpConsistentHash(key.GetHashCode(), buckets);
		}

		public static int JumpConsistentHash(ulong key, int buckets)
		{
			long b = -1, j = 0;
			while (j < buckets) {
				unchecked {
					b   = j;
					key = key * constant + 1;
					j   = ((long)((b + 1) * (constant2 / (key >> 33 + 1))));
				}
			}
			return unchecked((int)(b));
		}
	}
}
