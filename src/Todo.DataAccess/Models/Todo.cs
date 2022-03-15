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