using System;
using System.Collections.Generic;
using System.Linq;

namespace DatagenSharp
{
	public class GenerateChain
	{
		public List<IDataGenerator> DataGenerators = new List<IDataGenerator>();

		public List<(string name, IDataGenerator generator, Type wantedOutput, MutatorChain mutatorChain, object parameter)> OrderDefinition = new List<(string, IDataGenerator, Type, MutatorChain, object)>();

		public List<string> GetNames()
		{
			return this.OrderDefinition.Select(element => element.name).ToList();
		}

		public List<object> RunOneStep()
		{
			List<object> returnList = new List<object>(this.OrderDefinition.Count);

			for (int i = 0; i < this.OrderDefinition.Count; i++)
			{
				var item = this.OrderDefinition[i];
				object tempValue = item.generator.Generate(item.parameter, item.wantedOutput);
				
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

		public string Save()
		{
			return "";
		}
	}
}