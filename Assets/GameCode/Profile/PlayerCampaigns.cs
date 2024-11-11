
using Legacy.Database;

public class PlayerCampaigns
{
	private PlayerCampaign[] campaignsList =  new PlayerCampaign[0];
	public PlayerCampaigns()
	{
		BinaryCampaign[] dataList = Legacy.Database.Campaigns.Instance.Get();
		campaignsList = new PlayerCampaign[dataList.Length];
		for (int i = 0; i < campaignsList.Length; i++)
		{
			campaignsList[i] = new PlayerCampaign(dataList[i].index, 0, dataList[i].lives.free, 0, 0);
		}
		//campaignsList.
	}

	public PlayerCampaign GetCampaign(ushort cID)
	{
		foreach(var c in campaignsList)
		{
			if (c.CampaignID != cID) continue;
			return c;
		}
		return null;
	}

	public void Read(PlayerProfileInstance player)
	{
		
		foreach(var c in player.campaigns)
		{
			GetCampaign(ushort.Parse(c.Key)).Read(c.Value);
		}
	}

	public PlayerCampaign[] CampaignsList { get => campaignsList; }
}
