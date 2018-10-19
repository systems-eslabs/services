using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EmailService.DbEntities
{
    public partial class email_botContext : DbContext
    {
        public email_botContext()
        {
        }

        public email_botContext(DbContextOptions<email_botContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<Email> Email { get; set; }
        public virtual DbSet<Reply> Reply { get; set; }
        public virtual DbSet<Template> Template { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql(@"server=35.200.194.132;port=3306;user=root;
password=pop@123456;database=email_bot");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.ToTable("attachment");

                entity.HasIndex(e => e.TransactionId)
                    .HasName("transaction_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CloudUrl)
                    .HasColumnName("cloud_url")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.LocalUrl)
                    .IsRequired()
                    .HasColumnName("local_url")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.MailAttachmentId)
                    .IsRequired()
                    .HasColumnName("mail_attachment_id")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.Attachment)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_attachment_email_id");
            });

            modelBuilder.Entity<Email>(entity =>
            {
                entity.ToTable("email");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bcc)
                    .HasColumnName("bcc")
                    .HasColumnType("varchar(1000)");

                entity.Property(e => e.Cc)
                    .HasColumnName("cc")
                    .HasColumnType("varchar(1000)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("date")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.From)
                    .IsRequired()
                    .HasColumnName("from")
                    .HasColumnType("varchar(1000)");

                entity.Property(e => e.Mailid)
                    .IsRequired()
                    .HasColumnName("mailid")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasColumnName("subject")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.To)
                    .IsRequired()
                    .HasColumnName("to")
                    .HasColumnType("varchar(1000)");
            });

            modelBuilder.Entity<Reply>(entity =>
            {
                entity.ToTable("reply");

                entity.HasIndex(e => e.TransactionId)
                    .HasName("transaction_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bcc)
                    .HasColumnName("bcc")
                    .HasColumnType("varchar(1000)");

                entity.Property(e => e.Body)
                    .IsRequired()
                    .HasColumnName("body")
                    .HasColumnType("text");

                entity.Property(e => e.Cc)
                    .HasColumnName("cc")
                    .HasColumnType("varchar(1000)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("date")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.From)
                    .IsRequired()
                    .HasColumnName("from")
                    .HasColumnType("varchar(1000)");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasColumnName("subject")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.To)
                    .IsRequired()
                    .HasColumnName("to")
                    .HasColumnType("varchar(1000)");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.Reply)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_reply_email_id");
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.ToTable("template");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Template1)
                    .IsRequired()
                    .HasColumnName("template")
                    .HasColumnType("text");
            });
        }
    }
}
