using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDraw.Models;

namespace WebDraw.Controllers
{
    public class DrawController : Controller
    {
        private WebDrawDbContext db = new WebDrawDbContext();
        // GET: Draw
        public ActionResult Index(int? id)
        {
            if (id != null)
            {
                Entry entry = new Entry();
                List<Entry> EntryList = db.Entries.Where(s => s.ChainId == id).ToList();
                Entry lastLink = EntryList.LastOrDefault();
                if (lastLink == null)
                {
                    var chainStart = db.Chains.Find(id);
                    entry.entryType = EntryType.Description;
                    entry.Value = chainStart.StartSuggestion.Description;
                    entry.ChainId = (int)id;

                }
                else
                {
                    entry = lastLink;
                }

                // lets be real, this should go to the approprate type of view based on entryType
                if (entry.entryType == EntryType.Description)
                {
                    // If the last one was a description, have them make a picture now
                    return View(entry);
                }
                else
                {
                    return View("Describe", entry);
                }
            }
            else
            {
                return View("Describe");
            }
            
        }

        [HttpPost]
        public ActionResult SaveImage(string img_save, string save_id)
        {
            if (img_save == null)
            {
                return View("Index"); // will currently error
            }

            var imageData = img_save.Replace(@"data:image/png;base64,", "");
            var imageName = Guid.NewGuid().ToString() + ".png";
            var filepath = Path.Combine(Server.MapPath("~/Images"), imageName);

            Entry entry = new Entry();
            entry.ChainId = Convert.ToInt32(save_id);
            entry.entryType = EntryType.Picture;
            entry.Value = imageName;
            //Guid UserGuid = Guid.Parse(User.Identity.ToString());
            // entry.UserId = db.Users.Where(u => u.IdentityId == UserGuid).SingleOrDefault().Id;
            entry.UserId = 1;

            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(imageData);
                    bw.Write(data);
                    bw.Close();
                }
            }
            db.Entries.Add(entry);
            db.SaveChanges();
            CloseChain(Convert.ToInt32(save_id));
            return RedirectToAction("Index", new { id = entry.ChainId });
        }

        [HttpPost]
        public ActionResult SaveDescription(string description, string save_id)
        {
            Entry entry = new Entry();
            entry.ChainId = Convert.ToInt32(save_id);
            entry.entryType = EntryType.Description;
            entry.Value = description;
            entry.UserId = 1;

            db.Entries.Add(entry);
            db.SaveChanges();

            CloseChain(Convert.ToInt32(save_id));
            return RedirectToAction("Index", new { id = entry.ChainId });
        }

        public ActionResult FullChain(int id)
        {
            var chain = db.Chains.Find(id);
            return View(chain);
        }

        public ActionResult StartChain(string desc)
        {
            // don't do anything with desc yet, but at some point use that to add new start suggestions

            Chain chain = new Chain();
            chain.Open = true;
            int total = db.StartSuggestions.Count();
            Random rnd = new Random();
            int toSkip = rnd.Next(total);
            var ss = db.StartSuggestions.OrderBy(o => o.Id).Skip(toSkip).Take(1);
            chain.StartID = ss.First().Id;

            db.Chains.Add(chain);
            db.SaveChanges();

            return RedirectToAction("Index", new { id = chain.Id });
        }

        [NonAction]
        public void CloseChain(int id)
        {
            Chain chain = db.Chains.Find(id);
            if (chain.Entries.Count() > 10)
            {
                chain.Open = false;
                db.SaveChanges();
            }
        }
    }
}