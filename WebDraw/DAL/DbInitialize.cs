using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebDraw.Models;

namespace WebDraw.DAL
{
    public class WebDrawDbInitialize : DropCreateDatabaseIfModelChanges<WebDrawDbContext>
    {
        protected override void Seed(WebDrawDbContext context)
        {
            // do some init stuff

            List<StartSuggestion> starts = new List<StartSuggestion>()
            {
                new StartSuggestion { Description = "A cat jumping over the moon" },
                new StartSuggestion { Description = "Robert Frost" },
                new StartSuggestion { Description = "Mario vs Sonic" }
            };

            foreach (var item in starts)
            {
                context.StartSuggestions.Add(item);
            }

            User user = new User();
            user.VisibleName = "Imma the default!";

            context.Users.Add(user);

            context.SaveChanges();
        }
    }
}