using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class centerMap : EntityTypeConfiguration<center>
    {
        public centerMap()
        {
            // Primary Key
            this.HasKey(t => t.center_id);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.address)
                .HasMaxLength(200);

            this.Property(t => t.phone)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("centers");
            this.Property(t => t.center_id).HasColumnName("center_id");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.address).HasColumnName("address");
            this.Property(t => t.phone).HasColumnName("phone");
            this.Property(t => t.consultant_id).HasColumnName("consultant_id");

            // Relationships
            this.HasOptional(t => t.consultants)
                .WithMany(t => t.centers)
                .HasForeignKey(d => d.consultant_id);

        }
    }
}
