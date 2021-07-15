using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ContextAwareServiceSample
{
    public class Worker : BackgroundService
    {
        readonly WorkExecutor _executor;
        readonly IDiagnosticContext _diagnosticContext;

        public Worker(WorkExecutor executor, IDiagnosticContext diagnosticContext)
        {
            _executor = executor;
            _diagnosticContext = diagnosticContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) {
                var outerOpId = Guid.NewGuid();
                using (_diagnosticContext.Begin(
                    "Worker executed operation {OperationId} at time {Time}",
                    outerOpId, DateTimeOffset.Now)) {
                    await _executor.DoWork(outerOpId);

                    var innerOpId = Guid.NewGuid();
                    using (_diagnosticContext.Begin(
                        "Worker executed inner operation {OperationId} at time {Time}",
                        innerOpId,
                        DateTimeOffset.Now)) {
                        await _executor.DoWork(innerOpId);
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    public class WorkExecutor
    {
        readonly IDiagnosticContext _diagnosticContext;

        public WorkExecutor(IDiagnosticContext diagnosticContext)
        {
            _diagnosticContext = diagnosticContext;
        }

        public async Task DoWork(Guid operationId)
        {
            var R = new Random();

            var operationNames = new[] { "SaveUser", "LoadUser", "AddRole", "RemoveRole", "DeactivateUser" };

            var randomOperation = operationNames[R.Next(0, operationNames.Length - 1)];

            _diagnosticContext.Set("OperationName", randomOperation);
            _diagnosticContext.Set("ThisOperationId", operationId);

            await Task.Delay(R.Next(100, 1000));

            Log.Information("Completed operation {OperationId}: {OperationName}", operationId,
                randomOperation);
        }
    }
}