# Tyml
YAML Task Runner

TYML is library able to take YAML file in given format and process its instructions. It is like Your private local pipeline.

## Example

### Configuration
```c#
TymlContext tymlContext = new TymlContextBuilder()
    .UseTasks(typeof(CmdTask))
    .UseWorkingDirectory(path)
    .WithBaseVariables(new Dictionary<string, object>()
    {
        {"foo", 5},
        {"bar", "baz"}
    })
    .Build();
    
TymlExecutor tymlExecutor = provider.GetRequiredService<TymlExecutor>();

await tymlExecutor.Execute(tymlContext, CONTENT_OF_YAML_FILE);
await tymlExecutor.Execute(tymlContext, CONTENT_OF_SOME_OTHER_YAML_FILE);
```

### YAML Format
```yaml
description: "Some description of this YAML"
variables:
  ProjectDescription: |-
    This is testing multiline project description.
    This is second line of it.
    with: colon

steps:
    - task: Cmd
      displayName: "Run something"
      inputs:
        Script: "something.exe"
        Args:
          SomeEnvVariable: $(ProductionEnvironment)
          ProjectDescription: '$(ProjectDescription)'
          FooVariable: $(foo)! # Required variable
```

### Custom Tasks
```c#
[TymlTask("cmd")]
public class CmdTask : TaskBase<CmdTaskConfig>
{
    protected override Task Execute(TaskContext context, CmdTaskConfig inputs)
    {
        Console.WriteLine();
        Console.WriteLine($"Script: {inputs.Script} with args: {string.Join("; ", inputs.Args.Select(entry => entry.Key + ":" + entry.Value))}");
        return Task.CompletedTask;
    }
}
```