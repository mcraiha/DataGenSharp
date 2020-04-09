using System;
using System.Collections.Generic;
using System.Text;

namespace DatagenSharp
{
	public sealed class MutatorChain : ISerialization
	{
		private List<(IMutator mutator, object parameters, Type wantedOutput)> chain = new List<(IMutator mutator, object parameters, Type wantedOutput)>();

		private long internalId = 0;

		public MutatorChain()
		{
			this.internalId = UniqueIdMaker.GetId();
		}

		public (bool success, string possibleError) AddMutatorToChain(IMutator mutator, object parameters, Type wantedOutput)
		{
			// TODO: check if chain is compatible
			this.chain.Add((mutator, parameters, wantedOutput));

			return (success: true, possibleError: "");
		}

		public (bool success, string possibleError, object result) RunChainOneStep(object input)
		{
			if (this.chain == null || this.chain.Count == 0)
			{
				return (success: true, possibleError: "", result: input);
			}

			object tempObject = input;

			foreach (var mutatorEntry in this.chain)
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

		#region Serialization
		public (bool success, string possibleError) Load(string parameter)
		{
			return (true, "");
		}

		public string Save()
		{
			StringBuilder sb = new StringBuilder();

			foreach ((IMutator mutator, object parameters, Type wantedOutput) in this.chain)
			{
				if (mutator is ISerialization serialization)
				{
					sb.Append($"{wantedOutput} {mutator.GetNames().shortName}{serialization.Save()} ");
				}
			}

			return sb.ToString();
		}

		public long GetInternalId()
		{
			return this.internalId;
		}

		#endregion // Serialization
	}
}