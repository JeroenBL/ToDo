# Todo

Blazor Todo app (start-to-finish)

## Stap 1 - .NET Core 3.1 class Library project

1. Maak een nieuwe solution in VS genaamd `Todo`.

2. Maak een nieuw VS project. .NET Core 3.1 class library met de naam: `Todo.DataAccess`
3. Delete `Class1.cs`
4. Voeg de volgende drie NUGET packages toe:
    - https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/5.0.9
    - https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/5.0.9
    - https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/5.0.9
5. Voeg twee nieuwe folders toe met de naam: `Models` en `Data`
6. Maak -in de folder- `Models` de volgende CS class:

```csharp
using System;

namespace DataAccess.Models
{
    // De naam van de class is tevens de naam van de tabel in de database.
    // Hier moet je van te voren goed over nadenken.
    public class ToDo
    {
        // Het Id (mits exact zo geschreven) wordt de primary key in de database
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
```

7. Maak in de folder `Data` de volgende CS class:

```csharp
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ToDo> Todo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Locatie naar de DB. Nu Sqlite. Maar dit kan ook SQL of MongoDB zijn.
            // Het werkt allemaal op dezelfde wijze.
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var conn = $@"Data Source={baseDir}\\todo.db";
            optionsBuilder.UseSqlite(conn);
        }
    }
}
```

8. Open de `Package manager console` en type: `Add-Migration Initial`

9. Open de `Package manager console` en type: `Update-Database`.kLik

Als alles goed is zou je nu in de bin folder een `todo.db` moeten zien staan met daarin een tabel `Todo`

10. Download DBBrowser for Sqlite van: https://sqlitebrowser.org/ om te database te managen.

## Stap 2 - Toevoegen Blazor ServerSide App

1. Klik rechts op de solution en click op: `Add -> New project`.

2. Kies voor een: `Blazor ServerSide` of `Blazor Server App`.

3. Kies als naam voor het project: `Todo.Blazor`.

4. Bij `Additional information` selecteer:
    - .NET Core 3.1
    - Authentication: None
    - Configure for HTTPS

5. Zet de `Todo.Blazor` app als startup project.

6. Open de `Package manager console`.

7. Zorg dat je in de project root van `Todo.Blazor` staat en type: `dotnet watch run`

> dotnet watch run start de browser met de applicatie. Sluit de browser niet af!

8. Open `/Pages/_Host.cshtml`

9. Verander de `title` op regel 13 naar bv. `Todo`.

10. Save via `CTRL+S`.

11. Ga terug naar de browser en let de title van de browser tab.

# Stap 3 - Aanpassen counter

> In VS: Open de `Package manager console`. Vanuit project root van `Todo.Blazor` type: `dotnet watch run`

1. Open de browser en ga naar de `Counter` pagina.

> Op dit moment doet de counter nog weinig, maar we kunnen er een UI element aan toevoegen.

2. Ga naar VS en open `Pages/Counter.razor`

3. Voeg op regel 8 de volgende code toe:

```html
    <input @bind="incrementBy"/>
```

4. Voeg in het `@code block` de volgende code toe:

```csharp
    private int incrementBy;
```

5. In de functie `private void IncrementCount()` wijzig ` currentCount++;` in:

```charp
    currentCount+=incrementBy;
```

6. Save via `CTRL+S`.

7. Browse naar de `Counter` pagina en zie dat er een nieuw UI element is bijgekomen.

![counter](./assets/counter.png)

# Stap 4 - Toevoegen db crud methodes

1. Open vanuit VS het project `Todo.Blazor`.

2. Voeg een nieuwe folder `Services` toe.

3. Maak een nieuwe CS Interface class. `IToDoService`.

```csharp
using DataAccess.Models;
using System.Collections.Generic;

namespace Todo.Blazor.Services
{
    public interface IToDoService
    {
        //Create
        ToDo Create (ToDo todo);

        //Read
        ToDo Get(int id);

        //Update
        ToDo Update(ToDo todo);

        //Delete
        void Delete(int id);

        //List
        List<ToDo> ListAll();
    }
}
```

4. Maak een nieuwe CS class. `ToDoService`

5. Inherit van: `IToDoService`.

```csharp
using DataAccess.Data;
using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace Todo.Blazor.Services
{
    public class ToDoService : IToDoService
    {
        private readonly ApplicationDbContext _db;

        public ToDoService(ApplicationDbContext db)
        {
            _db = db;
        }

        public ToDo Create(ToDo todo)
        {
           var newTodo = _db.Todo.Add(todo);
            _db.SaveChanges();

            return newTodo.Entity;
        }

        public void Delete(int id)
        {
            var todo = _db.Todo.Find(id);
            if (todo != null)
            {
                _db.Todo.Remove(todo);
                _db.SaveChanges();
            };
        }

        public ToDo Get(int id)
        {
            return _db.Todo.Find(id);
        }

        public ToDo Update(ToDo todo)
        {
            var dbTodo = _db.Todo.Find(todo.Id);
            if (dbTodo != null)
            {
                dbTodo = todo;
                _db.SaveChanges();
            }

            return dbTodo;
        }

        public List<ToDo> ListAll()
        {
            return _db.Todo.ToList();
        }
    }
}
```

# Stap 5 - Setup Dependency Injection

1. Open vanuit VS het project `Todo.Blazor`.

2. Voeg de volgende twee NUGET packages toe aan het `Todo.Blazor` project.
    - https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/5.0.9
    - https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/5.0.9

> Zonder deze packages kun je de volgende stappen niet uitvoeren!

2. Open de file `Startup.cs`.

3. Op regel 32, voeg toe: `services.AddEntityFrameworkSqlite().AddDbContext<ApplicationDbContext>();`

4. Op regel 33, voeg toe: `services.AddScoped<IToDoService, ToDoService>();`

> Vergeet niet de using statements toe te voegen.

> Indien je een 'compile' error krijgt, sluit VS en start opnieuw!

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages();
    services.AddServerSideBlazor();
    services.AddSingleton<WeatherForecastService>();
    services.AddEntityFrameworkSqlite().AddDbContext<ApplicationDbContext>();
    services.AddScoped<IToDoService, ToDoService>();
}
```
