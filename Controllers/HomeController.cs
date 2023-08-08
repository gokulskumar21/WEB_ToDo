using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Todo.Models;
using Todo.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace Todo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Action to display the initial page with the Todo list
        public IActionResult Index()
        {
            var todoListViewModel = GetAllTodos();
            return View(todoListViewModel);
        }

        // Action to populate the form with a ToDo details for updating
        public JsonResult PopulateForm(int id)
        {
            var todo = GetById(id);
            return Json(todo);
        }

        // Retrieves all Todo items and display them in a ViewModel
        internal TodoViewModel GetAllTodos()
        {
            List<TodoItem> todoList = new();

            using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = "SELECT * FROM todo";

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                todoList.Add(new TodoItem
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                            }
                        }
                        else
                        {
                            // Return an empty ViewModel if there is no items
                            return new TodoViewModel
                            {
                                TodoList = todoList
                            };
                        }
                    }
                }
            }

            // Return ViewModel containing the list of Todo items
            return new TodoViewModel
            {
                TodoList = todoList
            };
        }

        // Retrieves a specific Todo item by its ID
        internal TodoItem GetById(int id)
        {
            TodoItem todo = new();

            using (var connection = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();
                    tableCmd.CommandText = $"SELECT * FROM todo WHERE Id = '{id}'";

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            todo.Id = reader.GetInt32(0);
                            todo.Name = reader.GetString(1);
                        }
                        else
                        {
                            // Return an empty Todo item if not found
                            return todo;
                        }
                    }
                }
            }
            return todo;
        }

        // Action to update a Todo list
        public RedirectResult Update(TodoItem todo)
        {
            using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"UPDATE todo SET name = '{todo.Name}' WHERE Id = '{todo.Id}'";
                    try
                    {
                        tableCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return Redirect("https://localhost:5153/");
        }

        // Action to insert a new Todo item
        public RedirectResult Insert(TodoItem todo)
        {
            using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"INSERT INTO todo (name) VALUES ('{todo.Name}')";
                    try
                    {
                        tableCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return Redirect("http://localhost:5153/");
        }

        // Action to delete a Todo item
        public JsonResult Delete(int id)
        {
            using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"DELETE FROM todo WHERE Id = '{id}'";
                    tableCmd.ExecuteNonQuery();
                }
            }
            return Json(new { });
        }
    }
}
