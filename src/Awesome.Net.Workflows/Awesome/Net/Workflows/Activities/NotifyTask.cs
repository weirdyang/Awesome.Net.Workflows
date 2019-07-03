//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Awesome.Net.Data;
//using Awesome.Net.Workflows.Models;
//using Awesome.Net.Workflows.Services;
//using Microsoft.Extensions.Localization;

//namespace Awesome.Net.Workflows.Activities
//{
//    public class NotifyTask : TaskActivity
//    {
//        private readonly INotifier _notifier;
//        private readonly IWorkflowExpressionEvaluator _expressionEvaluator;

//        public NotifyTask(INotifier notifier, IWorkflowExpressionEvaluator expressionvaluator)
//        {
//            _notifier = notifier;
//            _expressionEvaluator = expressionvaluator;
//        }

//        public override LocalizedString Category => T["UI"];

//        public NotifyType NotificationType
//        {
//            get => this.GetProperty<NotifyType>();
//            set => this.SetProperty(value);
//        }

//        public Expression<string> Message
//        {
//            get => this.GetProperty(() => new Expression<string>());
//            set => this.SetProperty(value);
//        }

//        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityExecutionContext activityContext)
//        {
//            return Outcomes(T["Done"]);
//        }

//        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityExecutionContext activityContext)
//        {
//            var message = await _expressionEvaluator.EvaluateAsync(Message, workflowContext);
//            _notifier.Add(NotificationType, new LocalizedHtmlString(nameof(NotifyTask), message));

//            return Outcomes("Done");
//        }
//    }
//}

