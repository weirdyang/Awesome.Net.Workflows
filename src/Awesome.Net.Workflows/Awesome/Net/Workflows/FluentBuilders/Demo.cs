using System;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Expressions.Syntaxs;

namespace Awesome.Net.Workflows.FluentBuilders
{
    public class Demo : IWorkflow
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string WorkflowTypeId { get; } = RandomHelper.Generate26UniqueId();
        public string Name { get; } = "Demo01";
        public bool IsEnabled { get; } = true;
        public bool IsSingleton { get; } = false;
        public bool DeleteFinishedWorkflows { get; } = false;

        public void Build(IWorkflowBuilder builder)
        {
            builder.StartWith<LogTask>(log => log.Text = new LiquidExpr("Start"))
                .Then<SetPropertyTask>(x =>
                {
                    x.PropertyName = "Value";
                    x.Value = new JavaScriptExpr<object>("'true'");
                })
                .Then<IfElseTask>(null, ifElse =>
                    {
                        ifElse.When(true)
                            .Then<LogTask>(log => log.Text = new LiquidExpr("true"))
                            .Then<SwitchTask>(null, @switch =>
                                {
                                    @switch.When("1").Then<LogTask>(log => log.Text = new LiquidExpr("1"));
                                    @switch.When("2").Then<LogTask>(log => log.Text = new LiquidExpr("2"));
                                    @switch.When("3").Then<LogTask>(log => log.Text = new LiquidExpr("3"));
                                }
                            );

                        ifElse.When(false)
                            .Then<LogTask>(log => log.Text = new LiquidExpr("false"))
                            .Fork()
                            .Do("branch1", then => then.Then<LogTask>(log => log.Text = new LiquidExpr("Start"))
                                .Then<SetPropertyTask>(x =>
                                {
                                    x.PropertyName = "Value";
                                    x.Value = new JavaScriptExpr<object>("'true'");
                                })
                                .Then<IfElseTask>(null, b =>
                                    {
                                        b.When(true).Then<LogTask>(log => log.Text = new LiquidExpr("true"));
                                        b.When(false).Then<LogTask>(log => log.Text = new LiquidExpr("false"));
                                    }
                                )
                                .Then<SwitchTask>(null, b =>
                                    {
                                        b.When("1").Then<LogTask>(log => log.Text = new LiquidExpr("1"));
                                        b.When("2").Then<LogTask>(log => log.Text = new LiquidExpr("2"));
                                        b.When("3").Then<LogTask>(log => log.Text = new LiquidExpr("3"));
                                    }
                                ))
                            .Do("branch2", then => then.Then<LogTask>(log => log.Text = new LiquidExpr("Start")))
                            .Join(waitAll: false, "branch1", "branch2");
                    }
                );
        }
    }
}
