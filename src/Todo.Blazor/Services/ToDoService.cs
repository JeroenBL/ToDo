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
