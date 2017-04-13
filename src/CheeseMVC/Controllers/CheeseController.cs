using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class CheeseController : Controller
    {
        private CheeseDbContext context;

        
        public CheeseController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        // Displays the current list of Cheeses including their Name, Description, and Category

        public IActionResult Index()
        {
            // Gets the complete list of cheeses from the Cheese Database and includes the Category
            IList<Cheese> cheeses = context.Cheeses.Include(c => c.Category).ToList();

            return View(cheeses);
        }

        // Displays inputs for the creation of a new Cheese.
        public IActionResult Add()
        {
            
            // Create ViewModel to allow for verification of Cheese Properties.
            AddCheeseViewModel addCheeseViewModel = new AddCheeseViewModel(context.Categories.ToList());

            return View(addCheeseViewModel);
        }

        // Receives Cheese Data from input and creates a new cheese to add to the database, if the data is valid.
        // If the data is not valid, the Input Fields are Displayed again, along with error messages.
        [HttpPost]
        public IActionResult Add(AddCheeseViewModel addCheeseViewModel)
        {
            if (ModelState.IsValid)
            {

                // Gets the Category selected for the cheese, based on the CategoryID from the selection.
                CheeseCategory newCheeseCategory = context.Categories.Single(c => c.ID == addCheeseViewModel.CategoryID);

                // Add the new cheese to my existing cheeses
                Cheese newCheese = new Cheese
                {
                    Name = addCheeseViewModel.Name,
                    Description = addCheeseViewModel.Description,
                    Category = newCheeseCategory
                };

                //Add Cheese to the database and save.
                context.Cheeses.Add(newCheese);
                context.SaveChanges();

                return Redirect("/Cheese");
            }

            return View(addCheeseViewModel);
        }

        // Displays View with a list of Cheeses that can be removed.
        public IActionResult Remove()
        {
            ViewBag.title = "Remove Cheeses";
            ViewBag.cheeses = context.Cheeses.ToList();
            return View();
        }


        [HttpPost]
        public IActionResult Remove(int[] cheeseIds)
        {
            foreach (int cheeseId in cheeseIds)
            {
                Cheese theCheese = context.Cheeses.Single(c => c.ID == cheeseId);
                context.Cheeses.Remove(theCheese);
            }

            context.SaveChanges();

            return Redirect("/");
        }
    }
}
