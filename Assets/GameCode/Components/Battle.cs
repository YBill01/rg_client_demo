using Legacy.Database;
using Unity.Entities;

namespace Legacy.Client
{

	public struct MinionClientBucket
	{
		public Entity entity;
		public MinionData minion;
		public EntityDatabase repl;
		public MinionBucketState state;
	}

	public struct EffectClientBucket
	{
		public Entity entity;
		public EffectData effect;
		public EntityDatabase database;
	}

    public class MiniProfile
    {
        public string name;
        public BattlePlayerProfileHero hero;
        public uint rating;

        public void Serialize(ref NetworkMessageRaw data)
        {
            /*NativeStringWriter.Write(new FixedString64(name), writer);
			config.Serialize(writer);
			writer.Write(rating);*/
        }

        public void Deserialize(ref NetworkMessageRaw data)
        {
            /*name = NativeStringWriter.Read(ref data).ToString();
			config.Deserialize(ref data);
			rating = data.ReadUInt();*/
        }
    }
}
