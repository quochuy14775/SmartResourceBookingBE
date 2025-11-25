using src.Model;

namespace src.DTOs.User;

 public class DepartmentResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
       
        
        public DepartmentResponse(Department department)
        {
            
            Description = department.Description;
            Id = department.Id;
            Name = department.Name;
            IsActive = department.IsActive;
            CreatedAt = department.CreatedAt;
        }

    }
    
    public class DepartmentDetailResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        
       
        public DepartmentDetailResponse(Department department)
        {
            Id = department.Id;
            Name = department.Name;
            Description = department.Description;
        }
    }