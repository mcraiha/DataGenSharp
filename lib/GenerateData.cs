using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DatagenSharp
{
	/// <summary>
	/// Top level class for data generation
	/// </summary>
	public sealed class GenerateData
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

		public void AddMutatorToNewChain(IMutator mutator, object parameters, Type wantedOutput)
		{
			MutatorChain mutatorChain = new MutatorChain();
			mutatorChain.AddMutatorToChain(mutator, parameters, wantedOutput);
			this.chain.MutatorChains.Add(mutatorChain);
		}

		public (bool success, string possibleError) AddMutatorToExistingChain(long internalChainId, IMutator mutator, object parameters, Type wantedOutput)
		{
			MutatorChain existingChain = this.chain.MutatorChains.Find((MutatorChain mc) => mc.GetInternalId() == internalChainId);
			if (existingChain == null)
			{
				return (false, $"Cannot find existing chain with internal ID: {internalChainId}");
			}

			existingChain.AddMutatorToChain(mutator, parameters, wantedOutput);

			return (true, "");
		}

		/// <summary>
		/// Add wanted element
		/// </summary>
		/// <param name="name">Name of wanted element. e.g. "Id", "Name", "Email"</param>
		/// <param name="generator">Generator that will be generating the content for the element</param>
		/// <param name="parameter">Optional parameter for the generator</param>
		/// <param name="wantedOutput">Output type wanted to generate</param>
		/// <param name="mutatorChain">Optional mutator chain that will mutate the output</param>
		public void AddWantedElement((string name, IDataGenerator generator, object parameter, Type wantedOutput, MutatorChain mutatorChain) wantedElement)
		{
			this.chain.WantedElements.Add(wantedElement);
		}

		private void AddWantedElement(string name, int zeroBasedGeneratorIndex, int zeroBasedMutatorChainIndex)
		{
			// Since mutator chains are not mandatory, check it
			this.AddWantedElement(name, this.chain.DataGenerators[zeroBasedGeneratorIndex], null, null, zeroBasedMutatorChainIndex > -1 ? this.chain.MutatorChains[zeroBasedMutatorChainIndex] : null);
		}

		/// <summary>
		/// Add wanted element
		/// </summary>
		/// <param name="name">Name of wanted element. e.g. "Id", "Name", "Email"</param>
		/// <param name="generator">Generator that will be generating the content for the element</param>
		/// <param name="parameter">Optional parameter for the generator</param>
		/// <param name="wantedOutput">Output type wanted to generate</param>
		/// <param name="mutatorChain">Optional mutator chain that will mutate the output</param>
		public void AddWantedElement(string name, IDataGenerator generator, object parameter, Type wantedOutput, MutatorChain mutatorChain)
		{
			this.AddWantedElement((name, generator, parameter, wantedOutput, mutatorChain));
		}

		#endregion // Helpers


		#region Serialization

		private enum ReadMode
		{
			SeekingSection = 0,
			ReadGenerators,
			ReadMutatorChains,
			ReadWantedElements,
			ReadOutputter,
		}

		private ReadMode readMode = ReadMode.SeekingSection;

		public (bool success, string possibleError) Load(string input)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			Type[] typesInAssembly = assembly.GetTypes();

			Dictionary<string, Type> typesOfIDataGenerator = new Dictionary<string, Type>();
			IEnumerable<Type> implementIDataGenerator = typesInAssembly.Where(t => t.GetInterfaces().Contains(typeof(IDataGenerator)));

			Dictionary<string, Type> typesOfIDataOutputter = new Dictionary<string, Type>();
			IEnumerable<Type> implementIDataOutputter = typesInAssembly.Where(t => t.GetInterfaces().Contains(typeof(IDataOutputter)));

			//IEnumerable<Type> implementISerialization = typesInAssembly.Where(t => t.GetInterfaces().Contains(typeof(ISerialization)));
			HashSet<Type> implementISerialization = new HashSet<Type>(typesInAssembly.Where(t => t.GetInterfaces().Contains(typeof(ISerialization))));

			foreach (Type type in implementIDataGenerator)
			{
				// We only want to keep track of types that implement both IDataGenerator and ISerialization
				if (implementISerialization.Contains(type))
				{
					var tempObj = Activator.CreateInstance(type) as IDataGenerator;
					typesOfIDataGenerator[tempObj.GetNames().shortName] = type;
				}
			}

			foreach (Type type in implementIDataOutputter)
			{
				// We only want to keep track of types that implement both IDataOutputter and ISerialization
				if (implementISerialization.Contains(type))
				{
					var tempObj = Activator.CreateInstance(type) as IDataOutputter;
					typesOfIDataOutputter[tempObj.GetNames().shortName] = type;
				}
			}

			this.readMode = ReadMode.SeekingSection;
			using (StringReader reader = new StringReader(input))
			{
				string line = reader.ReadLine();
				while (line != null)
				{
					if (line.StartsWith(CommonSerialization.generators))
					{
						this.readMode = ReadMode.ReadGenerators;
					}
					else if (line.StartsWith(CommonSerialization.mutatorChains))
					{
						this.readMode = ReadMode.ReadMutatorChains;
					}
					else if (line.StartsWith(CommonSerialization.wantedElements))
					{
						this.readMode = ReadMode.ReadWantedElements;
					}
					else if (line.StartsWith(CommonSerialization.outputter))
					{
						this.readMode = ReadMode.ReadOutputter;
					}
					else
					{
						if (readMode == ReadMode.ReadGenerators)
						{
							int indexOfFirstSpace = line.IndexOf(' ');
							string indexNumber = line.Substring(0, indexOfFirstSpace);
							string generator = line.Substring(indexOfFirstSpace + 1);

							if (!int.TryParse(indexNumber, out int generatorIndex))
							{
								return (false, $"Line: {line} has invalid generator index value");
							}

							string[] generatorSplitted = generator.Split(CommonSerialization.delimiter);
							if (!typesOfIDataGenerator.ContainsKey(generatorSplitted[0]))
							{
								return (false, $"Line: {line} has unknown generator shortname {generatorSplitted[0]}");
							}

							if (!int.TryParse(generatorSplitted[2], out int rngSeed))
							{
								return (false, $"Line: {line} has invalid rng seed {generatorSplitted[2]}");
							}

							var generatorToAdd = Activator.CreateInstance(typesOfIDataGenerator[generatorSplitted[0]]) as IDataGenerator;
							//bool shouldParameterBeNull = (string.IsNullOrEmpty(generatorSplitted[1]) || generatorSplitted[1] == CommonSerialization.nullValue);
							(bool generatorLoadSuccess, string generatorLoadError) = ((ISerialization)generatorToAdd).Load($"{CommonSerialization.delimiter}{generatorSplitted[1]}{CommonSerialization.delimiter}{generatorSplitted[2]}");
							
							if (!generatorLoadSuccess)
							{
								return (generatorLoadSuccess, generatorLoadError);
							}

							this.AddGeneratorToChain(generatorToAdd);
						}
						else if (readMode == ReadMode.ReadMutatorChains)
						{

						}
						else if (readMode == ReadMode.ReadWantedElements)
						{
							if (!line.StartsWith("-"))
							{
								return (false, $"Line: {line} does not start with -");
							}

							string[] wantedElementsSplitted = line.Split(CommonSerialization.delimiter);

							string name = wantedElementsSplitted[0].Substring(1);

							if (!int.TryParse(wantedElementsSplitted[1], out int generatorIndex))
							{
								return (false, $"Line: {line} has invalid generator index value");
							}

							int mutatorChainIndex = -1;

							if (wantedElementsSplitted[2].StartsWith(CommonSerialization.missingElementChar.ToString()))
							{

							}
							else if (!int.TryParse(wantedElementsSplitted[2], out mutatorChainIndex))
							{
								return (false, $"Line: {line} has invalid mutator chain index value");
							}

							this.AddWantedElement(name, generatorIndex - 1, mutatorChainIndex - 1);
						}
						else if (readMode == ReadMode.ReadOutputter)
						{
							string[] outputterSplitted = line.Split(CommonSerialization.delimiter);
							var outputterToAdd = Activator.CreateInstance(typesOfIDataOutputter[outputterSplitted[0]]) as IDataOutputter;
							this.output = outputterToAdd;
						}
					}


					line = reader.ReadLine();
				}
			}

			return (true, "");
		}

		public string Save()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine(CommonSerialization.header);
			sb.Append(this.chain.Save());
			sb.AppendLine(CommonSerialization.outputter);
			sb.AppendLine($"{this.output.GetNames().shortName}{((ISerialization)this.output).Save()}");

			return sb.ToString();
		}

		#endregion // Serialization
	}
}