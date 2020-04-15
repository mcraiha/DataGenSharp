using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DatagenSharp
{
	public sealed class BooleanMutator : IMutator
	{
		private static readonly char defaultValueSeparator = ',';

		public static readonly string LongName = "BooleanMutator";

		public static readonly string ShortName = "BOOL";
		
		public static readonly string Description = "Turns Boolean values into something else";

		public static readonly string Examples = "true -> 'dog'";

		public static readonly string VersionNumber = "0.9";

		private static readonly Type[] supportedInputTypes = new Type[] { typeof(bool) };

		/// <summary>
		/// With this mutator supported output types is dynamic since output type can vary
		/// </summary>
		/// <returns>Output types that are always supported</returns>
		private static readonly Type[] supportedOutputTypes = new Type[] { typeof(string)};

		private object replaceForTrue = null;

		private object replaceForFalse = null;

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			if (parameter == null)
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedNullParameter(LongName, "Init"));
			}

			(bool isArray, Type elementType) = ParameterParser.PrecheckPossibleArray(parameter);

			if (isArray)
			{
				IEnumerable enumerable = parameter as IEnumerable;
				object[] parameters = enumerable.Cast<object>().ToArray();
				if (parameters.Length != 2)
				{
					return (success: false, ErrorMessages.ParameterArrayNotEnoughElements(LongName, "Init", 2, parameters.Length));
				}

				this.replaceForTrue = parameters[0];
				this.replaceForFalse = parameters[1];
			}
			else if (parameter.GetType() == typeof(string))
			{
				string[] splitted = ((string)parameter).Split(defaultValueSeparator);
				if (splitted.Length != 2)
				{
					return (success: false, $"{LongName} Init string needs to have {defaultValueSeparator} for separating true and false values!");
				}

				this.replaceForTrue = splitted[0];
				this.replaceForFalse = splitted[1];
			}
			else
			{
				return (success: false, ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType()));
			}

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Mutate(object input, object parameter = null, Type wantedOutputType = null)
		{
			// Check that input is supported type
			if (Array.IndexOf(supportedInputTypes, input.GetType()) == -1)
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedInputType(LongName, input.GetType()), result: null);
			}

			// Check that wanted output type is supported
			if (wantedOutputType != null && Array.IndexOf(this.GetSupportedOutputs(), wantedOutputType) == -1)
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedWantedOutputType(LongName, wantedOutputType), result: null);
			}

			bool val = (bool)input;

			object returnValue = this.replaceForTrue;

			if (!val)
			{
				returnValue = this.replaceForFalse;
			}

			// If string is wanted, then call ToString()
			if (wantedOutputType == typeof(string))
			{
				returnValue = returnValue.ToString();
			}

			return (success: true, possibleError: "", result: returnValue);
		}

		public Type[] GetSupportedOutputs()
		{
			// We have to support dynamic outputs here, so add current replaceForTrue and replaceForFalse values to supported
			List<Type> returnValue = new List<Type>(supportedOutputTypes);
			
			if (this.replaceForTrue != null && !returnValue.Contains(this.replaceForTrue.GetType()))
			{
				returnValue.Add(this.replaceForTrue.GetType());
			}

			if (this.replaceForFalse != null && !returnValue.Contains(this.replaceForFalse.GetType()))
			{
				returnValue.Add(this.replaceForFalse.GetType());
			}

			return returnValue.ToArray();
		}

		public (string longName, string shortName) GetNames()
		{
			return (LongName, ShortName);
		}
	}
}