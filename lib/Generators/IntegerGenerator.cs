using System;
using System.Collections.Generic;
using System.Numerics;

namespace DatagenSharp
{
	public class IntegerGenerator : IDataGenerator
	{
		public static readonly string LongName = "IntegerGenerator";
		
		public static readonly string Description = "Generate integer values (1, 2, 3 etc.)";

		public static readonly string VersionNumber = "0.92";

		private static readonly Type[] supportedOutputTypes = new Type[] { typeof(int), typeof(long), typeof(float), typeof(double), typeof(string) };

		private static readonly List<string> randomModeKeywords = new List<string>() {"random", "rng"};

		private static readonly int defaultMinIntInclusive = 0;
		private static readonly int defaultMaxIntExclusive = 101;

		/// <summary>
		/// Use this value to see if user wants int or long generator
		/// </summary>
		private static readonly long intMax = int.MaxValue;

		/// <summary>
		/// Use this value to see if user wants int or long generator
		/// </summary>
		private static readonly long intMin = int.MinValue;


		private static readonly string[] rangeSeparators = new string[] { ",", ".."};

		private object currentMinInclusive = null;
		private object currentMaxExclusive = null;

		private Type generateType = typeof(int);

		private enum GenerateMode
		{
			Random = 0,
			WeightedRandom
		};

		private GenerateMode chosenMode = GenerateMode.Random;
		private Random rng = null;

		private object currentValue = true;

		public (bool success, string possibleError) Init(object parameter, int seed)
		{
			if (parameter == null)
			{
				// If no parameters are given
				this.generateType = typeof(int);
				this.chosenMode = GenerateMode.Random;
				this.currentMinInclusive = IntegerGenerator.defaultMinIntInclusive;
				this.currentMaxExclusive = IntegerGenerator.defaultMaxIntExclusive;
			}
			else if (parameter.GetType() == typeof(string))
			{
				string tempParameter = (string)parameter;
				// Parse parameters
				foreach (string separator in rangeSeparators)
				{
					if (tempParameter.Contains(separator))
					{
						string[] splitted = tempParameter.Split(new string[] {separator}, StringSplitOptions.None);
						if (splitted.Length != 2)
						{
							return (success: false, possibleError: "Range for random should contain exactly 2 items, e.g. 13-4536");
						}

						if (long.TryParse(splitted[0], out long resultMin))
						{
							// Check if wanted min inclusive is int or long range
							if (resultMin < IntegerGenerator.intMin || resultMin > IntegerGenerator.intMax)
							{
								this.currentMinInclusive = resultMin;
							}
							else
							{
								this.currentMinInclusive = (int)resultMin;
							}
						}
						else
						{
							return (success: false, possibleError: $"{splitted[0]} is not number or it is outside of range {long.MinValue} .. {long.MaxValue}");
						}

						if (long.TryParse(splitted[1], out long resultMax))
						{
							// Check if wanted min inclusive is int or long range
							if (resultMax < IntegerGenerator.intMin || resultMax > IntegerGenerator.intMax)
							{
								this.currentMaxExclusive = resultMax;
							}
							else
							{
								this.currentMaxExclusive = (int)resultMax;
							}
						}
						else
						{
							return (success: false, possibleError: $"{splitted[0]} is not number or it is outside of range {long.MinValue} .. {long.MaxValue}");
						}

						// Check that min and max are same type
						Type tOfMin = this.currentMinInclusive.GetType();
						Type tOfMax = this.currentMaxExclusive.GetType();

						if (tOfMin != tOfMax)
						{
							// If one is int and another is long, turn both to long
							if (tOfMin == typeof(int))
							{
								this.currentMinInclusive = (long) (int) this.currentMinInclusive;
							}

							if (tOfMax == typeof(int))
							{
								this.currentMaxExclusive = (long) (int) this.currentMaxExclusive;
							}
						}

						this.generateType = this.currentMinInclusive.GetType();
						this.chosenMode = GenerateMode.Random;
					}
				}
				
			}
			else
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedParameterType(LongName, "Init", parameter.GetType()));
			}

			if (this.chosenMode == GenerateMode.Random)
			{
				this.rng = new Random(seed);
				this.NextStep();
			}
			else if (this.chosenMode == GenerateMode.WeightedRandom)
			{
				// TODO: Add code

			}

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) Generate(object parameter = null, Type wantedOutput = null)
		{
			object returnValue = this.currentValue;

			if (wantedOutput == null || wantedOutput == typeof(int))
			{
				// Limit to int if needed
				if (this.generateType == typeof(long))
				{
					if ((long)this.currentValue < int.MinValue)
					{
						returnValue = int.MinValue;
					}
					else if ((long)this.currentValue > int.MaxValue)
					{
						returnValue = int.MaxValue;
					}
				}
			}
			else if (wantedOutput == typeof(long))
			{
				// Cast to long
				returnValue = (long)this.currentValue;
			}
			else if (wantedOutput == typeof(float))
			{
				// Cast to float
				if (this.generateType == typeof(int))
				{
					returnValue = (float)(int)this.currentValue;
				}
				else if (this.generateType == typeof(long))
				{
					returnValue = (float)(long)this.currentValue;
				}
			}
			else if (wantedOutput == typeof(double))
			{
				// Cast to double
				if (this.generateType == typeof(int))
				{
					returnValue = (double)(int)this.currentValue;
				}
				else if (this.generateType == typeof(long))
				{
					returnValue = (double)(long)this.currentValue;
				}
			}
			else if (wantedOutput == typeof(string))
			{
				returnValue = this.currentValue.ToString();
			}
			else
			{
				return (success: false, possibleError: ErrorMessages.UnsupportedWantedOutputType(LongName, wantedOutput), result: null);
			}

			return (success: true, possibleError: "", result: returnValue);
		}

		public Type[] GetSupportedOutputs()
		{
			return supportedOutputTypes;
		}

		private static readonly BigInteger rangeDivider = new BigInteger(ulong.MaxValue);
		private static byte[] randomBytesForULong = new byte[8];

		public void NextStep()
		{
			if (this.chosenMode == GenerateMode.Random)
			{
				// Generate new
				if (this.generateType == typeof(int))
				{
					this.currentValue = this.rng.Next((int)this.currentMinInclusive, (int)this.currentMaxExclusive);
				}
				else if (this.generateType == typeof(long))
				{
					// This is slow, but it should somewhat work
					BigInteger rangeStart = new BigInteger((long)currentMinInclusive);
					BigInteger rangeEnd = new BigInteger((long)currentMaxExclusive);				
					rangeEnd--;
					BigInteger range = rangeEnd - rangeStart;
					this.rng.NextBytes(randomBytesForULong);
					BigInteger rangeMultiplier = new BigInteger(BitConverter.ToUInt64(randomBytesForULong, 0));
					BigInteger result = rangeStart + (range * rangeMultiplier / rangeDivider);
					this.currentValue = (long)result;
				}
			}
			else if (this.chosenMode == GenerateMode.WeightedRandom)
			{
				// TODO: Add code
			}
		}

		public (object minRangeInclusive, object maxRangeExclusive) GetRange()
		{
			return (this.currentMinInclusive, this.currentMaxExclusive);
		}
	}
}