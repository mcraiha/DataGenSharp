using System;
using System.Collections.Generic;

namespace DatagenSharp
{
	public class MutatorChain
	{
		public List<(IMutator mutator, string parameters, Type wantedOutput)> chain = new List<(IMutator mutator, string parameters, Type wantedOutput)>();

		public (bool success, string possibleError) AddMutatorToChain(IMutator mutator, string parameters, Type wantedOutput)
		{
			// TODO: check if chain is compatible
			chain.Add((mutator, parameters, wantedOutput));

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) RunChainOneStep(object input)
		{
			if (chain == null || chain.Count == 0)
			{
				return (success: true, possibleError: "", result: input);
			}

			object tempObject = input;

			foreach (var mutatorEntry in chain)
			{
				tempObject = mutatorEntry.mutator.Mutate(tempObject, mutatorEntry.parameters, mutatorEntry.wantedOutput);
			}

			return (success: true, possibleError: "", result: tempObject);
		}
	}
}