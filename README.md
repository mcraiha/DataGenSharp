# DataGenSharp
C# library for generating data, e.g. JSON and CSV/TSV files

## Why
Because there are situations where I have to generate data for testing, so it is better to have tool for that.

## How to use
1. Build .dll or include [lib folder](lib) in your project
2. Use code like
```csharp
// Arrange
RunningNumberGenerator runningNumberGenerator = new RunningNumberGenerator();
GenerateData.chain.DataGenerators.Add(runningNumberGenerator);

NameGenerator nameGenerator = new NameGenerator();
nameGenerator.Init(null, seed: 1337);
GenerateData.chain.DataGenerators.Add(nameGenerator);

GenerateData.chain.OrderDefinition.Add(("Id", runningNumberGenerator, typeof(int), null, null));
GenerateData.chain.OrderDefinition.Add(("Firstname", nameGenerator, typeof(string), null, "firstname"));
GenerateData.chain.OrderDefinition.Add(("Lastname", nameGenerator, typeof(string), null, "lastname"));

SomeSeparatedValueOutput outCSV = new SomeSeparatedValueOutput();
GenerateData.output = outCSV;

MemoryStream ms = new MemoryStream();

// Act
GenerateData.Generate(ms);
string result = Encoding.UTF8.GetString(ms.ToArray());
```

to generate data like
```csv
Id,Firstname,Lastname
0,Jacob,Smith
1,Sophia,Johnson
2,Mason,Williams
3,Isabella,Brown
4,William,Jones
5,Emma,Garcia
6,Jayden,Miller
7,Olivia,Davis
8,Noah,Rodriguez
9,Ava,Martinez
```

all features are under **DatagenSharp** namespace

## How should my brain handle this

### In nutshell
1. Add all generators to **GenerateData**
2. Add all chains to **GenerateData** and use generators given in step 1.
3. Add output for **GenerateData**
4. Generate

### Additional tips
- One generator can be used by multiple chains
- Chain fetches data from generator and then applies mutators in order to that given data
- Generators are notified when one line/entry has been completely generated

## How do I build this
### Requirements
Dotnet core 2.0 environment

### Build .dll
Move to lib folder and run
```bash
dotnet build
```

### Build nuget
TBA

## Testing
### Requirements 
* nunit
* NUnit3TestAdapter
* Microsoft.NET.Test.Sdk

All requirements are restored when you run
```bash
dotnet restore
```

### Run tests
Just call
```bash
dotnet test
```

## What is in
* Running number generator
* Name generator (supports English-US and Finnish names)
* Basic CSV/TSV output
* Few test cases

## What is missing
* More generators
* Mutators
* CI
* Nuget
* More test cases
* Benchmarks
* Better help messages

## License
All code is released under *"Do whatever you want"* license aka [Unlicense](LICENSE)
