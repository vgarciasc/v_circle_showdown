using UnityEngine;

public class PlayerInstance {
	public string joystick; //formato "_Jk" para joystick de numero k	
	public int joystickNum; //numero do joystick do jogador
	public int playerID; //ordem que o player apertou pra entrar no jogo
	public string name;
	public Color color; //sprite que ele escolheu pra jogar
	public int victories = 0;

	public PlayerInstance(string joystick, int joystickNum, int playerID, string name, Color color) {
		this.joystick = joystick;
		this.joystickNum = joystickNum;
		this.playerID = playerID;
		this.name = name;
		this.color = color;
	}
}
