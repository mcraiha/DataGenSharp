using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatagenSharp
{
	public sealed class GenerateChain
	{
		public List<IDataGenerator> DataGenerators = new List<IDataGenerator>();

		public List<MutatorChain> MutatorChains = new List<MutatorChain>();

		public List<(string name, IDataGenerator generator, object parameter, Type wantedOutput, MutatorChain mutatorChain)> WantedElements = new List<(string, IDataGenerator,  object, Type, MutatorChain)>();

		public List<string> GetNames()
		{
			return this.WantedElements.Select(element => element.name).ToList();
		}

		public List<object> RunOneStep()
		{
			List<object> returnList = new List<object>(this.WantedElements.Count);

			for (int i = 0; i < this.WantedElements.Count; i++)
			{
				var item = this.WantedElements[i];
				(bool generateSuccess, string possibleError, object tempValue) = item.generator.Generate(item.parameter, item.wantedOutput);
				
				if (item.mutatorChain != null)
				{
					var mutatorChainResult = item.mutatorChain.RunChainOneStep(tempValue);

					tempValue = mutatorChainResult.result;
				}

				returnList.Add(tempValue);
			}

			return returnList;
		}

		public void UpdateToNextStep()
		{
			foreach (IDataGenerator generator in this.DataGenerators)
			{
				generator.NextStep();
			}
		}

		#region Serialization

		public void Load(string parameter)
		{

		}

		public string Save()
		{
			StringBuilder sb = new StringBuilder();

			// Data generators first
			sb.AppendLine(CommonSerialization.generators);
			int currentIndex = 1;
			foreach (IDataGenerator dataGenerator in this.DataGenerators)
			{
				if (dataGenerator is ISerialization serialization)
				{
					sb.AppendLine($"{currentIndex} {dataGenerator.GetNames().shortName}{serialization.Save()}");
					currentIndex++;
				}
			}

			// Then mutator chains
			sb.AppendLine(CommonSerialization.mutatorChains);
			currentIndex = 1;
			foreach (MutatorChain mutatorChain in this.MutatorChains)
			{
				sb.AppendLine($"{currentIndex} {mutatorChain.Save()}");
				currentIndex++;
			}

			// Finally Wanted outputs
			sb.AppendLine(CommonSerialization.wantedElements);
			foreach ((string name, IDataGenerator generator, object parameter, Type wantedOutput, MutatorChain mutatorChain) in this.WantedElements)
			{
				int generatorId = 0;
				if (generator is ISerialization generatorSerialization)
				{
					generatorId = this.FindIndexOfGenerator(generatorSerialization.GetInternalId()) + 1;
				}

				// If generator cannot be found, skip wanted output
				if (generatorId < 1)
				{
					continue;
				}

				int chainId = 0;
				if (mutatorChain != null && mutatorChain is ISerialization mutatorSerialization)
				{
					chainId = this.FindIndexOfMutatorChain(mutatorSerialization.GetInternalId()) + 1;
				}

				string mutatorChainId = (chainId > 0) ? chainId.ToString() : CommonSerialization.missingElementChar.ToString();

				sb.AppendLine($"{CommonSerialization.itemStartChar}{name}{CommonSerialization.delimiter}{generatorId}{CommonSerialization.delimiter}{mutatorChainId}");
			}

			return sb.ToString();
		}

		private int FindIndexOfGenerator(long internalChainId)
		{
			int zeroBasedIndex = -1;

			for (int i = 0; i < this.DataGenerators.Count; i++)
			{
				if (this.DataGenerators[i] is ISerialization serialization)
				{
					if (serialization.GetInternalId() == internalChainId)
					{
						zeroBasedIndex = i;
						break;
					}
				}
			}
			
			return zeroBasedIndex;
		}

		private int FindIndexOfMutatorChain(long internalChainId)
		{
			int zeroBasedIndex = -1;

			for (int i = 0; i < this.MutatorChains.Count; i++)
			{
				if (this.MutatorChains[i] is ISerialization serialization)
				{
					if (serialization.GetInternalId() == internalChainId)
					{
						zeroBasedIndex = i;
						break;
					}
				}
			}
			
			return zeroBasedIndex;
		}

		#endregion // Serialization
	}
}