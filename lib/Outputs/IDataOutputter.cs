using System.IO;
using System.Collections.Generic;

namespace DatagenSharp
{
	public interface IDataOutputter
	{
		(bool success, string possibleError) Init(object parameter, Stream outputStream);

		(bool success, string possibleError) WriteHeader(List<object> parameter);

		(bool success, string possibleError) WriteSingleEntry(List<object> parameter);

		(bool success, string possibleError) WriteFooter(List<object> parameter);

		#region Non essentials

		/// <summary>
		/// Get long and short names of this data outputter
		/// </summary>
		/// <returns>Valuetuple that constains long name and short name</returns>
		(string longName, string shortName) GetNames();

		#endregion // Non essentials
	}
}