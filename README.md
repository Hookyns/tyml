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

IList<TaskOutput> exec1Outputs = await tymlExecutor.Execute(tymlContext, CONTENT_OF_YAML_FILE);
IList<TaskOutput> exec2Outputs = await tymlExecutor.Execute(tymlContext, CONTENT_OF_SOME_OTHER_YAML_FILE);
```

### YAML Format
```yaml
description: "Some description of this YAML" # optional description
variables: # optional variable section; overwriting environment variables and TymlContext variables.
  ProjectDescription: |-
    This is testing multiline project description.
    This is second line of it.
    with: colon

steps:
  - task: Cmd # Name of task
    displayName: "Run something" # Optional display name
    inputs: # Input arguments parsed into IDictionary passed to ITask.Execute or parsed into generic type of TaskBase<TInputs> if you inherit from that 
      Script: "something.exe"
      Args:
        SomeEnvVariable: $(ProductionEnvironment) # $(..) replaced by variable value before execution if defined
        ProjectDescription: '$(ProjectDescription)'
        FooVariable: $(foo)! # Required variable; Exception thrown if not defined
```

## Predefined Tasks 
You can install package `RJDev.Tyml.Tasks.Base` which implements basic tasks such as Cmd (run script on cmd/bash) and ExtractFiles.

## Custom Tasks
If You want to create custom Task You must implement `ITask` interface or use generic abstract class `TaskBase<TOfInputObject>`.
If You want to publish your packages to NuGet, it would be nice to keep some naming convention, at least keep `Tyml.Tasks.{WhatKindOfTasks}`; or create PR to Tasks.Basic.

```c#
[TymlTask("Cmd", "Optional task description")]
public class CmdTask : TaskBase<CmdTaskConfig>
{
    protected override async Task Execute(TaskContext context, CmdTaskConfig inputs)
    {
        await context.Output.WriteLineAsync($"Script: {inputs.Script} with args: {string.Join("; ", inputs.Args.Select(entry => entry.Key + ":" + entry.Value))}");
        return Task.CompletedTask;
    }
}
```