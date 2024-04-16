using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Dental_Manager.Models;

public partial class QlkrContext : DbContext
{
    public QlkrContext()
    {
    }

    public QlkrContext(DbContextOptions<QlkrContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentDetail> AppointmentDetails { get; set; }

    public virtual DbSet<Clinic> Clinics { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }

    public virtual DbSet<EmployeeScheduleDetail> EmployeeScheduleDetails { get; set; }

    public virtual DbSet<MedicalHistory> MedicalHistories { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceType> ServiceTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=KHANH-LAPTOP;Database=QLKR;Integrated Security=true;Encrypt=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointment");

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.AppointmentCreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("appointment_created_date");
            entity.Property(e => e.AppointmentDate)
                .HasColumnType("datetime")
                .HasColumnName("appointment_date");
            entity.Property(e => e.ClinicId).HasColumnName("clinic_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.IsBooking).HasColumnName("isBooking");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Clinic).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Clinics");

            entity.HasOne(d => d.Employee).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Employees");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Patients");
        });

        modelBuilder.Entity<AppointmentDetail>(entity =>
        {
            entity.HasKey(e => new { e.AppointmentId, e.EmployeeId, e.ServiceId });

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");

            entity.HasOne(d => d.EmployeeSchedule).WithMany(p => p.AppointmentDetails)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppointmentDetails_Appointment");

            entity.HasOne(d => d.Employee).WithMany(p => p.AppointmentDetails)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppointmentDetails_Employees");

            entity.HasOne(d => d.Service).WithMany(p => p.AppointmentDetails)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppointmentDetails_Services");
        });

        modelBuilder.Entity<Clinic>(entity =>
        {
            entity.HasKey(e => e.ClinicId).HasName("PK__Clinics__A0C8D19B45FB7B99");

            entity.Property(e => e.ClinicId).HasColumnName("clinic_id");
            entity.Property(e => e.ClinicAddress)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("clinic_address");
            entity.Property(e => e.ClinicName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("clinic_name");
            entity.Property(e => e.ClinicPhone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("clinic_phone");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__C52E0BA81B86A6C3");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ClinicId).HasColumnName("clinic_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("Created_by");
            entity.Property(e => e.EmployeeAddress)
                .HasMaxLength(255)
                .HasColumnName("employee_address");
            entity.Property(e => e.EmployeeEmail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("employee_email");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .HasColumnName("employee_name");
            entity.Property(e => e.EmployeePassword)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("employee_password");
            entity.Property(e => e.EmployeePhone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("employee_phone");
            entity.Property(e => e.LastFailedLoginAttempt).HasColumnType("datetime");
            entity.Property(e => e.RoleId).HasColumnName("Role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("Updated_by");

            entity.HasOne(d => d.Clinic).WithMany(p => p.Employees)
                .HasForeignKey(d => d.ClinicId)
                .HasConstraintName("FK__Employees__clini__4D94879B");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Employees_Role");
        });

        modelBuilder.Entity<EmployeeSchedule>(entity =>
        {
            entity.ToTable("Employee_Schedule");

            entity.Property(e => e.EmployeeScheduleId).HasColumnName("Employee_Schedule_id");
        });

        modelBuilder.Entity<EmployeeScheduleDetail>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeScheduleId, e.EmployeeId });

            entity.ToTable("Employee_Schedule_Detail");

            entity.Property(e => e.EmployeeScheduleId).HasColumnName("Employee_Schedule_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Date).HasColumnType("date");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeScheduleDetails)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Schedule_Detail_Employees");

            entity.HasOne(d => d.EmployeeSchedule).WithMany(p => p.EmployeeScheduleDetails)
                .HasForeignKey(d => d.EmployeeScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Schedule_Detail_Employee_Schedule");
        });

        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__MedicalH__096AA2E939D5B085");

            entity.ToTable("MedicalHistory");

            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.HistoryDetails)
                .HasColumnType("text")
                .HasColumnName("history_details");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalHistories)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MedicalHi__patie__4E88ABD4");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__4D5CE476DA4974A5");

            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.PatientAddress)
                .HasMaxLength(255)
                .HasColumnName("patient_address");
            entity.Property(e => e.PatientEmail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("patient_email");
            entity.Property(e => e.PatientName)
                .HasMaxLength(255)
                .HasColumnName("patient_name");
            entity.Property(e => e.PatientPassword)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("patient_password");
            entity.Property(e => e.PatientPhone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("patient_phone");
            entity.Property(e => e.RoleId).HasColumnName("Role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Patients)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Patients_Role");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EAF9C1D0FD");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.PaymentAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("payment_amount");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("date")
                .HasColumnName("payment_date");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("Role_id");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("Created_by");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("service_name");
            entity.Property(e => e.ServicePrice).HasColumnName("service_price");
            entity.Property(e => e.ServiceStatus).HasColumnName("service_status");
            entity.Property(e => e.ServiceTypeId).HasColumnName("Service_type_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("Updated_by");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.Services)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Services_Services");
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.ToTable("ServiceType");

            entity.Property(e => e.ServiceTypeId).HasColumnName("Service_type_id");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
