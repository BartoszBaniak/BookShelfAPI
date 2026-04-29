using BookShelfAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShelfAPI.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title).IsRequired().HasMaxLength(Book.TitleMaxLength);
        builder.Property(b => b.Author).IsRequired().HasMaxLength(Book.AuthorMaxLength);
        builder.Property(b => b.Isbn).HasMaxLength(13);
        builder.HasIndex(b => b.Isbn).IsUnique();
        builder.Property(b => b.Description).HasMaxLength(Book.DescriptionMaxLength);
        builder.Property(b => b.Status).HasConversion<int>();
    }
}
