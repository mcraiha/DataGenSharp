namespace DatagenSharp
{
	/// <summary>
	/// Common (de)serialization related stuff
	/// </summary>
	public static class CommonSerialization
	{
		/// <summary>
		/// Each object definition line should start with this character
		/// </summary>
		public static readonly char itemStartChar = '-';

		/// <summary>
		/// Delimiter for values of single object
		/// </summary>
		public static readonly char delimiter = '~';

		public static readonly string nullValue = "NULL";
	}
}