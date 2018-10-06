using System;

namespace DatagenSharp
{
	public interface IDataGenerator
	{
		(bool success, string possibleError) Init(object parameter, int seed);

		(bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null);

		Type[] GetSupportedOutputs();

		void NextStep();
	}
}