using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Knobs.Controller
{
	public class ControllerConfiguration
	{
		[JsonPropertyName("actuator_groups")]
		public required List<ActuatorGroupConfig> ActuatorGroups { get; set; }
		[JsonPropertyName("process_groups")]
		public required List<ProcessGroup> ProcessGroups { get; set; }
	}

	public class ActuatorGroupConfig
	{
		[JsonPropertyName("actuator_group_id")]
		public int Id { get; set; }
		[JsonPropertyName("process_group")]
		public string? ProcessGroup { get; set; }
		[JsonPropertyName("actuators")]
		public required List<ActuatorConfig> Actuators { get; set; }
	}

	public class ActuatorConfig
	{
		public int Id { get; set; }
		[JsonPropertyName("min_value")]
		public int MinValue { get; set; }
		[JsonPropertyName("max_value")]
		public int MaxValue { get; set; }
		[JsonPropertyName("physical_type")]
		public required string PhysicalType { get; set; }
		[JsonPropertyName("actuator_type")]
		public required string ActuatorType { get; set; }
	}

	public class ProcessGroup
	{
		[JsonPropertyName("group_name")]
		public required string GroupName { get; set; }
		[JsonPropertyName("processes")]
		public required List<Process> Processes { get; set; }
	}

	public class Process
	{
		[JsonPropertyName("process_name")]
		public required string ProcessName { get; set; }
	}
}