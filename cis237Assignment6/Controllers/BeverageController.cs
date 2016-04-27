using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237Assignment6.Models;

namespace cis237Assignment6.Controllers
{
    [Authorize]
    public class BeverageController : Controller
    {
        private BeverageRGowanEntities db = new BeverageRGowanEntities();

        // GET: Beverage
        public ActionResult Index()
        {
            //Setup a variable to hold the cars data
            DbSet<Beverage> BeveragesToFilter = db.Beverages;

            //Setup some strings to hold the data that might be in the session.
            //If there is nothing in the session we can still use these variables
            //as a default value.
            string filterName = "";
            string filterPack = "";
            string filterMin = "";
            string filterMax = "";

            //Define a min and max for the price
            decimal min = 0m;
            decimal max = 999999m;


            //Check to see if there is a value in the session, and if there is, assign it
            //to the variable that we setup to hold the value
            if (Session["name"] != null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
                filterName = (string)Session["name"];
            }

            if (Session["pack"] != null && !String.IsNullOrWhiteSpace((string)Session["pack"]))
            {
                filterPack = (string)Session["pack"];
            }

            if (Session["min"] != null && !String.IsNullOrWhiteSpace((string)Session["min"]))
            {
                filterMin = (string)Session["min"];
                min = decimal.Parse(filterMin);
            }

            if (Session["max"] != null && !String.IsNullOrWhiteSpace((string)Session["max"]))
            {
                filterMax = (string)Session["max"];
                max = decimal.Parse(filterMax);
            }

            //Do the filter on the CarsToFilter Dataset. Use the where that we used before
            //when doing EF work, only this time send in more lambda expressions to narrow it
            //down further. Since we setup default values for each of the filter parameters,
            //min, max, filterName, and filterPack, we can count on this always running with no errors.
            IEnumerable<Beverage> filtered = BeveragesToFilter.Where(beverage => beverage.price >= min &&
                                                                  beverage.price <= max &&
                                                                  beverage.name.Contains(filterName) &&
                                                                  beverage.pack.Contains(filterPack));

            //Convert the database set to a list now that the query work is done on it.
            //The view is expecting a List, so we convert the database set to a list.
            IEnumerable<Beverage> finalFiltered = filtered.ToList();

            //Place the string representation of the values that are in the session into
            //the viewbag so that they can be retrived and displayed on the view.
            ViewBag.filterName = filterName;
            ViewBag.filterPack = filterPack;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;


            //Return the view with a filtered selection of the beverages.
            return View(finalFiltered);

            //This is what used to be returned before a filter was setup.
            //return View(db.Beverages.ToList());
        }

        // GET: Beverage/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: Beverage/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Beverage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Beverages.Add(beverage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(beverage);
        }

        // GET: Beverage/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: Beverage/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // POST: Beverage/Filter
        //We need to add the HttpPost so it limits the type of
        //requests it will handle to only POST. We can also specify
        //an Action Name if we don't want it to use the Method name
        //as the action name.
        [HttpPost, ActionName("Filter")]
        public ActionResult Filter()
        {
            //Get the form data that we sent out of the Request object.
            //The string that is used as a key to get the data matches the
            //name property of the form control.
            string name = Request.Form.Get("name");
            string pack = Request.Form.Get("pack");
            string min = Request.Form.Get("min");
            string max = Request.Form.Get("max");


            //Store the form data into the session so that it can be retrived later
            //on to filter the data.
            Session["name"] = name;
            Session["pack"] = pack;
            Session["min"] = min;
            Session["max"] = max;

            //Redirect the user to the index page. We will do the work of actually
            //filtering the list in the index method.
            return RedirectToAction("Index");
        }

    }
}
