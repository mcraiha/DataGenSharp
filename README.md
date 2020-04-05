# DataGenSharp
C# managed library for generating data, e.g. JSON and CSV/TSV files

[![Build Status](https://travis-ci.com/mcraiha/DataGenSharp.svg?branch=master)](https://travis-ci.com/mcraiha/DataGenSharp)

## Why
Because there are situations where I have to generate data for testing, so it is better to have a tool for that.

## Released version
Current [nuget release](https://www.nuget.org/packages/LibDataGenSharp/) is 0.9.1-git-8fd607a

## How to use
1. Build .dll or include [lib folder](lib) in your project
2. Use code like
```csharp
// Arrange
GenerateData generateData = new GenerateData();
RunningNumberGenerator runningNumberGenerator = new RunningNumberGenerator();
runningNumberGenerator.Init(null, seed: 1337);
generateData.AddGeneratorToChain(runningNumberGenerator);

NameGenerator nameGenerator = new NameGenerator();
nameGenerator.Init(null, seed: 1337);
generateData.AddGeneratorToChain(nameGenerator);

generateData.AddWantedElement(("Id", runningNumberGenerator, typeof(int), null, null));
generateData.AddWantedElement(("Firstname", nameGenerator, typeof(string), null, "firstname"));
generateData.AddWantedElement(("Lastname", nameGenerator, typeof(string), null, "lastname"));

SomeSeparatedValueOutput outCSV = new SomeSeparatedValueOutput();
generateData.output = outCSV;

MemoryStream ms = new MemoryStream();

// Act
generateData.Generate(ms);
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
1. Create instance of **GenerateData**
2. Add all generators 
3. Add all wanted elements to (use generators given in step 2.)
4. Add output 
5. Generate

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
Use [create-nuget-debug.ps1](create-nuget-debug.ps1) go generate the command, it will be something like
```bash
dotnet pack --configuration Debug /p:InformationalVersion="07/21/2018 09:10:01 8866971970797f9d9300438f31bd8712b0defae4" --version-suffix 8866971
```
(this will be improved in future!)

## Testing
Move to [tests](tests) folder if you aren't there yet
### Requirements 
* nunit
* NUnit3TestAdapter
* Microsoft.NET.Test.Sdk
* Newtonsoft.Json

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
* Basic JSON output
* Few test cases
* Random null mutator
* Email mutator
* Boolean generator
* Boolean mutator
* Guid generator
* Integer generator
* IPv4 generator
* Bitcoin address generator

## What is work in progress
* Nuget
* More generators
* More mutators
* More test cases


## What is missing

* Benchmarks
* Better help messages

## License
All code is released under *"Do whatever you want"* license aka [Unlicense](LICENSE)
