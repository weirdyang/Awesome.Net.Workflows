using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;

namespace Awesome.Net.Workflows.Activities
{
    public abstract class Activity : IActivity
    {
        protected IServiceProvider ServiceProvider { get; }

        private ILoggerFactory _loggerFactory;
        protected ILoggerFactory LoggerFactory => LazyGetRequiredService(ref _loggerFactory);

        protected ILogger Logger => _lazyLogger.Value;

        private Lazy<ILogger> _lazyLogger =>
            new Lazy<ILogger>(() => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance, true);

        private IWorkflowExpressionEvaluator _expressionEvaluator;
        protected IWorkflowExpressionEvaluator ExpressionEvaluator => LazyGetRequiredService(ref _expressionEvaluator);

        private IStringLocalizer _localizer;
        protected IStringLocalizer T => LazyGetRequiredService(ref _localizer);

        public virtual string TypeName => GetType().Name;
        public abstract LocalizedString Category { get; }
        public virtual JObject Properties { get; set; }

        public virtual bool HasEditor => true;

        protected Activity(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public abstract IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext);

        public virtual Task<bool> CanExecuteAsync(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Task.FromResult(CanExecute(workflowContext, activityContext));
        }

        public virtual bool CanExecute(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return true;
        }

        public virtual Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Task.FromResult(Execute(workflowContext, activityContext));
        }

        public virtual ActivityExecutionResult Execute(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Noop();
        }

        public virtual Task<ActivityExecutionResult> ResumeAsync(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Task.FromResult(Resume(workflowContext, activityContext));
        }

        public virtual ActivityExecutionResult Resume(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Noop();
        }

        public virtual Task OnInputReceivedAsync(WorkflowExecutionContext workflowContext,
            IDictionary<string, object> input)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnWorkflowStartingAsync(WorkflowExecutionContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public virtual Task OnWorkflowStartedAsync(WorkflowExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnWorkflowResumingAsync(WorkflowExecutionContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public virtual Task OnWorkflowResumedAsync(WorkflowExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnActivityExecutingAsync(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public virtual Task OnActivityExecutedAsync(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Task.CompletedTask;
        }

        protected Outcome Outcome(LocalizedString name)
        {
            return new Outcome(name);
        }

        protected IEnumerable<Outcome> Outcomes(params LocalizedString[] names)
        {
            return names.Select(x => new Outcome(x));
        }

        protected IEnumerable<Outcome> Outcomes(IEnumerable<LocalizedString> names)
        {
            return names.Select(x => new Outcome(x));
        }

        protected ActivityExecutionResult Outcomes(params string[] names)
        {
            return ActivityExecutionResult.FromOutcomes(names);
        }

        protected ActivityExecutionResult Outcomes(IEnumerable<string> names)
        {
            return ActivityExecutionResult.FromOutcomes(names);
        }

        protected ActivityExecutionResult Halt()
        {
            return ActivityExecutionResult.Halt();
        }

        protected ActivityExecutionResult Noop()
        {
            return ActivityExecutionResult.Noop();
        }

        protected virtual T GetProperty<T>(Func<T> defaultValue = null, [CallerMemberName] string name = null)
        {
            var item = Properties[name];
            return item != null ? item.ToObject<T>() : defaultValue != null ? defaultValue() : default;
        }

        protected virtual WorkflowExpression<TReturn> GetExpressionProperty<TReturn>(
            WorkflowExpression<TReturn> defaultValue = default, [CallerMemberName] string name = null)
        {
            var item = Properties[name];
            return item != null ? item.ToObject<WorkflowExpression<TReturn>>() : defaultValue;
        }

        protected virtual void SetProperty(object value, [CallerMemberName] string name = null)
        {
            Properties[name] = JToken.FromObject(value);
        }

        protected readonly object ServiceProviderLock = new object();

        protected TService LazyGetRequiredService<TService>(ref TService reference)
        {
            if (reference == null)
            {
                lock (ServiceProviderLock)
                {
                    if (reference == null)
                    {
                        reference = ServiceProvider.GetRequiredService<TService>();
                    }
                }
            }

            return reference;
        }
    }

    public static class ActivityExtensions
    {
        public static bool IsEvent(this IActivity activity)
        {
            return activity is IEvent;
        }
    }
}
