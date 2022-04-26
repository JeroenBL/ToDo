using Microsoft.Extensions.DependencyInjection;
using Todo.Blazor.Data;
using Todo.Blazor.Services;
using Todo.DataAccess.Data;

namespace Todo.Blazor
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterMyServices(this IServiceCollection services)
        {
            services.AddEntityFrameworkSqlite().AddDbContext<ApplicationDbContext>();
            services.AddScoped<IToDoService, ToDoService>();

            return services;
        }
    }
}
