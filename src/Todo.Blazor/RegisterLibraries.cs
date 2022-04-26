using Blazored.Modal;
using Blazored.Toast;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.Blazor
{
    public static class RegisterLibraries
    {
        public static IServiceCollection RegisterCustomLibraries(this IServiceCollection services)
        {
            services.AddBlazoredToast();
            services.AddBlazoredModal();

            return services;
        }
    }
}
