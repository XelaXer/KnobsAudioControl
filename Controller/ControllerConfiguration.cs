using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Knobs.Controller
{

	public class ControllerConfiguration
	{
		public List<ActuatorConfig> Actuators { get; set; }
		public List<ProcessGroup> ProcessGroups { get; set; }
	}

	public class ActuatorConfig
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string PhysicalType { get; set; }
		public int MinValue { get; set; }
		public int MaxValue { get; set; }
		public string ActuatorType { get; set; }
		public ActuatorTypeSettings ActuatorTypeSettings { get; set; }
	}

	public class ActuatorTypeSettings
	{
		public string ProcessGroup { get; set; }
		public string ProcessType { get; set; }
		public string ProcessName { get; set; }
		public string ProcessNickname { get; set; }
		public string AppGroup { get; set; }
		public string PauseKey { get; set; }
	}

	public class ProcessGroup
	{
		public string GroupName { get; set; }
		public List<Process> Processes { get; set; }
	}

	public class Process
	{
		public string ProcessName { get; set; }
	}
}