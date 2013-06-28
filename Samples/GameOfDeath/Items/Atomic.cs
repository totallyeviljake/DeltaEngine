using DeltaEngine.Content;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;

namespace GameOfDeath.Items
{
	/// <summary>
	/// A big explosion lasting a while and killing everything in the area with huge damage.
	/// </summary>
	public class Atomic : Item
	{
		public Atomic()
			: base(
				ContentLoader.Load<Image>("Atomic"), ContentLoader.Load<Image>("RingExplosion"),
				ContentLoader.Load<Sound>("AtomicExplosion")) { }

		protected override float ImpactSize
		{
			get { return 0.175f; }
		}

		protected override float ImpactTime
		{
			get { return 1.0f; }
		}

		protected override float Damage
		{
			get { return 150; }
		}

		protected override float DamageInterval
		{
			get { return 0.5f; }
		}

		public override int Cost
		{
			get { return 50; }
		}
	}
}