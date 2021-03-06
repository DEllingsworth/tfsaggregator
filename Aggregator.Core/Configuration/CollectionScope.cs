﻿using System.Collections.Generic;
using System.Linq;

using Aggregator.Core.Extensions;
using Aggregator.Core.Interfaces;

namespace Aggregator.Core.Configuration
{
    /// <summary>
    /// Implements a <see cref="PolicyScope"/> that allows the user to bind to a specific Project collectionName
    /// </summary>
    /// <remarks>Combine with the ProjectScope to scope to a specific project or projects under a collection.</remarks>
    public class CollectionScope : PolicyScope
    {
        /// <summary>
        /// The list of collection names that should execute this policy
        /// </summary>
        public IEnumerable<string> CollectionNames { get; set; }

        public override bool Matches(IRequestContext requestContext, INotification notification)
        {
            return this.CollectionNames.Any(c => requestContext.CollectionName.SameAs(c));
        }
    }
}