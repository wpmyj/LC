using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LC.Model.Entity.Models.Mapping
{
    public class SysRoleMap : EntityTypeConfiguration<SysRole>
    {
        public SysRoleMap()
        {
            // Primary Key
            this.HasKey(t => t.RoleCode);

            // Properties
            this.Property(t => t.RoleCode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Name)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SysRole");
            this.Property(t => t.RoleCode).HasColumnName("RoleCode");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Stopped).HasColumnName("Stopped");

            // Relationships
            this.HasMany(t => t.SysUsers)
                .WithMany(t => t.SysRoles)
                .Map(m =>
                    {
                        m.ToTable("SysRoleUser");
                        m.MapLeftKey("RoleCode");
                        m.MapRightKey("UserCode");
                    });


        }
    }
}
