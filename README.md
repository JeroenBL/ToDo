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

namespace Todo.DataAccess.Models
{
    // De naam van de class is tevens de naam van de tabel in de database.
    // Hier moet je van te voren goed over nadenken.
    public class ToDo
    {
        // Het Id (mits exact zo geschreven) wordt de primary key in de database
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DateCreated { get; set; }
        public string DateUpdated { get; set; }
    }
}
```

7. Maak in de folder `Data` de volgende CS class:

```csharp
using Todo.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Todo.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ToDo> Todo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Locatie naar de DB. Nu Sqlite. Maar dit kan ook SQL of MongoDB zijn.
            // Het werkt allemaal op dezelfde wijze.
            var conn = @"Data Source=c:\\temp\\Todo\\todo.db";
            optionsBuilder.UseSqlite(conn);
        }
    }
}
```

8. Open de `Package manager console` en type: `Add-Migration Initial`

9. Open de `Package manager console` en type: `Update-Database`.

Als alles goed is zou je nu in `c:\temp\Todo` een `todo.db` moeten zien staan met daarin een tabel `Todo`

10. Download DBBrowser for Sqlite van: https://sqlitebrowser.org/ of een Sqlite VSCode extension om te database te managen.

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

11. Ga terug naar de browser en let op de title van de browser tab.

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
using Todo.DataAccess.Models;
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
using Todo.DataAccess.Data;
using Todo.DataAccess.Models;
using System;
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
            todo.DateCreated = DateTime.Now.ToShortDateString();
            todo.DateUpdated = DateTime.Now.ToShortDateString();
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
                dbTodo.DateUpdated = DateTime.Now.ToShortDateString();
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

# Stap 6 - Voeg een record toe aan de Sqlite database

1. Open DBBrowser for Sqlite of execute via een VSCode extensions.

```sql
INSERT INTO Todo (Id,Title,Description,DateCreated,DateUpdated)
VALUES (1, 'MyFirstTodo', 'My very first todo', '16-03-2022', '16-03-2022');
```

# Stap 7 - Nieuwe razor pagina

> In VS: Open de `Package manager console`. Vanuit project root van `Todo.Blazor` type: `dotnet watch run`

1. Open VS. Maak in het project `Todo.Blazor` in de `Pages` folder een nieuwe folder met de naam: `Todo`.

2. Maak in de folder `Todo` een nieuw `Razor component` met de naam: `TodoList.razor`.

```html
@page "/todos"

@using DataAccess.Models;
@using Services;

@inject IToDoService _todoService

<h3>Todo's</h3>

<table class="table table-striped">
  <thead>
    <tr>
      <th scope="col">#</th>
      <th scope="col">Title</th>
      <th scope="col">Description</th>
      <th scope="col">DateCreated</th>
      <th scope="col">DateUpdated</th>
    </tr>
  </thead>
  <tbody>
    @if (!Todos.Any())
    {
        <tr>
            <th scope="row" colspan="5">No todo's are currently available</th>
            </tr>
        }

        else
        {
            foreach (var todo in Todos)
            {
                 <tr>
                    <th scope="row">@todo.Id</th>
                    <td>@todo.Title</td>
                    <td>@todo.Description</td>
                    <td>@todo.DateCreated</td>
                    <td>@todo.DateUpdated</td>
                </tr>
            }
        }
  </tbody>
</table>

@code {
    List<ToDo> Todos = new List<ToDo>();

    protected override async Task OnInitializedAsync()
    {
        Todos = _todoService.ListAll();
    }
}
```

3. Open file: `Shared/NavMenu.razor`.

4. In het code block van: `@NavMenuCssClass` voeg de volgende `NavLink` toe:

```html
<li class="nav-item px-3">
    <NavLink class="nav-link" href="todos">
        <span class="oi oi-task" aria-hidden="true"></span> Todo's
    </NavLink>
</li>
```

> Als alles goed is uitgevoerd zou je nu een tabel moeten zien met daarin de aangemaakte Todo in de database.

![todoList](./assets/todoList.png)

# Stap 8 - Voorbereiding TodoDetails razor page

1. Open file: `pages/Todo/TodoList.razor`.

2. Voeg toe op regel 7 de `navigionManager toe`.

```csharp
@inject NavigationManager _navigationManager
```

3. Pas regel 33 (binnen het `<tr>` block in de foreach) als volgt aan:

```html
<tr @onclick="() => RedirectTo(todo.Id)" class="cursor-pointer">
```

4. Voeg de volgende method toe op regel 61 (binnen het `@code` block):

```csharp
private void RedirectTo(int todoId)
{
    _navigationManager.NavigateTo($"/todo/details/{todoId}");
}
```

5. Open de file: `wwwroot/css/site.css`

6. Scroll naar het einde van de file en voeg toe op regel 184 de volgende css:

```css
.cursor-pointer {
    cursor: pointer;
}
```

![todoListCursorPointer](./assets/todoListCursorPointer.png)

![todoDetailsEmpty](./assets/TodoDetailsEmpty.png)

# Stap 9 - Implementatie TodoDetails razor page

1. Maak in de folder `Todo` een nieuw `Razor component` met de naam: `TodoDetails.razor`.

```html
@page "/todo/details/{Id:int}"

@using DataAccess.Models
@using Services

@inject IToDoService _todoService
@inject NavigationManager _navigationManager

<div class="container d-flex justify-content-center m-0 p-0">
    <EditForm Model="@Todo" class="col-sm-12 col-md-10 col-lg-8 p-0">

        <div class="form-group">
            <label for="title">Title</label>
            <InputText id="title" @bind-Value="@Todo.Title" class="form-control" />
        </div>

        <div class="form-group">
            <label for="description">Description</label>
            <InputTextArea id="description" @bind-Value="@Todo.Description" class="form-control" rows="5" />
        </div>

        <div class="form-group">
            <a href="/todos" class="btn btn-sm btn-secondary"><i class="fas fa-times pr-2"></i>Cancel</a>
        </div>
        @if (Id != null)
        {
            <hr />
            <label>Created on: @Todo.DateCreated</label> <br />
            <label>Updated on: @Todo.DateUpdated</label>
        }
    </EditForm>
</div>

@code {
    [Parameter]
    public int? Id { get; set; }
    public ToDo Todo = new ToDo();

    protected override async Task OnInitializedAsync()
    {
        if (Id != null)
        {
            Todo = _todoService.Get(Id.Value);
        }
    }
}
```

![todoDetails](./assets/todoDetails.png)

# Stap 10 - Configureer blazored toast

1. Open het project `Todo.Blazor`

2. Installeer nuget package: `Blazored.Toast` https://www.nuget.org/packages/Blazored.Toast/3.2.2

3. Open cs file `Startup.cs`

4. Binnen de method `ConfigureServices` voeg op regel 37 de volgende code toe:

```csharp
 services.AddBlazoredToast();
 ```

> Importeer de juiste using statements!

5. Open de file `_Imports.razor`

6. Voeg de volgende twee regels code toe:

```csharp
@using Blazored.Toast
@using Blazored.Toast.Services
```

7. Open de file `_Hosts.cshtml`

8. Binnen het `<head></head>` code block, voeg de volgende regel code toe:

```html
<link href="_content/Blazored.Toast/blazored-toast.min.css" rel="stylesheet" />
```

9. Open de file `Sharerd\MainLayout.razor`

10. Voeg toe op regel op regel 6 (direct onder het `<div class="sidebar">/div` element)

```html
<BlazoredToasts Position="ToastPosition.BottomRight"
                Timeout="10"/>
```

> Importeer de juiste using statements!