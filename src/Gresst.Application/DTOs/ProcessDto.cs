namespace Gresst.Application.DTOs;

/// <summary>
/// Represents a process that contains subprocesses and/or tasks
/// </summary>
public class ProcessDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public List<SubProcessDto> SubProcesses { get; set; } = new();
    public List<TaskDto> Tasks { get; set; } = new();
}

/// <summary>
/// Represents a subprocess that can contain tasks
/// </summary>
public class SubProcessDto
{
    public Guid Id { get; set; }
    public Guid ProcessId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public List<TaskDto> Tasks { get; set; } = new();
}

/// <summary>
/// Represents a task within a process or subprocess
/// </summary>
public class TaskDto
{
    public Guid Id { get; set; }
    public Guid? ProcessId { get; set; }
    public Guid? SubProcessId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    public string? AssignedTo { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

