# Awesome.Net.Workflows

Awesome.Net.Workflows 是从 [Orchard Core Workflows](https://orchardcore.readthedocs.io/en/dev/OrchardCore.Modules/OrchardCore.Workflows/) 分离出来的，基于 .NET Standard 2.0 的工作流组件，具有跨平台，可扩展，易集成等特点。

## 背景

**在此之前建议您先阅读：**
https://orchardcore.readthedocs.io/en/dev/OrchardCore.Modules/OrchardCore.Workflows/

OrchardCore 附带了强大的工作流模块，其使用的 Liquid & JavaScript 表达式能够让我们轻松访问整个工作流上下文的动态数据（变量），很是亮眼。
但 OrchardCore Workflows 它强依赖了 Web 层，同时它还与 Orchard Core Framework 本身相关联。
出于这两点，我决定分离出一个足够简单、灵活的工作流组件，
允许在任何 .NET 应用程序中集成并使用它。

## 如何使用

```pm
Install-Package Awesome.Net.Workflows
```

```C#
class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection()
            .ConfigureWorkflows() // 配置使用工作流服务
            .BuildServiceProvider();

        var workflowManager = services.GetService<IWorkflowManager>();
        var ctx = await workflowManager.StartWorkflowAsync<Sample01Workflow>();

        Console.ReadLine();
    }
}
```

### Fluent Builder

使用 Fluent 方式定义您的工作流程: 

```C#
public class Sample01Workflow : IWorkflow
{
    public Guid Id { get; } = Guid.NewGuid();
    public string WorkflowTypeId { get; } = RandomHelper.Generate26UniqueId();
    public string Name { get; } = "Sample01Workflow";
    public bool IsEnabled { get; } = true;
    public bool IsSingleton { get; } = false;
    public bool DeleteFinishedWorkflows { get; } = false;

    public void Build(IWorkflowBuilder builder)
    {
        builder.StartWith<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("StartWith ConsoleWriteLineTask"))
            .Then<SetPropertyTask>(x =>
            {
                x.PropertyName = "Value";
                x.Value = new JavaScriptExpr<object>("'Awesome!'");
            })
            .Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("Value has set: {{Workflow.Properties['Value']}}"))
            .Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("Then Fork"))
            .Fork("fork1")
            .Branch(then => then.Then<ConsoleWriteLineTask>(log => log.Text = new LiquidExpr("Branch branch1"))
                .Then<SetPropertyTask>(x =>
                    {
                        x.PropertyName = "Value";
                        x.Value = new JavaScriptExpr<object>("'0'");
                    })
                .Then<IfElseTask>(x => x.Condition = new JavaScriptExpr<bool>("property('Value')==3"), b =>
                     {
                         b.When(false).Then<ConsoleWriteLineTask>(x =>
                                 x.Text = new LiquidExpr("Value is: {{Workflow.Properties['Value']}}"))
                             .Then<SetPropertyTask>(x =>
                                 {
                                     x.PropertyName = "Value";
                                     x.Value = new JavaScriptExpr<object>("'3'");
                                 })
                             .Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("Value has set: {{Workflow.Properties['Value']}}"))
                             .Then<SwitchTask>(x =>
                                 {
                                     x.Cases = new List<string> { "1", "2", "3" };
                                     x.Expression = new JavaScriptExpr("property('Value')");
                                 },
                                 @switch =>
                                 {
                                     @switch.When("1").Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("@switch case 1"));
                                     @switch.When("2").Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("@switch case 2"));
                                     @switch.When("3").Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("@switch case 3")).Connect("fork1_join");
                                 });
                     })
            )
            .Branch(then => then.Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("Branch branch2")).Connect("fork1_join"))
            .Branch(then => then.Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("Branch branch3")).Connect("fork1_join"))
            .Join("fork1_join", waitAll: false)
            .Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("Joined"));
    }
}
```

### JSON 工作流定义

使用 **IWorkflowTypeLoader** 可以允许您加载来自 JSON 文件（也可以是 XAML/YAML ，这取决于您的 deserializer）：

```C#
public class WorkflowTypeLoader : IWorkflowTypeLoader
{
    public WorkflowType Load(string source, Func<string, WorkflowType> deserializer)
    {
        return deserializer.Invoke(source);
    }
}
```

```C#
var loader = services.GetService<IWorkflowTypeLoader>();
var fromJsonType = loader.Load(workflowTypeJson, JsonConvert.DeserializeObject<WorkflowType>);

var workflowManager = services.GetService<IWorkflowManager>();
await workflowManager.StartWorkflowAsync(fromJsonType);
```

### 可视化设计器

（TODO）
https://github.com/elsa-workflows/elsa-designer-html

## 持久化

几乎任何存储介质都可以存储工作流程/定义。

- In Memory （默认已实现）
- SQL Server
- MongoDB
- CosmosDB
- ...

## 自定义活动

```C#
public class ConsoleWriteLineTask : TaskActivity
{
    public override LocalizedString Category => T["Console"];

    public WorkflowExpression<string> Text
    {
        get => GetExpressionProperty<string>();
        set => SetProperty(value);
    }

    public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
        ActivityExecutionContext activityContext)
    {
        return Outcomes(T["Done"]);
    }

    public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext,
        ActivityExecutionContext activityContext)
    {
        var text = await ExpressionEvaluator.EvaluateAsync(Text, workflowContext);
        Console.WriteLine(text);
        return Outcomes("Done");
    }

    public ConsoleWriteLineTask(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
```

## 更多例子

与 ABP vNext 的集成（TODO）

## 线路图

（TODO）

- 实现更多活动
- 更加详细的文档
- RabbitMQ 集成
- Redis 集成
- ...

## 致敬

[**Orchard Core**](https://github.com/OrchardCMS/OrchardCore/)
Copyright (c) .NET Foundation BSD 3-Clause License.

[**elsa-workflows**](https://github.com/elsa-workflows/)
Copyright (c) 2019, Sipke Schoorstra BSD 3-Clause License.

[**workflow-core**](https://github.com/danielgerlag/workflow-core/)
Copyright (c) 2016 Daniel Gerlag MIT License

## License

BSD 3-Clause License
