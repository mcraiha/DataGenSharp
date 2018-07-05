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
	}
}