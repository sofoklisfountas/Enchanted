
// This script isn't attached to any GameObject but it is used by
// the PlayerDatabase script in building the PlayerList

public class PlayerData {

	//Variables Start___________________________________
	
	public int networkPlayer;
	
	public string playerName;
	
	public int playerScore;
	
	public string playerTeam;
	
	//Variables End_____________________________________
	
	
	public PlayerData Constructor ()
	{
		PlayerData capture = new PlayerData();	
		
		capture.networkPlayer = networkPlayer;
		
		capture.playerName = playerName;
		
		capture.playerScore = playerScore;
		
		capture.playerTeam = playerTeam;
		
		return capture;
	}
}
