namespace Gresst.Application.DTOs;

/// <summary>
/// Represents a process that contains subprocesses and/or tasks
/// </summary>
public class ProcessDto
{
    public string Id { get; set; } = string.Empty;
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
    public string Id { get; set; } = string.Empty;
    public string ProcessId { get; set; } = string.Empty;
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
    public string Id { get; set; } = string.Empty;
    public string? ProcessId { get; set; }
    public string? SubProcessId { get; set; }
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

