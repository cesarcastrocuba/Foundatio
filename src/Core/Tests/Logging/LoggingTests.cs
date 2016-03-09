﻿using System;
using System.Threading.Tasks;
using Foundatio.Extensions;
using Foundatio.Logging;
using Foundatio.Logging.Xunit;
using Foundatio.Utility;
using Xunit;
using Xunit.Abstractions;

namespace Foundatio.Tests.Logging {
    public class LoggingTests : TestWithLoggingBase {
        public LoggingTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task BeginScopeProperty() {
            var logger = Log.CreateLogger<LoggingTests>();
            using (logger.BeginScope(b => b.Property("prop1", "val1").Property("prop2", "val2")))
            using (logger.BeginScope(b => b.Property("prop1", "innerval1"))) {
                logger.Info("Hey {Stuff}!", "Eric");
                await BlahAsync(logger).AnyContext();
            }

            foreach (var entry in Log.LogEntries) {
                Assert.Equal(2, entry.Scopes.Length);

                Assert.Equal(2, entry.Properties.Count);
                Assert.Equal("innerval1", entry.Properties["prop1"]);
                Assert.Equal("val2", entry.Properties["prop2"]);
            }
        }

        [Fact]
        public void LogDelegate()
        {
            var logger = Log.CreateLogger<LoggingTests>();
            var name = "Tester";

            logger.Info(() => $"{name} at {DateTime.Now}.");
        }

        private Task BlahAsync(ILogger logger) {
            logger.Info("Task hello");

            return TaskHelper.Completed();
        }
    }
}
