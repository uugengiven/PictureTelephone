using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebDraw.Models;

namespace WebDraw.Controllers
{
    public class ChainsController : Controller
    {
        private WebDrawDbContext db = new WebDrawDbContext();

        // GET: Chains
        public ActionResult Index()
        {
            var chains = db.Chains.Include(c => c.StartSuggestion).Include(c => c.Entries).OrderBy(c => c.Id);
            return View(chains.ToList());
        }

        // GET: Chains/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chain chain = db.Chains.Find(id);
            if (chain == null)
            {
                return HttpNotFound();
            }
            return View(chain);
        }

        // GET: Chains/Create
        public ActionResult Create()
        {
            ViewBag.StartID = new SelectList(db.StartSuggestions, "Id", "Description");
            return View();
        }

        // POST: Chains/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StartID")] Chain chain)
        {
            if (ModelState.IsValid)
            {
                db.Chains.Add(chain);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StartID = new SelectList(db.StartSuggestions, "Id", "Description", chain.StartID);
            return View(chain);
        }

        // GET: Chains/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chain chain = db.Chains.Find(id);
            if (chain == null)
            {
                return HttpNotFound();
            }
            ViewBag.StartID = new SelectList(db.StartSuggestions, "Id", "Description", chain.StartID);
            return View(chain);
        }

        // POST: Chains/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StartID")] Chain chain)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chain).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StartID = new SelectList(db.StartSuggestions, "Id", "Description", chain.StartID);
            return View(chain);
        }

        // GET: Chains/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chain chain = db.Chains.Find(id);
            if (chain == null)
            {
                return HttpNotFound();
            }
            return View(chain);
        }

        // POST: Chains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Chain chain = db.Chains.Find(id);
            db.Chains.Remove(chain);
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
