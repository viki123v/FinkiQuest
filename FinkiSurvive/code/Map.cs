using Godot;
using System;
using System.Collections.Generic;

namespace FinkiAdventureQuest.FinkiSurvive.code
{
	public partial class Map : Node2D
	{
		Random rng = new Random();
		public static int WaveTime = 30;
		public Timer Timer;
		public int TimeSecs;
		public static int WaveCount = 1;
		private int _score = 0;
		public int WinAtWave = 10;
		
		public static int FrameCount = 0;
		
		public Vector2[] MobSpawnPoints = new Vector2[4];

		private Label _fps;

		private List<string> _mobSceneNames = new ();
		
		public override void _Ready()
		{
			_mobSceneNames.Add("mob_orc");
			_mobSceneNames.Add("mob_zombie");
			_mobSceneNames.Add("mob_knight");
			
			Timer = GetNode<Timer>("GameTimer");
			TimeSecs = WaveTime;
			
			_fps = GetNode<Label>("UI/FpsCont/Label");
			
			var player = GetNode<Player>("Player");
			player?.Connect(nameof(player.PlayerDied), new Callable(this, nameof(PlayerDeath)));
			
			UpdateTimerText(WaveTime / 60, WaveTime % 60);
			SetupUIListeners();
			
		}

		private void SetupUIListeners()
		{
			GetNode<MenuButton>("UI/GameInfoCont/MenuButton").GetPopup().PopupHide += ResumeGame;
			GetNode<Button>("DeathScreen/MarginContainer2/PlayAgainButton").Pressed += RestartGame;
			GetNode<Button>("DeathScreen/MarginContainer3/QuitButton").Pressed += QuitGame;
			GetNode<Button>("WinScreen/MarginContainer2/ContinueButton").Pressed += ResumeGame;
			GetNode<Button>("WinScreen/MarginContainer3/GraduateButton").Pressed += QuitGame;
			GetNode<CanvasLayer>("DeathScreen").Visible = false;
			GetNode<CanvasLayer>("WinScreen").Visible = false;
		}
		
		public override void _Process(double delta)
		{
			FrameCount++;
			_fps.Text = Engine.GetFramesPerSecond().ToString();
		}

		public void PauseSceneTree()
		{
			GetTree().Paused = true;
		}

		public void ResumeGame()
		{
			GD.Print("CLICK");
			GetTree().Paused = false;
			Mob.StateValid = true;
			GetNode<CanvasLayer>("WinScreen").Visible = false;
		}

		public void ClearMobs()
		{
			var mobs = GetNode<Node2D>("Mobs");
			foreach (var mob in mobs.GetChildren())
				mob.QueueFree();
		}

		public void PauseGame()
		{
			GetNode<Timer>("MobSpawnTimer").Stop();
			GetNode<Timer>("GameTimer").Stop();
			Mob.StateValid = false;
		}

		public void Win()
		{
			GetNode<CanvasLayer>("WinScreen").Visible = true;
			PauseSceneTree();
			GD.Print("Game Over");
		}

		public void PlayerDeath()
		{
			GetNode<CanvasLayer>("DeathScreen").Visible = true;
			// ClearMobs();
			// PauseGame();
			PauseSceneTree();
		}
		

		public void RestartGame()
		{
			GetTree().ReloadCurrentScene();
			WaveCount = 1;
			Mob.StateValid = true;
			GD.Print("RESTART");
		}
		

		public void QuitGame()
		{
			GetTree().Quit();
		}
		

		public void KillMob(Mob mobDead)
		{
			_score++; // vo func ova
			
			mobDead.Death();
			
			GetNode<Label>("UI/ScoreMarginCont/Score").Text = "Score: " + _score;
			
			var anim = mobDead.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			anim.Play("death");
			anim.AnimationFinished += () =>
			{
				mobDead.Visible = false;
				var mobsCont = GetNode<Node2D>("Mobs");
				mobsCont.GetChild<CharacterBody2D>(mobDead.GetIndex()).QueueFree();
			};

		}

		public void UpdateTimerText(int mins, int secs)
		{
			GetNode<Label>("UI/GameTimeMarginCont/GameTime").Text = $"{mins:D1}:{secs:D2}";
		}

