﻿using UnityEngine;

public class PlayerInstance {
	public string joystick; //formato "_Jk" para joystick de numero k	
	public int joystickNum; //numero do joystick do jogador
	public int playerID; //ordem que o player apertou pra entrar no jogo
	public string name;
	public PlayerColor palette; //sprite que ele escolheu pra jogar

	public PlayerInstance(string joystick, int joystickNum, int playerID, string name, PlayerColor color) {
		this.joystick = joystick;
		this.joystickNum = joystickNum;
		this.playerID = playerID;
		this.name = name;
		this.palette = color;
	}
}
