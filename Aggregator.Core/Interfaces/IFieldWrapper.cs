﻿using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Aggregator.Core.Interfaces
{
    /// <summary>
    /// Decouples Core from TFS Client API <see cref="Microsoft.TeamFoundation.WorkItemTracking.Client.Field"/>
    /// </summary>
    public interface IFieldWrapper
    {
        string Name { get; }

        string ReferenceName { get; }

        object Value { get; set; }

        FieldStatus Status { get; }

        object OriginalValue { get; }
    }
}
