using System;

namespace DatagenSharp
{
	public class RunningNumberGenerator : IDataGenerator
	{
		public static readonly string LongName = "RunningNumberGenerator";

		public static readonly string ShortName = "RUN";

		private static readonly Type[] supportedTypes = new Type[] { typeof(int), typeof(string) };

		private int currentValue = 0;

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			// TODO: add parameter support
			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null)
		{
			return (success: true, possibleError: "", result: this.currentValue);
		}

		public Type[] GetSupportedOutputs()
		{
			return supportedTypes;
		}

		public void NextStep()
		{
			currentValue++;
		}

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}
	}
}