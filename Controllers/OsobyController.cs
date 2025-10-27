using System.Net;
using CRUD.DAL;
using CRUD.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Controllers
{
    public class OsobyController(OsobaContext context) : Controller
    {
        private readonly OsobaContext db = context;

        // GET: Osoby
        public ActionResult Index()
        {
            return View(db.Osoby.ToList());
        }

        // GET: Osoby/Details/5
        public ActionResult Details(int? id)
        {
			if (id == null)
			{
				return BadRequest();
			}
			Osoba osoba = db.Osoby.Find(id);
			if (osoba == null)
			{
				return NotFound();
			}
            return View(osoba);
        }

        // GET: Osoby/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Osoby/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("id,imie,nazwisko,waga,wzrostCM")] Osoba osoba)
        {
            if (ModelState.IsValid)
            {
                db.Osoby.Add(osoba);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(osoba);
        }

        // GET: Osoby/Edit/5
        public ActionResult Edit(int? id)
        {
			if (id == null)
			{
				return BadRequest();
			}
			Osoba osoba = db.Osoby.Find(id);
			if (osoba == null)
			{
				return NotFound();
			}
            return View(osoba);
        }

        // POST: Osoby/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        // GET: Osoby/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Osoba osoba = db.Osoby.Find(id);
            if (osoba == null)
            {
                return NotFound();
            }
            return View(osoba);
        }

        // POST: Osoby/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Osoba osoba = db.Osoby.Find(id);
            db.Osoby.Remove(osoba);
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
    }

    
 
}
