﻿namespace UnitTests.Core
{
    using System;
    using System.Linq;

    using Aggregator.Core;
    using Aggregator.Core.Configuration;
    using Aggregator.Core.Facade;
    using Aggregator.Core.Interfaces;

    using Microsoft.TeamFoundation.Server.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NSubstitute;

    [TestClass]
    public class PolicyScopeTests
    {
        [TestMethod]
        public void PolicyScopeMatchesProjectNameFromContextWithSingleProject()
        {
            Policy p = new Policy();
            p.Scope = new[] { new ProjectScope() { ProjectNames = new[] { "TestOne" } } };

            var context = Substitute.For<IRequestContext>();
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyScopeDoesNotMatchProjectNameFromContext()
        {
            Policy p = new Policy();
            p.Scope = new[] { new ProjectScope() { ProjectNames = new[] { "TestOne" } } };

            var context = Substitute.For<IRequestContext>();
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestTwo");

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PolicyScopeMatchesProjectNameFromContextWithMultipleProjects()
        {
            Policy p = new Policy();
            p.Scope = new[] { new ProjectScope() { ProjectNames = new[] { "TestOne", "TestTwo" } } };

            var context = Substitute.For<IRequestContext>();
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyScopeMatchesWildcardProjectFromContext()
        {
            Policy p = new Policy();
            p.Scope = new[] { new ProjectScope() { ProjectNames = new[] { "*" } } };

            var context = Substitute.For<IRequestContext>();
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyScopeMatchesWildcardCollectionFromContext()
        {
            Policy p = new Policy();
            p.Scope = new[] { new CollectionScope() { CollectionNames = new[] { "*" } } };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyScopeMatchesCollectionNameFromContextWithSingleCollection()
        {
            Policy p = new Policy();
            p.Scope = new[] { new CollectionScope() { CollectionNames = new[] { "DefaultCollection" } } };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyScopeMatchesCollectionNameFromContextWithMultipleCollection()
        {
            Policy p = new Policy();
            p.Scope = new[] { new CollectionScope() { CollectionNames = new[] { "DefaultCollection", "OtherCollection" } } };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyCombiningProjectAndCollectionScopeMatches()
        {
            Policy p = new Policy();
            p.Scope = new PolicyScope[]
                      {
                          new CollectionScope() { CollectionNames = new[] { "DefaultCollection", "OtherCollection" } },
                          new ProjectScope() { ProjectNames = new[] { "TestOne" } }
                      };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyMismatchingProjectAndCollectionScopeDoesNotMatches()
        {
            Policy p = new Policy();
            p.Scope = new PolicyScope[]
                      {
                          new CollectionScope() { CollectionNames = new[] { "DefaultCollection", "OtherCollection" } },
                          new ProjectScope() { ProjectNames = new[] { "TestTwo" } }
                      };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PolicyTemplateNameMatches()
        {
            Policy p = new Policy();
            p.Scope = new[]
            {
                new TemplateScope() { TemplateName = "Scrum" }
            };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");
            context.GetProjectProperties(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne"))
                .Returns(
                    new IProjectPropertyWrapper[]
                    {
                        new ProjectPropertyWrapper()
                        {
                            Name = "Process Template",
                            Value = "Scrum"
                        }
                    });

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyTemplateMinVersionMatchesWhenCurrentIsHigher()
        {
            Policy p = new Policy();
            p.Scope = new[]
            {
                new TemplateScope() { MinVersion = "3.0" }
            };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");
            context.GetCurrentProjectProcessVersion(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne"))
                .Returns(new ProcessTemplateVersionWrapper(Guid.Empty, 3, 5));

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyTemplateMinVersionMatchesWhenCurrentIsEqual()
        {
            Policy p = new Policy();
            p.Scope = new[]
                      {
                          new TemplateScope() { MinVersion = "3.0" }
                      };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");
            context.GetCurrentProjectProcessVersion(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne"))
                .Returns(new ProcessTemplateVersionWrapper(Guid.Empty, 3, 0));

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyTemplateMinVersionDoesNotMatchesWhenCurrentIsLower()
        {
            Policy p = new Policy();
            p.Scope = new[]
            {
                new TemplateScope() { MinVersion = "5.0" }
            };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");
            context.GetCurrentProjectProcessVersion(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne"))
                .Returns(new ProcessTemplateVersionWrapper(Guid.Empty, 3, 0));

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PolicyTemplateMaxVersionMatchesWhenCurrentIsLower()
        {
            Policy p = new Policy();
            p.Scope = new[]
            {
                new TemplateScope() { MaxVersion = "3.0" }
            };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");
            context.GetCurrentProjectProcessVersion(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne"))
                .Returns(new ProcessTemplateVersionWrapper(Guid.Empty, 2, 9));

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyTemplateMaxVersionMatchesWhenCurrentIsEqual()
        {
            Policy p = new Policy();
            p.Scope = new[]
            {
                new TemplateScope() { MaxVersion = "3.0" }
            };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");
            context.GetCurrentProjectProcessVersion(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne"))
                .Returns(new ProcessTemplateVersionWrapper(Guid.Empty, 3, 0));

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PolicyTemplateMaxVersionDoesNotMatchesWhenCurrentIsHigher()
        {
            Policy p = new Policy();
            p.Scope = new[]
            {
                new TemplateScope() { MinVersion = "5.0" }
            };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");
            context.GetCurrentProjectProcessVersion(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne"))
                .Returns(new ProcessTemplateVersionWrapper(Guid.Empty, 3, 0));

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PolicyTemplateVersionAndNameMatchTogetherOnly()
        {
            Policy p = new Policy();
            p.Scope = new[]
            {
                new TemplateScope()
                {
                    TemplateName = "Scrum",
                    MinVersion = "5.0",
                    MaxVersion = "5.5"
                }
            };

            var context = Substitute.For<IRequestContext>();
            context.CollectionName.Returns("DefaultCollection");
            context.GetProjectName(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne")).Returns("TestOne");
            context.GetCurrentProjectProcessVersion(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne"))
                .Returns(new ProcessTemplateVersionWrapper(Guid.Empty, 5, 3));
            context.GetProjectProperties(new Uri("http://localhost:8080/tfs/defaultcollection/TestOne"))
                .Returns(
                    new IProjectPropertyWrapper[]
                                {
                                    new ProjectPropertyWrapper()
                                    {
                                        Name = "Process Template",
                                        Value = "Scrum"
                                    }
                                });

            var notification = Substitute.For<INotification>();
            notification.ProjectUri.Returns("http://localhost:8080/tfs/defaultcollection/TestOne");

            bool result = p.Scope.All(s => s.Matches(context, notification));

            Assert.IsTrue(result);
        }
    }
}
