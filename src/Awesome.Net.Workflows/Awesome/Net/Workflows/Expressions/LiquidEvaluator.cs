using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Liquid;
using Awesome.Net.Workflows.Events;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Models;
using Fluid;
using Fluid.Values;
using Volo.Abp.EventBus.Local;

namespace Awesome.Net.Workflows.Expressions
{
    public class LiquidEvaluator : ISyntaxEvaluator
    {
        private readonly ILocalEventBus _localEventBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        public string Syntax => SyntaxNameAttribute.GetSyntax(typeof(LiquidExpression));

        public LiquidEvaluator(
            IServiceProvider serviceProvider,
            ILiquidTemplateManager liquidTemplateManager,
            ILocalEventBus localEventBus)
        {
            _serviceProvider = serviceProvider;
            _liquidTemplateManager = liquidTemplateManager;
            _localEventBus = localEventBus;
        }


        public async Task<T> EvaluateAsync<T>(string expression, WorkflowExecutionContext workflowExecutionContext, CancellationToken cancellationToken = default, params object[] @params)
        {
            var templateContext = await CreateTemplateContextAsync(workflowExecutionContext);

            await _localEventBus.PublishAsync(new EvaluatingLiquidEventData(workflowExecutionContext, templateContext));

            var result = await _liquidTemplateManager.RenderAsync(expression, System.Text.Encodings.Web.JavaScriptEncoder.Default, templateContext);
            return string.IsNullOrWhiteSpace(result) ? default : (T)Convert.ChangeType(result, typeof(T));
        }

        private Task<TemplateContext> CreateTemplateContextAsync(WorkflowExecutionContext workflowContext)
        {
            var context = new TemplateContext();
            var services = _serviceProvider;

            // Set WorkflowEventData as the model.
            context.MemberAccessStrategy.Register<LiquidPropertyAccessor, FluidValue>((obj, name) => obj.GetValueAsync(name));
            context.MemberAccessStrategy.Register<WorkflowExecutionContext>();
            context.MemberAccessStrategy.Register<WorkflowExecutionContext, LiquidPropertyAccessor>("Input", obj => new LiquidPropertyAccessor(name => ToFluidValue(obj.Input, name)));
            context.MemberAccessStrategy.Register<WorkflowExecutionContext, LiquidPropertyAccessor>("Output", obj => new LiquidPropertyAccessor(name => ToFluidValue(obj.Output, name)));
            context.MemberAccessStrategy.Register<WorkflowExecutionContext, LiquidPropertyAccessor>("Properties", obj => new LiquidPropertyAccessor(name => ToFluidValue(obj.Properties, name)));

            context.SetValue("Workflow", workflowContext);
            // Add services.
            context.AmbientValues.Add("Services", services);


            return Task.FromResult(context);
        }

        private Task<FluidValue> ToFluidValue(IDictionary<string, object> dictionary, string key)
        {
            if(!dictionary.ContainsKey(key))
            {
                return Task.FromResult(default(FluidValue));
            }

            return Task.FromResult(FluidValue.Create(dictionary[key]));
        }
    }
}
