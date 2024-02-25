using Knobs.Actuators;
namespace Knobs.ActuatorGroups {
	public class ActuatorGroup {
		private int Id { get; }
		private string Type { get; }
		private string? ProcessGroup;
		private List<Actuator> Actuators;

		public ActuatorGroup(int id, string type, string? processGroup, List<Actuator> actuators) {
			Id = id;
			Type = type;
			ProcessGroup = processGroup;
			Actuators = actuators;
		}

		public void AddActuator(Actuator actuator) {
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