using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using src.DTOs.Requests;
using src.DTOs.User;
using src.Extensions;
using src.Model;
using src.Repositories;

namespace src.Controller.AdminController;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN")]
public class DepartmentController : BaseController<Department>
{
    private readonly IRepository<Department> _departmentRepository;

    public DepartmentController(IRepository<Department> departmentRepository, ICurrentUser currentUser) : base(departmentRepository, currentUser)
    {
        _departmentRepository = departmentRepository;
    }

    // GET: api/Department
    [HttpGet]
    public IActionResult Get(ODataQueryOptions<Department> queryOptions)
    {
        var queryable = _departmentRepository.Query()
            .Where(x => !x.IsDeleted);

        var (count, vm) = queryable.AppendQueryOptions(queryOptions);

        var response = new ODataResponse<DepartmentResponse>
        {
            Count = count,
            Value = vm.Select(x => new DepartmentResponse(x))
        };

        return Ok(response);
    }

    // GET: api/Department/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Department>> GetDepartment(long id)
    {
        
        var department = await _departmentRepository.Query()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (department == null)
            return NotFound();

        return Ok(new DepartmentDetailResponse(department));
    }

    // POST: api/Department
    [HttpPost]
    public async Task<ActionResult<Department>> CreateDepartment([FromBody] DepartmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = request.GetDepartment();
        _departmentRepository.Add(entity);
        await _departmentRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, new DepartmentResponse(entity));
       
    }

    // PUT: api/Department/5
    [HttpPut("{id}")]
    public async Task<ActionResult<Department>> UpdateDepartment(long id, [FromBody] DepartmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await _departmentRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
            return NotFound();

       

        // var check = await model.CheckCourseRuleAsync(_baseRepository, id);
        // if (!check.Success)
        //     return BadRequest(check.Value);


        request.ToEntity(entity);
        await _departmentRepository.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Department/5
    [HttpPut("delete")]
    public async Task<IActionResult> DeleteDepartment([FromBody] List<long> ids)
    {
        var entities = await _departmentRepository.Query()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync();

        if (!entities.Any())
            return NotFound();

        // Soft-delete: mark IsDeleted = true for each entity
        foreach (var department in entities)
        {
            department.IsDeleted = true;
            department.UpdatedAt = DateTime.UtcNow;
        }

        await _departmentRepository.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpPut("disable")]
    public async Task<IActionResult> Disable([FromBody] List<long> ids)
    {
        var entities = await _departmentRepository.Query()
            .Where(x => ids.Contains(x.Id) && x.IsActive)
            .ToListAsync();

        if (!entities.Any())
            return NotFound();

        foreach (var entity in entities)
        {
            entity.IsActive = false;
        }

        await _departmentRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("enable")]
    public async Task<IActionResult> Enable([FromBody] List<long> ids)
    {
        var entities = await _departmentRepository.Query()
            .Where(x => ids.Contains(x.Id) && !x.IsActive)
            .ToListAsync();

        if (!entities.Any())
            return NotFound();

        foreach (var entity in entities)
        {
            entity.IsActive = true;
        }

        await _departmentRepository.SaveChangesAsync();
        return NoContent();
    }
}
