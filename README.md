# DataGenSharp
C# library for generating data

## Why
Because there are situations where I have to generate data for testing

## How to use
1. Build .dll or include [lib folder](lib) in your project

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
* Only running number generator
* CSV/TSV output
* One test case

## What is missing
* More generators
* Mutators
* CI
* Nuget
* Proper test cases
* Benchmarks

## License
All code is released under *"Do whatever you want"* license aka [Unlicense](LICENSE)
