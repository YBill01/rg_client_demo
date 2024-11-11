
using Legacy.Database;
using UnityEngine;

public class PlayerCampaign
{
	private ushort campaignID;
	private int missionsDone;
	private byte hitPoints;
	private CampainDifficulty difficulty;
	private byte bestPassed;
	private bool done;
	private bool available;
	private BinaryCampaign data;
	public PlayerCampaign(ushort campaignID, byte missionsDone, byte hitPoints, CampainDifficulty difficult, byte bestPassed)
	{
		this.campaignID = campaignID;
		this.missionsDone = missionsDone;
		this.hitPoints = hitPoints;
		this.difficulty = difficult;
		this.bestPassed = bestPassed;

		Campaigns.Instance.Get(campaignID, out BinaryCampaign binaryCampaign);
		data = binaryCampaign;
	}

	public void BuyHP(byte count)
	{
		this.hitPoints = (byte)Mathf.Max(data.lives.free, this.hitPoints + count);
	}

	public void Read(PlayerBattleCampaign battleCampaign)
	{
		this.difficulty = battleCampaign.difficulty;
		this.bestPassed = battleCampaign.max_progress;
		this.missionsDone = battleCampaign.complete.Count;
		this.hitPoints = (byte)Mathf.Max((int)data.lives.free - (int)battleCampaign.attempts, 0);
		//battleCampaign.
	}

	public ushort CampaignID { get => campaignID; }
	public int MissionsDone { get => missionsDone;}
	public byte HitPoints { get => hitPoints;}
	public CampainDifficulty Difficult { get => difficulty;}
	public byte BestPassed { get => bestPassed; }
	public bool Done { get => done; }
	public bool Available { get => available; }
	public BinaryCampaign Data { get => data;}
}
