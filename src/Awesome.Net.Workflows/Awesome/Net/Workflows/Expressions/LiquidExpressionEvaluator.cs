using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Liquid;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Handlers;
using Fluid;
using Fluid.Values;
using Microsoft.Extensions.Logging;

namespace Awesome.Net.Workflows.Expressions
{
    public class LiquidExpressionEvaluator : IExpressionEvaluator
    {
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        private readonly IEnumerable<IWorkflowExecutionContextHandler> _workflowContextHandlers;
        private readonly ILogger<JavaScriptExpressionEvaluator> _logger;
        public string Syntax => SyntaxNameAttribute.GetSyntax(typeof(LiquidExpression));

        public LiquidExpressionEvaluator(
            ILiquidTemplateManager liquidTemplateManager,
            IEnumerable<IWorkflowExecutionContextHandler> workflowContextHandlers,
            ILogger<JavaScriptExpressionEvaluator> logger)
        {
            _liquidTemplateManager = liquidTemplateManager;
            _workflowContextHandlers = workflowContextHandlers;
            _logger = logger;
        }


        public async Task<T> EvaluateAsync<T>(string expression,
            WorkflowExecutionContext workflowContext,
            IDictionary<string, object> arguments = null,
            CancellationToken cancellationToken = default)
        {
            var templateContext = await CreateTemplateContextAsync(workflowContext);

            var expressionContext = new WorkflowExecutionLiquidContext(templateContext, workflowContext);

            await _workflowContextHandlers.InvokeAsync(x => x.EvaluatingLiquidAsync(expressionContext), _logger);

            var result = await _liquidTemplateManager.RenderAsync(expression,
                System.Text.Encodings.Web.JavaScriptEncoder.Default, templateContext);
            return string.IsNullOrWhiteSpace(result) ? default : (T) Convert.ChangeType(result, typeof(T));
        }

        private Task<TemplateContext> CreateTemplateContextAsync(WorkflowExecutionContext workflowContext)
        {
            var context = new TemplateContext();

            // Set WorkflowEventData as the model.
            context.MemberAccessStrategy.Register<LiquidPropertyAccessor, FluidValue>((obj, name) =>
                obj.GetValueAsync(name));
            context.MemberAccessStrategy.Register<WorkflowExecutionContext>();
            context.MemberAccessStrategy.Register<WorkflowExecutionContext, LiquidPropertyAccessor>("Input",
                obj => new LiquidPropertyAccessor(name => ToFluidValue(obj.Input, name)));
            context.MemberAccessStrategy.Register<WorkflowExecutionContext, LiquidPropertyAccessor>("Output",
                obj => new LiquidPropertyAccessor(name => ToFluidValue(obj.Output, name)));
            context.MemberAccessStrategy.Register<WorkflowExecutionContext, LiquidPropertyAccessor>("Properties",
                obj => new LiquidPropertyAccessor(name => ToFluidValue(obj.Properties, name)));

            context.SetValue("Workflow", workflowContext);

            return Task.FromResult(context);
        }

        private Task<FluidValue> ToFluidValue(IDictionary<string, object> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                return Task.FromResult(default(FluidValue));
            }

            return Task.FromResult(FluidValue.Create(dictionary[key]));
        }
    }
}
