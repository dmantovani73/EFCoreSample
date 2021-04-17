using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

var config = GetConfiguration();
var connectionString = GetConnectionString(config);

var optionsBuilder = new DbContextOptionsBuilder<UniversityContext>()
    .UseSqlite(connectionString);

using var db = new UniversityContext(optionsBuilder.Options);
//await Setup(db);
//await Update(db);
await RunQueries(db);


async Task Seed(UniversityContext db)
{
    var informatica = new Degree { Name = "Informatica" };
    var fisica = new Degree { Name = "Fisica" };

    var programmazioneI = new Course { Name = "Programmazione I", Degree = informatica };
    var programmazioneII = new Course { Name = "Programmazione II", Degree = informatica };
    var fisicaQuantistica = new Course { Name = "Fisica Quantistica", Degree = fisica };

    var marioRossi = new Student { FirstName = "Mario", LastName = "Rossi", Courses = new[] { programmazioneI, programmazioneII } };
    var marcoNeri = new Student { FirstName = "Marco", LastName = "Neri", Courses = new[] { programmazioneI } };

    await db.Degrees.AddRangeAsync(informatica, fisica);
    await db.Courses.AddRangeAsync(programmazioneI, programmazioneII, fisicaQuantistica);
    await db.Students.AddRangeAsync(marioRossi, marcoNeri);

    await db.SaveChangesAsync();
}

async Task Update(UniversityContext db)
{
    (await db.CourseStudents.FirstAsync(p => p.Student.LastName == "Rossi" && p.Course.Name == "Programmazione I")).DateOfRegistration = DateTime.Parse("2021-01-02");
    (await db.CourseStudents.FirstAsync(p => p.Student.LastName == "Rossi" && p.Course.Name == "Programmazione II")).DateOfRegistration = DateTime.Parse("2021-01-03");
    (await db.CourseStudents.FirstAsync(p => p.Student.LastName == "Neri" && p.Course.Name == "Programmazione I")).DateOfRegistration = DateTime.Parse("2021-01-02");

    await db.SaveChangesAsync();
}

async Task RunQueries(UniversityContext db)
{
    // Nome degli studenti iscritti a Programmazione I.
    (await db.Courses.Include(p => p.Students).FirstAsync(p => p.Name == "Programmazione I")).Students.Dump("Nome degli studenti iscritti a Programmazione I");

    // Per ogni corso, nome e cognome degli studenti iscritti.
    (
        from c in db.Courses
        from s in c.Students
        select new
        {
            Course = c.Name,
            Student = s.FirstName + " " + s.LastName,
        }
    ).Dump("Per ogni corso, nome e cognome degli studenti iscritti");

    // Numero di iscritti ad ogni corso di laurea.
    var query =
        from d in db.Degrees
        orderby d.Name
        select new
        {
            d.Name,
            NumberOfStudents = d.Courses.Select(c => c.Students.Count()).Sum(),
        };
    query.Dump("Numero di iscritti ad ogni corso di laurea");

    Console.WriteLine($"\nSQL:\n{query.ToQueryString()}");
}


static IConfigurationRoot GetConfiguration()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var builder = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

    return builder.Build();
}

static string GetConnectionString(IConfigurationRoot configuration) => configuration.GetConnectionString("(default)");

static class Extensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var i in source ?? Enumerable.Empty<T>())
        {
            action?.Invoke(i);
        }
    }

    public static void Dump<T>(this IEnumerable<T> source, string label = null)
    {
        if (!string.IsNullOrEmpty(label))
        {
            Console.WriteLine();
            Console.WriteLine(label);
            Console.WriteLine("------------");
        }

        ForEach(source, i => Console.WriteLine(i));
    }
}

class UniversityContext : DbContext
{
    public UniversityContext()
        : base()
    { }

    public UniversityContext(DbContextOptions<UniversityContext> options)
        : base(options)
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=University.db;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CourseStudent>()
            .HasKey(nameof(CourseStudent.CourseId), nameof(CourseStudent.StudentId));

        modelBuilder.Entity<Course>()
            .HasMany(p => p.Students)
            .WithMany(p => p.Courses)
            .UsingEntity<CourseStudent>(
                cs => cs.HasOne(p => p.Student).WithMany(p => p.CourseStudents),
                cs => cs.HasOne(p => p.Course).WithMany(p => p.CourseStudents)
            );
    }

    public DbSet<Student> Students { get; set; }

    public DbSet<Degree> Degrees { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<CourseStudent> CourseStudents { get; set; }
}

class Student
{
    public Student()
    {
        Courses = new List<Course>();
        CourseStudents = new List<CourseStudent>();
    }

    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public ICollection<Course> Courses { get; set; }

    public ICollection<CourseStudent> CourseStudents { get; set; }

    public override string ToString() => new { Id, FirstName, LastName }.ToString();
}

class Course
{
    public Course()
    {
        Students = new List<Student>();
        CourseStudents = new List<CourseStudent>();
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public Degree Degree { get; set; }

    public ICollection<Student> Students { get; set; }

    public ICollection<CourseStudent> CourseStudents { get; set; }

    public override string ToString() => new { Id, Name }.ToString();
}

class Degree
{
    public Degree()
    {
        Courses = new List<Course>();
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<Course> Courses { get; set; }

    public override string ToString() => $"Id: {Id}, Name: {Name}";
}

class CourseStudent
{
    public int CourseId { get; set; }

    public int StudentId { get; set; }

    public Course Course { get; set; }

    public Student Student { get; set; }

    public DateTime DateOfRegistration { get; set; }

    public override string ToString() => new { Course = Course?.Name, Student = $"{Student?.FirstName} {Student?.LastName}", DateOfRegistration }.ToString();
}