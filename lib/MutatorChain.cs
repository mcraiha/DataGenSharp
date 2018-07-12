using System;
using System.Collections.Generic;

namespace DatagenSharp
{
	public class MutatorChain
	{
		private List<(IMutator mutator, object parameters, Type wantedOutput)> chain = new List<(IMutator mutator, object parameters, Type wantedOutput)>();

		public (bool success, string possibleError) AddMutatorToChain(IMutator mutator, object parameters, Type wantedOutput)
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
				(bool mutateSuccess, string possibleMutateError, object mutateResult) = mutatorEntry.mutator.Mutate(tempObject, mutatorEntry.parameters, mutatorEntry.wantedOutput);

				if (!mutateSuccess)
				{
					return (success: false, possibleError: possibleMutateError, result: null);
				}

				tempObject = mutateResult;
			}

			return (success: true, possibleError: "", result: tempObject);
		}
	}
}