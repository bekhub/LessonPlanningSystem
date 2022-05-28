using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LPS.Desktop.Models;

namespace LPS.Desktop;

public class Database
{
    public IEnumerable<TodoItem> GetItems() => new[] {
        new TodoItem { Description = "Walk with dog" },
        new TodoItem { Description = "Buy some milk" },
        new TodoItem { Description = "Learn avalonia", IsChecked = true },
    };
}
