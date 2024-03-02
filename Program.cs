
// Importing namespaces
using Microsoft.SemanticKernel;

namespace Semantic_Kernel_Local_LLM
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("\n======== LM Studio Phi 2 example ========\n");

            var userInput = Console.ReadLine();

            HttpClient client = new HttpClient(handler: new LocalHostServer("http://localhost:1234/v1/chat/completions"));

            Kernel kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(
                    deploymentName: "Phi2",
                    apiKey: "is not Required",
                    endpoint: "http://localhost:1234/v1/chat/completions",
                    httpClient: client)
                .Build();


            // Plugins > Prompts > AvailableIngredients   
            var plugInDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins\\RecipePrompts");
            kernel.ImportPluginFromPromptDirectory(plugInDirectory, "RecipePrompts");
            var AvailableIngredientsFunction = kernel.Plugins.GetFunction(pluginName: "RecipePrompts", functionName: "AvailableIngredients");

            var kernelArguments = new KernelArguments()
            {
                {
                    "input" , userInput
                }
            };

            var response = kernel.InvokeStreamingAsync(AvailableIngredientsFunction, kernelArguments);
            await foreach (var answer in response)
            {
                Console.Write(answer);
            }


        }
    }

    public class LocalHostServer(string url) : HttpClientHandler
    {
        private readonly Uri uri = new Uri(url);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.RequestUri = uri;
            return base.SendAsync(request, cancellationToken);
        }


    }

}