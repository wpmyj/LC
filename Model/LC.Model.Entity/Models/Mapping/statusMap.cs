using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class statusMap : EntityTypeConfiguration<status>
    {
        public statusMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.cat)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.description)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("statuses");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.cat).HasColumnName("cat");
            this.Property(t => t.description).HasColumnName("description");
        }
    }
}