		public void UpdateWaveLabel()
		{
			GetNode<Label>("UI/WaveNumCont/Label").Text = "Wave: " + WaveCount;
		}

		public void UpdateMenu()
		{
			if (WaveCount > 3) return;
			
			var menuPopup = GetNode<MenuButton>("UI/GameInfoCont/MenuButton").GetPopup();
			menuPopup.AddSeparator();
			if (WaveCount == 2)
			{
				Texture2D texture = ResourceLoader.Load<Texture2D>(ProjectPath.AssetsPath + "fx/slash6/image/slash6_00005.png");
				Image image = texture.GetImage();
				image.Resize(50, 50, Image.Interpolation.Lanczos);
				ImageTexture resizedTexture = ImageTexture.CreateFromImage(image);
				menuPopup.AddIconItem(resizedTexture,"Big damage, slow attack speed");
				
				
			} else if (WaveCount == 3)
			{
				Texture2D texture = ResourceLoader.Load<Texture2D>(ProjectPath.AssetsPath + "fx/slash5/image/slash5_1_00006.png");
				Image image = texture.GetImage();
				image.Resize(50, 50, Image.Interpolation.Lanczos);
				ImageTexture resizedTexture = ImageTexture.CreateFromImage(image);
				menuPopup.AddIconItem(resizedTexture,"Largest DPS (Damage Per Second");
			}
			
		}
		
		public void OnTimerTick()
		{
			if (--TimeSecs <= 0)
			{
				TimeSecs = WaveTime;
				WaveCount++;
				UpdateWaveLabel();
				UpdateMenu();
				
				if (WaveCount % WinAtWave == 0)
					Win();
			}
			
			GetNode<Timer>("MobSpawnTimer").WaitTime -= 0.008f;
			UpdateTimerText(TimeSecs / 60, TimeSecs % 60);
			
		}
		
		public int GetBiasedIndex()
		{
			if (WaveCount == 1) return 0;
			
			if (WaveCount == 2 )
			{
				GetNode<Timer>("MobSpawnTimer").WaitTime = 1.1f;
				return rng.NextDouble() < 0.31 ? 1 : 0;
			}

			if (WaveCount > 2)
			{
				GetNode<Timer>("MobSpawnTimer").WaitTime = 0.8f;
				if (rng.NextDouble() < 0.05)
				{
					return 2;
				}
				
				return rng.NextDouble() < 0.7 ? 1 : 0;
			}

			return 0;
		}

		public Vector2 GetMobSpawnPos()
		{
			var player = GetNode<Player>("Player");
			var width = player.Position.X + ( (int) GetViewport().GetVisibleRect().Size[0] - player.Position.X);
			var height = player.Position.Y + (GetViewport().GetVisibleRect().Size[1] - player.Position.X);
			MobSpawnPoints[0] = new Vector2(rng.Next((int) width + 200, (int) width + 300), rng.Next( (int)height - 150, (int)height)); // gore desno
			MobSpawnPoints[1] = new Vector2(rng.Next((int) 200, (int)300), rng.Next((int)height - 200,(int) height)); // gore levo
			MobSpawnPoints[2] = new Vector2(rng.Next(200, 300), rng.Next((int)-height, -(int)height + 250)); // dolu levo
			MobSpawnPoints[3] = new Vector2(rng.Next(200,300), rng.Next((int)-height, -(int)height + 250));// dolu desno
			
			// ova da sa optimizirat, samo na toj indeks so ke go izberite da presmetvit kaj da sa naogjat ne na site
			
			return MobSpawnPoints[rng.Next(MobSpawnPoints.Length)];
		}
		
		public void GenerateMobs()
		{
			int idx = GetBiasedIndex();
			PackedScene mobScene = GD.Load<PackedScene>(ProjectPath.ScenesPath + _mobSceneNames[idx] +  ".tscn");
			var instance = mobScene.Instantiate<Mob>();
			
			instance.Connect(nameof(Mob.MobDamaged),new Callable(this,nameof(KillMob)));

			instance.Position = GetMobSpawnPos(); 
			GetNode<Node2D>("Mobs").AddChild(instance);
		}
	}
}

