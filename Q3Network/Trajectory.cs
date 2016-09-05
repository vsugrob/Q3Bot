//using System.Windows.Media.Media3D;

namespace Q3Network	// FIXIT: Move this out of here!
{
	public enum TrajectoryType {
		Stationary,
		Interpolate,				// non-parametric, but interpolate between snapshots
		Linear,
		LinearStop,
		Sine,						// value = base + sin( time / duration ) * delta
		Gravity
	}

	public class Trajectory {
		public TrajectoryType trType;
		public int trTime;
		public int trDuration;						// if non 0, trTime + trDuration = stop time
		public float [] trBase  = new float [3];
		public float [] trDelta = new float [3];	// velocity, etc
	}
}