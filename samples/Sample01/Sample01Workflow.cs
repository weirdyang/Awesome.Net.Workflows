using System;
using System.Collections.Generic;
using Awesome.Net;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.FluentBuilders;

namespace Sample01
{
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
            builder
                .StartWith<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("StartWith ConsoleWriteLineTask"))
                .Then<SetPropertyTask>(x =>
                {
                    x.PropertyName = "Value";
                    x.Value = new JavaScriptExpr<object>("'Awesome!'");
                })
                .Then<ConsoleWriteLineTask>(x =>
                    x.Text = new LiquidExpr("Value has set: {{Workflow.Properties['Value']}}"))
                .Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("Then Fork"))
                .Fork("fork1")
                .Do("branch1", then => then.Then<ConsoleWriteLineTask>(log => log.Text = new LiquidExpr("Do branch1"))
                    .Then<SetPropertyTask>(
                        x =>
                        {
                            x.PropertyName = "Value";
                            x.Value = new JavaScriptExpr<object>("'0'");
                        })
                    .Then<IfElseTask>(x => x.Condition = new JavaScriptExpr<bool>("property('Value')==3"),
                        b =>
                        {
                            b.When(false).Then<ConsoleWriteLineTask>(x =>
                                    x.Text = new LiquidExpr("Value is: {{Workflow.Properties['Value']}}"))
                                .Then<SetPropertyTask>(
                                    x =>
                                    {
                                        x.PropertyName = "Value";
                                        x.Value = new JavaScriptExpr<object>("'3'");
                                    })
                                .Then<ConsoleWriteLineTask>(x =>
                                    x.Text = new LiquidExpr("Value has set: {{Workflow.Properties['Value']}}"))
                                .Then<SwitchTask>(x =>
                                    {
                                        x.Cases = new List<string> {"1", "2", "3"};
                                        x.Expression = new JavaScriptExpr("property('Value')");
                                    },
                                    @switch =>
                                    {
                                        @switch.When("1").Then<ConsoleWriteLineTask>(x =>
                                            x.Text = new LiquidExpr("@switch case 1"));
                                        @switch.When("2").Then<ConsoleWriteLineTask>(x =>
                                            x.Text = new LiquidExpr("@switch case 2"));
                                        @switch.When("3").Then<ConsoleWriteLineTask>(x =>
                                            x.Text = new LiquidExpr("@switch case 3")).Connect("fork1_join");
                                    });
                        })
                )
                .Do("branch2",
                    then => then.Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("Do branch2"))
                        .Connect("fork1_join"))
                .Do("branch3",
                    then => then.Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("Do branch3"))
                        .Connect("fork1_join"))
                .Join("fork1_join", waitAll: false)
                .Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("Joined"));
        }
    }
}
