using System;

namespace DatagenSharp
{
	public interface IMutator
	{
		void Init(string parameter, int seed);

		object Mutate(object input, string parameter = null, Type wantedOutputType = null);

		Type[] GetSupportedOutputs();
	}
}