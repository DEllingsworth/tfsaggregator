﻿using Aggregator.Core.Interfaces;

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;

namespace Aggregator.Core.Facade
{
    public class NotificationWrapper : INotification
    {
        private readonly NotificationType notification;

        private readonly WorkItemChangedEvent eventArgs;

        public NotificationWrapper(NotificationType notification, WorkItemChangedEvent eventArgs)
        {
            this.notification = notification;
            this.eventArgs = eventArgs;
        }

        public int WorkItemId
        {
            get
            {
                return this.eventArgs.CoreFields.IntegerFields[0].NewValue;
            }
        }

        public string ProjectUri
        {
            get
            {
                // HACK
                return string.Format("vstfs:///Classification/TeamProject/{0}", this.eventArgs.ProjectNodeId);
            }
        }
    }
}
