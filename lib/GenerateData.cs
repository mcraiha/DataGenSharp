using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DatagenSharp
{
	public class GenerateData
	{
		public GenerateChain chain = new GenerateChain();

		public IDataOutputter output;

		public object outputParameters = null;

		public uint howManyStepToRun = 10;

		public (bool success, string possibleError) Generate(Stream outputStream)
		{
			// Init output
			(bool initSuccess, string possibleInitError) = this.output.Init(outputParameters, outputStream);

			// Verify that init went OK
			if (!initSuccess)
			{
				return (success: initSuccess, possibleError: possibleInitError);
			}


			// Write header
			var names = this.chain.GetNames();
			this.output.WriteHeader(names.ConvertAll(s => (object)s));

			List<object> entries = null;

			// Write each entry
			for (int i = 0; i < howManyStepToRun; i++)
			{
				entries = GetCurrentLine();
				(bool entryWriteSuccess, string possibleEntryWriteError) = this.output.WriteSingleEntry(entries);

				if (!entryWriteSuccess)
				{
					return (success: entryWriteSuccess, possibleError: possibleEntryWriteError);
				}

				this.chain.UpdateToNextStep();
			}

			// Write footer
			return output.WriteFooter(entries);
		}

		private List<object> GetCurrentLine()
		{
			return this.chain.RunOneStep();
		}


		#region Helpers

		public void AddGeneratorToChain(IDataGenerator generator)
		{
			this.chain.DataGenerators.Add(generator);
		}

		public void AddGeneratorsToChain(params IDataGenerator[] generators)
		{
			this.chain.DataGenerators.AddRange(generators);
		}

		public void AddWantedElement((string name, IDataGenerator generator, Type wantedOutput, MutatorChain mutatorChain, object parameter) wantedElement)
		{
			this.chain.WantedElements.Add(wantedElement);
		}

		public void AddWantedElement(string name, IDataGenerator generator, Type wantedOutput, MutatorChain mutatorChain, object parameter)
		{
			this.AddWantedElement((name, generator, wantedOutput, mutatorChain, parameter));
		}

		#endregion // Helpers


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