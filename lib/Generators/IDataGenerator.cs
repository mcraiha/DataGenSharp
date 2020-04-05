using System;

namespace DatagenSharp
{
	public interface IDataGenerator
	{
		/// <summary>
		/// Init DataGenerator
		/// </summary>
		/// <param name="parameter">Parameter used for initing this data generator</param>
		/// <param name="seed">Seed for random number generation if needeed</param>
		/// <returns>Valuetuple that tells if init was success, and possible error</returns>
		(bool success, string possibleError) Init(object parameter, int seed);

		/// <summary>
		/// Generate single item
		/// </summary>
		/// <param name="parameter">Optional parameter for generation</param>
		/// <param name="wantedOutputType">Optional output type parameter</param>
		/// <returns>Valuetuple that tells if init was success, possible error and output result</returns>
		(bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null);

		/// <summary>
		/// Get output Types that this genereator supports 
		/// </summary>
		/// <returns>Array of Types</returns>
		Type[] GetSupportedOutputs();

		/// <summary>
		/// Inform generator that one entry is completed
		/// </summary>
		void NextStep();

		#region Non essentials

		/// <summary>
		/// Get long and short names of this generator
		/// </summary>
		/// <returns>Valuetuple that constains long name and short name</returns>
		(string longName, string shortName) GetNames();

		#endregion // Non essentials
	}
}