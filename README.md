# Domain Driven Design Seedwork

![NUPKG Deploy](https://github.com/japurcell/ddd-seed/.github/workflows/nuget-push.yml/badge.svg)

Reusable base classes for your domain model

* [Entity](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/seedwork-domain-model-base-classes-interfaces#the-custom-entity-base-class)
* [Value Object](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects)
* [Enumeration](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types)

## References

### [Github Actions](https://help.github.com/en/actions/reference/context-and-expression-syntax-for-github-actions)
  * [Env](https://help.github.com/en/actions/configuring-and-managing-workflows/using-environment-variables)
  * [GitTools](https://github.com/GitTools)

### [Nuget](https://docs.microsoft.com/en-us/nuget/)
  * [Versioning](https://docs.microsoft.com/en-us/nuget/concepts/package-versioning)

### [GitVersion](https://gitversion.net/docs/configuration)

### MS Test
  * [RunSettings](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file)
  * [Parallel Execution](https://www.meziantou.net/mstest-v2-execute-tests-in-parallel.htm)
  * Run tests in context: `Command R T`
  * Run tests with coverage: `./test.sh -t`
  * Generate coverage report in ./coverage/report/index.html: `./test.sh -r`
