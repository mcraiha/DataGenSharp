using System;

namespace DatagenSharp
{
	public interface IMutator
	{
		/// <summary>
		/// Init Mutator
		/// </summary>
		/// <param name="parameter">Parameter used for initing this mutator</param>
		/// <param name="seed">Seed for random number generation if needeed</param>
		/// <returns>Valuetuple that tells if init was success, and possible error</returns>
		(bool success, string possibleError) Init(object parameter, int seed);

		/// <summary>
		/// Mutate single item
		/// </summary>
		/// <param name="input">Input that will be mutated</param>
		/// <param name="parameter">Optional parameter for mutation</param>
		/// <param name="wantedOutputType">Optional output type parameter</param>
		/// <returns>Valuetuple that tells if init was success, possible error and output result</returns>
		(bool success, string possibleError, object result) Mutate(object input, object parameter = null, Type wantedOutputType = null);

		/// <summary>
		/// Get output Types that this mutator supports 
		/// </summary>
		/// <returns>Array of Types</returns>
		Type[] GetSupportedOutputs();

		#region Non essentials

		/// <summary>
		/// Get long and short names of this mutator
		/// </summary>
		/// <returns>Valuetuple that constains long name and short name</returns>
		(string longName, string shortName) GetNames();

		#endregion // Non essentials
	}
}