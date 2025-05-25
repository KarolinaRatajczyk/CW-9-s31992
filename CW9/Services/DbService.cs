namespace CW9.Services;


public interface IDbService
{
    public Task<ICollection<StudentGetDto>> GetStudentsDetailsAsync();
    public Task<StudentGetDto> GetStudentDetailsByIdAsync(int studentId);
    public Task<StudentGetDto> CreateStudentAsync(StudentCreateDto studentData);
    public Task RemoveStudentAsync(int studentId);
    public Task UpdateStudentAsync(int studentId, StudentUpdateDto studentData);
}


public class DbService(AppDbContext data) : IDbService
{
    public async Task<ICollection<StudentGetDto>> GetStudentsDetailsAsync()
    {
        // Get all students from the DB and map them to a DTO
        return await data.Students.Select(st => new StudentGetDto
        {
            Id = st.Id,
            FirstName = st.FirstName,
            LastName = st.LastName,
            Age = st.Age,
            EntranceExamScore = st.EntranceExamScore,
            Groups = st.GroupAssignments.Select(ga => new StudentGetDtoGroup
            {
                Id = ga.GroupId,
                Name = ga.Group.Name,
            }).ToList()
        }).ToListAsync();
    }

    public async Task<StudentGetDto> GetStudentDetailsByIdAsync(int studentId)
    {
        // Try to get and map a student to a DTO
        var result = await data.Students.Select(st => new StudentGetDto
        {
            Id = st.Id,
            FirstName = st.FirstName,
            LastName = st.LastName,
            Age = st.Age,
            EntranceExamScore = st.EntranceExamScore,
            Groups = st.GroupAssignments.Select(ga => new StudentGetDtoGroup
            {
                Id = ga.GroupId,
                Name = ga.Group.Name,
            }).ToList()
        }).FirstOrDefaultAsync(e => e.Id == studentId);

        // If the student does not exist, we have to send a notification about it to the controller.
        return result ?? throw new NotFoundException($"Student with id: {studentId} not found");
    }

    public async Task<StudentGetDto> CreateStudentAsync(StudentCreateDto studentData)
    {
        List<Group> groups = [];
        
        // At first, we have to check if the given groups exist in the DB
        if (studentData.Groups is not null && studentData.Groups.Count != 0)
        {
            foreach (var groupId in studentData.Groups)
            {
                var group = await data.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                if (group is null)
                {
                    throw new NotFoundException($"Group with id: {groupId} not found");
                }
                groups.Add(group);
            }
        }
        
        var student = new Student
        {
            FirstName = studentData.FirstName,
            LastName = studentData.LastName,
            Age = studentData.Age,
            EntranceExamScore = studentData.EntranceExamScore,
            GroupAssignments = (studentData.Groups ?? []).Select(groupId => new GroupAssignment
            {
                GroupId = groupId,
            }).ToList()
        };

        // Add new student to the db context, and save all changes.
        await data.Students.AddAsync(student);
        await data.SaveChangesAsync();

        
        // Return created records to the controller.
        return new StudentGetDto
        {
            Id = student.Id,
            FirstName = studentData.FirstName,
            LastName = studentData.LastName,
            Age = studentData.Age,
            EntranceExamScore = studentData.EntranceExamScore,
            Groups = groups.Select(group => new StudentGetDtoGroup
            {
                Id = group.Id,
                Name = group.Name,
            }).ToList()
        };
    }

    public async Task RemoveStudentAsync(int studentId)
    {
        var affectedRows = await data.Students.Where(s => s.Id == studentId).ExecuteDeleteAsync();

        // If there are no changes, it means that, there is no record with this id.
        if (affectedRows == 0)
        {
            throw new NotFoundException($"Student with id: {studentId} not found");
        }
    }

    public async Task UpdateStudentAsync(int studentId, StudentUpdateDto studentData)
    {
        var affectedRows = await data.Students.Where(e => e.Id == studentId).ExecuteUpdateAsync(
            setters => setters
                .SetProperty(e => e.FirstName, studentData.FirstName)
                .SetProperty(e => e.LastName, studentData.LastName)
                .SetProperty(e => e.Age, studentData.Age)
                .SetProperty(e => e.EntranceExamScore, studentData.EntranceExamScore)
        );
        
        // If there are no changes, it means that, there is no record with this id.
        if (affectedRows == 0)
        {
            throw new NotFoundException($"Student with id: {studentId} not found");
        }
    }
}