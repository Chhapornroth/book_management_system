using System;

namespace WindowsFormsApp.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateTime AddingDate { get; set; }
    }
}


