using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDraw.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Drawing;
using System.Configuration;
using System.Drawing.Imaging;

namespace WebDraw.Controllers
{
    public class DrawController : Controller
    {
        private WebDrawDbContext db = new WebDrawDbContext();
        Random rnd = new Random();

        // GET: Draw
        [Authorize]
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
                // try to find a random one for the person to work on
                int uid = UserID();
                var potentialEntries = db.Entries.Where(e => e.Active == true && e.UserId != uid).ToList();
                if (potentialEntries.Count == 0)
                {
                    return RedirectToAction("StartChain");
                }
                else
                {
                    int total = potentialEntries.Count();
                    int skipTotal = rnd.Next(total);
                    var ChainID = potentialEntries.Skip(skipTotal).ToList().First().ChainId;
                    return RedirectToAction("Index", new { id = ChainID });
                }
            }
            
        }

        [HttpPost]
        [Authorize]
        public ActionResult SaveImage(string img_save, string save_id)
        {
            if (img_save == null)
            {
                return View("Index"); // will currently error
            }



            var imageData = img_save.Replace(@"data:image/png;base64,", "");
            var imageName = Guid.NewGuid().ToString() + ".jpg";
            var filepath = Path.Combine(Server.MapPath("~/Images"), imageName);
            int ChainID = Convert.ToInt32(save_id);

            Entry entry = new Entry();
            entry.ChainId = ChainID;
            entry.entryType = EntryType.Picture;
            entry.Value = imageName;
            entry.UserId = UserID();
            entry.Active = true;

            MemoryStream ms = new MemoryStream(Convert.FromBase64String(imageData));
            Image img = Image.FromStream(ms);


            using (var b = new Bitmap(img.Width, img.Height))
            {
                // This sets the background of the image to white
                b.SetResolution(img.HorizontalResolution, img.VerticalResolution);

                using (var g = Graphics.FromImage(b))
                {
                    g.Clear(Color.White);
                    g.DrawImageUnscaled(img, 0, 0);
                }
                uploadtoAzure(imageName, b);
            }
            

            foreach (var item in db.Entries.Where(e=> e.ChainId == ChainID))
            {
                item.Active = false;
            }
            db.Entries.Add(entry);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult SaveDescription(string description, string save_id)
        {
            Entry entry = new Entry();
            int ChainID = Convert.ToInt32(save_id);
            entry.ChainId = ChainID;
            entry.entryType = EntryType.Description;
            entry.Value = description;
            entry.UserId = UserID();
            entry.Active = true;

            foreach (var item in db.Entries.Where(e => e.ChainId == ChainID))
            {
                item.Active = false;
            }

            db.Entries.Add(entry);
            db.SaveChanges();

            if (db.Entries.Where(x => x.ChainId == ChainID).ToList().Count > 10)
            {
                CloseChain(ChainID);
            }
            return RedirectToAction("Index");
        }

        public ActionResult FullChain(int id)
        {
            var chain = db.Chains.Find(id);
            return View(chain);
        }

        [Authorize]
        public ActionResult StartChain(string desc)
        {
            // don't do anything with desc yet, but at some point use that to add new start suggestions

            Chain chain = new Chain();
            chain.Open = true;
            if (desc != null)
            {
                StartSuggestion ss = new StartSuggestion();
                ss.Description = desc;
                db.StartSuggestions.Add(ss);
                db.SaveChanges();
                chain.StartID = ss.Id;
            }
            else
            { 
                int total = db.StartSuggestions.Count();
                int toSkip = rnd.Next(total);
                var ss = db.StartSuggestions.OrderBy(o => o.Id).Skip(toSkip).Take(1);
                chain.StartID = ss.First().Id;
            }

            db.Chains.Add(chain);
            db.SaveChanges();

            return RedirectToAction("Index", new { id = chain.Id });
        }

        [NonAction]
        public void CloseChain(int id)
        {
            Chain chain = db.Chains.Find(id);
            chain.Open = false;
            foreach (var entry in chain.Entries)
            {
                entry.Active = false;
            }
            db.SaveChanges();
        }

        [NonAction]
        public int UserID()
        {
            Guid userGuid;
            if (Guid.TryParse(User.Identity.GetUserId(), out userGuid))
            {
                var user = db.Users.Where(u => u.IdentityId == userGuid).SingleOrDefault();
                if (user == null)
                {
                    // valid guid but no user in the table? that means a user should be created!
                    User newUser = new Models.User();
                    newUser.IdentityId = userGuid;
                    newUser.VisibleName = User.Identity.Name;
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    return newUser.Id;
                }
                else
                {
                    return user.Id;
                }
            }
            else
            {
                return 1; // default user, will be impossible to hit once I turn on authentication (supposedly)
            }
        }

        private void uploadtoAzure(string filename, Image upload)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("images");
            // Retrieve reference to a blob named "someimage.jpg".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{filename}");
            // Create or overwrite the "someimage.jpg" blob with contents from an upload stream.
            blockBlob.UploadFromStream(ToStream(upload, ImageFormat.Jpeg));
        }
        private void deletefromAzure(string filename)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("images");
            // Retrieve reference to a blob named "someimage.jpg".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{filename}");
            // Create or overwrite the "someimage.jpg" blob with contents from an upload stream.
            if (blockBlob.Exists())
            {
                blockBlob.Delete();
            }
        }

        private Stream ToStream(Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

    }
}