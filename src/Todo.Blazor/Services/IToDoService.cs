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
