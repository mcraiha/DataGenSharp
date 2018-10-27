using System;

namespace DatagenSharp
{
	public interface IMutator
	{
		(bool success, string possibleError) Init(object parameter, int seed);

		(bool success, string possibleError, object result) Mutate(object input, object parameter = null, Type wantedOutputType = null);

		Type[] GetSupportedOutputs();
	}
}