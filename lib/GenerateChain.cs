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
				sb.AppendLine($"{CommonSerialization.itemStartChar}{name}{CommonSerialization.delimiter}{1}");
			}

			return sb.ToString();
		}

		#endregion // Serialization
	}
}