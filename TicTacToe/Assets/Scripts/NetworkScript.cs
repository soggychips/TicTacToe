﻿using UnityEngine;
using System.Collections;

public class NetworkScript : MonoBehaviour {

	private const string gameTypeName = "Jake_Cohen_TicTacToe";
	private const string roomName = "Game 1";

	private HostData[] hostList;

	private Engine engine;

	void Start(){
		engine = GameObject.Find ("Engine").GetComponent("Engine") as Engine;
	}

	public HostData[] HostList{
		get{return hostList;}
	}

	public void HostServer(){
		Network.InitializeServer(2,2500,!Network.HavePublicAddress());
		MasterServer.RegisterHost(gameTypeName,roomName);
		engine.GameName = roomName;
	}

	public void HostServer(string customRoomName){
		Network.InitializeServer(2,2500,!Network.HavePublicAddress());
		MasterServer.RegisterHost(gameTypeName,customRoomName);
		engine.GameName = customRoomName;
	}

	public void RefreshHostList(){
		Debug.Log ("Refreshing Host List...");
		MasterServer.RequestHostList(gameTypeName);
	}

	void OnMasterServerEvent(MasterServerEvent mse){
		if(mse == MasterServerEvent.HostListReceived){
			Debug.Log ("Host List Received");
			hostList = MasterServer.PollHostList();
		}
	}

	public void DisconnectPlayersAndCloseGame(){
		Network.Disconnect();
		if(Network.isServer){
			MasterServer.UnregisterHost();
		}
	}

	public void JoinHost(HostData hostData){
		Debug.Log ("Joining host "+hostData.gameName+"...");
		Network.Connect(hostData);
		engine.GameName = hostData.gameName;
	}

	void OnConnectedToServer(){
		Debug.Log ("Connected to: "+roomName);
		engine.JoinNetworkedGame();
	}

	void OnServerInitialized(){
		Debug.Log (roomName + " server initialized");
		engine.BeginNetworkedGame();
	}

	void OnPlayerDisconnected(){
		//engine.DestroyBoardAndPieces();
		if(Network.isClient)
			Debug.Log ("OnPlayerDisconnected called for client");
		else
			Debug.Log ("OnPlayerDisconnected called for server");

		engine.DisconnectProcedure();

		if(Network.isServer){
			MasterServer.UnregisterHost();
		}
	}

}
