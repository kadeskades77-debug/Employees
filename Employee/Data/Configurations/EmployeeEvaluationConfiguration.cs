using EmployeeDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EmployeeEvaluationConfiguration
    : IEntityTypeConfiguration<EmployeeEvaluation>
{
    public void Configure(EntityTypeBuilder<EmployeeEvaluation> builder)
    {
        builder.ToTable("EmployeeEvaluations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ManagerId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(e => e.FinalScore)
            .HasPrecision(5, 2);

        builder.Property(e => e.Period)
            .IsRequired();

        builder.Property(e => e.Comments)
            .HasMaxLength(2000);

       
        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
