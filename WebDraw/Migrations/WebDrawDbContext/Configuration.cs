namespace WebDraw.Migrations.WebDrawDbContext
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebDraw.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WebDraw.Models.WebDrawDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\WebDrawDbContext";
        }

        protected override void Seed(WebDraw.Models.WebDrawDbContext context)
        {
            List<StartSuggestion> starts = new List<StartSuggestion>()
            {
                new StartSuggestion { Description = "Treasure Island" },
                new StartSuggestion { Description = "Best Van Damme Movie" },
                new StartSuggestion { Description = "TMNT" },
                new StartSuggestion { Description = "Count of Monte Cristo" },
                new StartSuggestion { Description = "Arbor Day" },
                new StartSuggestion { Description = "Sensible Chuckle" },
                new StartSuggestion { Description = "Show me them guns" },
                new StartSuggestion { Description = "Lookin good over there" },
                new StartSuggestion { Description = "I believe you gave me the wrong child" },
                new StartSuggestion { Description = "Too many cats" }
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
