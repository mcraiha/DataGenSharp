using System;

namespace DatagenSharp
{
	public interface IDataGenerator
	{
		void Init(object parameter, int seed);

		object Generate(object parameter = null, Type wantedOutput = null);

		Type[] GetSupportedOutputs();

		void NextStep();
	}
}