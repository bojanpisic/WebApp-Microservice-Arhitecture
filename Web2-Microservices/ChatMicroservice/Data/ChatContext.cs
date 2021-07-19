using ChatMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.Data
{
    public class ChatContext: DbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Conversation>()
               .HasMany(c => c.Messages)
               .WithOne(e => e.Conversation);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
