
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoFunctionApp
{
    public static class ApiFunctions
    {
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

            using (var ctx = new TodoContext(options))
            {
                await ctx.TodoItems.AddAsync(data);
                await ctx.SaveChangesAsync();
                return new NoContentResult();
            }
        }
    }
}
