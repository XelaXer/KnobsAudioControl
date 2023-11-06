using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Knobs.Controller
{

	public class ControllerConfiguration
	{
		[JsonPropertyName("actuators")]
		public required List<ActuatorConfig> Actuators { get; set; }
		[JsonPropertyName("process_groups")]
		public required List<ProcessGroup> ProcessGroups { get; set; }
	}

	public class ActuatorConfig
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		[JsonPropertyName("physical_type")]
		public required string PhysicalType { get; set; }
		[JsonPropertyName("min_value")]
		public int MinValue { get; set; }
		[JsonPropertyName("max_value")]
		public int MaxValue { get; set; }
		[JsonPropertyName("actuator_type")]
		public required string ActuatorType { get; set; }
		[JsonPropertyName("actuator_type_settings")]
		public required ActuatorTypeSettings ActuatorTypeSettings { get; set; }
	}

	public class ActuatorTypeSettings
	{
		[JsonPropertyName("process_group")]
		public string ProcessGroup { get; set; }
		[JsonPropertyName("process_type")]
		public string ProcessType { get; set; }
		[JsonPropertyName("process_name")]
		public string ProcessName { get; set; }
		[JsonPropertyName("process_nickname")]
		public string ProcessNickname { get; set; }
		[JsonPropertyName("app_group")]
		public string AppGroup { get; set; }
		[JsonPropertyName("pause_key")]
		public string PauseKey { get; set; }
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