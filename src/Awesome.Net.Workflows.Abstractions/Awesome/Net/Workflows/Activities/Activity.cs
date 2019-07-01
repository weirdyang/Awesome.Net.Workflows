using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Localization;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using Volo.Abp;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;
using Volo.Abp.Users;

namespace Awesome.Net.Workflows.Activities
{
    public abstract class Activity : IActivity
    {
        public IServiceProvider ServiceProvider { get; set; }

        private ICurrentTenant _currentTenant;
        public ICurrentTenant CurrentTenant => LazyGetRequiredService(ref _currentTenant);

        private ICurrentUser _currentUser;
        public ICurrentUser CurrentUser => LazyGetRequiredService(ref _currentUser);

        private IClock _clock;
        public IClock Clock => LazyGetRequiredService(ref _clock);

        private IStringLocalizerFactory _stringLocalizerFactory;
        public IStringLocalizerFactory StringLocalizerFactory => LazyGetRequiredService(ref _stringLocalizerFactory);

        private IStringLocalizer _localizer;
        public IStringLocalizer L => _localizer ?? (_localizer = StringLocalizerFactory.Create(LocalizationResource));

        private ILoggerFactory _loggerFactory;
        public ILoggerFactory LoggerFactory => LazyGetRequiredService(ref _loggerFactory);

        protected ILogger Logger => _lazyLogger.Value;
        private Lazy<ILogger> _lazyLogger => new Lazy<ILogger>(() => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance, true);

        private Type _localizationResource = typeof(AwesomeNetWorkflowsResource);
        protected Type LocalizationResource
        {
            get => _localizationResource;
            set
            {
                _localizationResource = value;
                _localizer = null;
            }
        }

        private IWorkflowExpressionEvaluator _expressionEvaluator;
        public IWorkflowExpressionEvaluator ExpressionEvaluator => LazyGetRequiredService(ref _expressionEvaluator);

        public virtual string Name => GetType().Name;
        public abstract LocalizedString Category { get; }
        public JObject Properties { get; set; } = new JObject();
        public virtual bool HasEditor => true;

        public abstract IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext);

        public virtual Task<bool> CanExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Task.FromResult(CanExecute(workflowContext, activityContext));
        }

        public virtual bool CanExecute(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return true;
        }

        public virtual Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Task.FromResult(Execute(workflowContext, activityContext));
        }

        public virtual ActivityExecutionResult Execute(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Noop();
        }

        public virtual Task<ActivityExecutionResult> ResumeAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Task.FromResult(Resume(workflowContext, activityContext));
        }

        public virtual ActivityExecutionResult Resume(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Noop();
        }

        public virtual Task OnInputReceivedAsync(WorkflowExecutionContext workflowContext, IDictionary<string, object> input)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnWorkflowStartingAsync(WorkflowExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public virtual Task OnWorkflowStartedAsync(WorkflowExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnWorkflowResumingAsync(WorkflowExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public virtual Task OnWorkflowResumedAsync(WorkflowExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnActivityExecutingAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public virtual Task OnActivityExecutedAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
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

        protected readonly object ServiceProviderLock = new object();
        protected TService LazyGetRequiredService<TService>(ref TService reference)
        {
            Check.NotNull(ServiceProvider, nameof(ServiceProvider));
            if(reference == null)
            {
                lock(ServiceProviderLock)
                {
                    if(reference == null)
                    {
                        reference = ServiceProvider.GetRequiredService<TService>();
                    }
                }
            }

            return reference;
        }
    }
}