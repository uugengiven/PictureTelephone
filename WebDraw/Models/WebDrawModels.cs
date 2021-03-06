﻿using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace WebDraw.Models
{
    public enum EntryType
    {
        Picture,
        Description
    }

    public class Chain
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("StartSuggestion")]
        public int StartID { get; set; }
        public bool Open { get; set; }

        public virtual StartSuggestion StartSuggestion { get; set; }
        public virtual ICollection<Entry> Entries { get; set; }
    }

    public class Entry
    {
        public int Id { get; set; }
        public int ChainId { get; set; }
        public EntryType entryType { get; set; }
        public string Value { get; set; }
        public bool Active { get; set; }
        public DateTime? created {get;set;}
        public DateTime? lastShown { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual Chain Chain { get; set; }

        public string PictureURL { 
            get
            {
                if (Value != null && entryType == EntryType.Picture)
                    return ConfigurationManager.AppSettings["AzurePrefix"] + Value;
                else
                    return Value;
            }
        }
    }

    public class StartSuggestion
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class User
    {
        public static object Identity { get; internal set; }
        [Key]
        public int Id { get; set; }
        public Guid? IdentityId { get; set; }
        public string VisibleName { get; set; }
    }

    public class WebDrawDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chain> Chains { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<StartSuggestion> StartSuggestions { get; set; }
    }


}