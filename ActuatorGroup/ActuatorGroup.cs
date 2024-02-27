using Knobs.Actuators;
using IHID.HIDDevice;
namespace Knobs.ActuatorGroups {
	public class ActuatorGroup {
		private int Id { get; }
		private string Type { get; }
		private string? ProcessGroup;
		private List<Actuator> Actuators;
		public Dictionary<string, List<Actuator>> MActuatorListByType;
		public ActuatorGroup(int id, string type, string? processGroup, List<Actuator> actuators) 
		{
			Id = id;
			Type = type;
			ProcessGroup = processGroup;
			Actuators = actuators;
			MActuatorListByType = new Dictionary<string, List<Actuator>>();
			
			foreach (var actuator in Actuators)
			{
				int actuatorId = actuator.GetId();
				string actuatorType = actuator.GetActuatorType();

				Console.WriteLine($"[ACTUATOR_GROUP] [INFO] Adding actuator with ID {actuatorId} and type {actuatorType} to actuator group with ID {Id}");
				
				if (MActuatorListByType.ContainsKey(actuatorType))
				{
					MActuatorListByType[actuatorType].Add(actuator);
				}
				else
				{
					MActuatorListByType.Add(actuatorType, new List<Actuator> { actuator });
				}
			}
		}

		public virtual void UpdateState(IHIDDevice device) {}

		public int GetId()
		{
			return Id;
		}

		public string GetGroupType()
		{
			return Type;
		}

		public void AddActuator(Actuator actuator)
		{
			Actuators.Add(actuator);
		}

		public IEnumerable<Actuator> GetActuators()
		{
			foreach (var actuator in Actuators)
			{
				yield return actuator;
			}
		}
	}
}