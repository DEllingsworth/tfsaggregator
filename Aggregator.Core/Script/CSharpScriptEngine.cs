﻿using Aggregator.Core.Interfaces;
using Aggregator.Core.Monitoring;
using Aggregator.Core.Script;

namespace Aggregator.Core
{
    using Microsoft.CSharp;

    public class CSharpScriptEngine : DotNetScriptEngine<CSharpCodeProvider>
    {
        public CSharpScriptEngine(IWorkItemRepository store, ILogEvents logger, bool debug)
            : base(store, logger, debug)
        {
        }

        protected override int LineOffset
        {
            get
            {
                return 10;
            }
        }

        protected override string WrapScript(string scriptName, string script)
        {
            return @"
namespace RESERVED
{
  using Microsoft.TeamFoundation.WorkItemTracking.Client;
  using Aggregator.Core;
  using Aggregator.Core.Extensions;
  using Aggregator.Core.Interfaces;
  using Aggregator.Core.Navigation;
  using Aggregator.Core.Monitoring;
  using System.Linq;

  public class Script_" + scriptName + @" : Aggregator.Core.Script.IDotNetScript
  {
    public object RunScript(Aggregator.Core.Interfaces.IWorkItemExposed self, Aggregator.Core.Interfaces.IWorkItemRepositoryExposed store, Aggregator.Core.Monitoring.IRuleLogger logger)
    {
" + script + @"
      return null;
    }
  }
}
";
        }
    }
}
