using Knobs.Actuators;
namespace Knobs.ActuatorGroups {
	public class ActuatorGroup {
		private int id { get; }
		private string? processGroup;
		private List<Actuator> actuators;

		public ActuatorGroup(int id, string? processGroup, List<Actuator> actuators) {
			this.id = id;
			this.processGroup = processGroup;
			this.actuators = actuators;
		}

		public void AddActuator(Actuator actuator) {
			actuators.Add(actuator);
		}
	}
}