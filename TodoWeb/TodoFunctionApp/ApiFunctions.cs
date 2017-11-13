
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.ApplicationInsights;
using System;

namespace TodoFunctionApp
{
    public static class ApiFunctions
    {
        public static TelemetryClient telemetry = new TelemetryClient()
        {
            InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY")
        };

        [FunctionName("PostTodo")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            TodoItem data = JsonConvert.DeserializeObject<TodoItem>(requestBody);
            
            if (data == null)
            {
                return new BadRequestResult();
            }

            var options = new DbContextOptions<TodoContext>();

            var startTime = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            using (var ctx = new TodoContext(options))
            {
                telemetry.TrackDependency("SQLConnect", "Context", startTime, timer.Elapsed, true);
                startTime = DateTime.UtcNow;
                timer.Restart();
                await ctx.TodoItems.AddAsync(data);
                await ctx.SaveChangesAsync();
                telemetry.TrackDependency("SQLInsert", "Insert", startTime, timer.Elapsed, true);

                return new NoContentResult();
            }
        }
    }
}
