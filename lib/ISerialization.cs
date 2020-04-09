using System;

namespace DatagenSharp
{
	/// <summary>
	/// Interface for every thing that needs (de)serialization
	/// </summary>
	public interface ISerialization
	{
		/// <summary>
		/// Load / deserialize from string
		/// </summary>
		/// <param name="parameter">String that will be loaded / deserialized</param>
		/// <returns>Valuetuple that tells if load / deserilization was success, and possible error message</returns>
		(bool success, string possibleError) Load(string parameter);

		/// <summary>
		/// Save / serialize to string
		/// </summary>
		/// <returns>String that contains serialization</returns>
		string Save();

		/// <summary>
		/// Get internal ID needed for serialization ordering
		/// </summary>
		/// <returns>Long, that has range of 0 .. long.max</returns>
		long GetInternalId();
	}
}