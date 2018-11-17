## Usage

See also [Fody usage](https://github.com/Fody/Fody#usage).

### NuGet installation

Install the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.

### Add to FodyWeavers.xml

Add `<BasicFodyAddin/>` to [FodyWeavers.xml](https://github.com/Fody/Fody#add-fodyweaversxml)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <AOP/>
</Weavers>
```

### AOP.Fody Project
 A  [AspectInjector](https://github.com/pamidur/aspect-injector) project wrapper by code rewrite for Fody Support. **Aspect Injector** is a framework for creating and injecting aspects into your .net assemblies.

## Features ##
- Compile-time injection
- No runtime dependencies
- Advice debugging
- Interface injection
- Injection into Methods, Properties, Events
- Ability to wrap method around

### Concept ###

Aspect is a class which contains a set of advices - methods which should be injected to certain points in the code. Each advice has mandatory attributes which define a kind of target class members (constructor, getter, setter, regular method etc.) and join points - points in the code where this advice should be injected (before target member, after or both). Aspects and advices are marked with appropriate attributes. For example, we have a class with one method marked as advice:
```C#
[Aspect(Aspect.Scope.Instance)]
class TraceAspect
{
	private int count;
	
	[Advice(Advice.Type.Before, Advice.Target.Method)]
	public void CallCountTrace()
	{
		Console.WriteLine("Call #{0}", count);
		count++;
	}
} 
```

Having it we can apply this aspect to any method:
```C#
//Method CallCountTrace of TraceAspect instance will be called at the beginning of Calculate() 
[Inject(typeof(TraceAspect))]
public void Calculate() { }
```
... or a set of methods of some class:
```C#
//Method CallCountTrace of TraceAspect instance will be called at the beginning of Load() and Save()
[Inject(typeof(TraceAspect))]
class Container
{
    public string Name { get; set; }

    public void Load() { }
    public void Save() { }
}
```
... but not to properties in this case:
```C#
//Will not work - CallCountTrace() advice is applicable to regular methods only
[Inject(typeof(TraceAspect))]
public string Name { get; set; }
```
Please note that there will be only one instace of an aspect per target class regardless of number of affected members. So in the example above Container class will have only one instance of TraceAspect, so both Load() and Save() will increment the same call counter.

#### Target Frameworks

This project must target and `netstandard2.1` (for .NET Framework `net46` or above) so that it can target `msbuild.exe` or `dotnet build`.


