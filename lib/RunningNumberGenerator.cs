using System;

namespace DatagenSharp
{
	public class RunningNumberGenerator : IDataGenerator
	{
		private static readonly Type[] supportedTypes = new Type[] { typeof(int), typeof(string) };

		private int currentValue = 0;

		public void Init(object parameter, int seed)
		{

		}

		public object Generate(object parameter = null, Type wantedOutput = null)
		{
			return this.currentValue;
		}

		public Type[] GetSupportedOutputs()
		{
			return supportedTypes;
		}

		public void NextStep()
		{
			currentValue++;
		}
	}
}