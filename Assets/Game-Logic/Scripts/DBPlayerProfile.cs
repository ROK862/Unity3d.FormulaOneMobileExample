using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DBPlayerProfile : MonoBehaviour
{
	public int ID;
	public string FirstName;
	public string LastName;
	public string TeamName;
	public string LastAccessed;
	public double AccessCount;
	public double AverageGameplayDuration;
	public string GoogleAccount;
	public string FacebookAccount;
	public string ImageURL;
	public string BillableAccount;
	public string AccountKey;
	public string AccountSecret;
	public DBPlayerProfile profile;
	public Text manageTeamTitle;

	public UIMenuNavigator initialNavigator;
	public string nextNodeLable = "Menu-Game-Manager";
	// Start is called before the first frame update
	public DBPlayerProfile(int _ID, string _FirstName, string _LastName, string _TeamName, string _LastAccessed, double _AccessCount, double _AverageGameplayDuration, string _GoogleAccount, string _FacebookAccount, string _ImageURL, string _BillableAccount, string _AccountKey, string _AccountSecret) {
		ID = _ID;
		FirstName = _FirstName;
		LastName = _LastName;
		TeamName = _TeamName;
		LastAccessed = _LastAccessed;
		AccessCount = _AccessCount;
		AverageGameplayDuration = _AverageGameplayDuration;
		GoogleAccount = _GoogleAccount;
		FacebookAccount = _FacebookAccount;
		ImageURL = _ImageURL;
		BillableAccount = _BillableAccount;
		AccountKey = _AccountKey;
		AccountSecret = _AccountSecret;
	}

	public DBPlayerProfile()
	{
		ID = -1;
	}

	private void Start()
    {
		InitializePlayer();
	}

	public void InitializePlayer() {
		profile = DBDriver.GetPlayerProfile();

		if (profile.ID != -1)
		{
			if (initialNavigator != null)
			{
				initialNavigator.SetNodeActive(nextNodeLable);
			}
			ID = profile.ID;
			FirstName = profile.FirstName;
			LastName = profile.LastName;
			TeamName = profile.TeamName;
			LastAccessed = profile.LastAccessed;
			AccessCount = profile.AccessCount;
			AverageGameplayDuration = profile.AverageGameplayDuration;
			GoogleAccount = profile.GoogleAccount;
			FacebookAccount = profile.FacebookAccount;
			ImageURL = profile.ImageURL;
			BillableAccount = profile.BillableAccount;
			AccountKey = profile.AccountKey;
			AccountSecret = profile.AccountSecret;

			if (manageTeamTitle)
				manageTeamTitle.text = "MANAGE " + TeamName.ToUpper();
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
