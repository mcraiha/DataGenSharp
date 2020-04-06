using System;
using System.Collections.Generic;
using System.Linq;

namespace DatagenSharp
{
	public class GenerateChain
	{
		public List<IDataGenerator> DataGenerators = new List<IDataGenerator>();

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
			return "";
		}

		#endregion // Serialization
	}
}