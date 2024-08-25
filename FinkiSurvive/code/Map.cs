using System;
using System.Collections.Generic;
using FinkiAdventureQuest.FinkiSurvive.code.Util;
using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code
{
	public partial class Map : Node2D
	{
		
		public static int WaveCount = 1;
		public static int Score = 0;
		public static int Grade = 5;
		public static readonly int WaveTime = 30;
		public static uint FrameCount;
		
		private readonly Random _rng = new();

		private Timer _timer;
		private int _timeSecs;
		
		private int _checkpointWave = 12;
		private bool _canGraduate;

		private Vector2[] _mobSpawnPoints = new Vector2[4];
		private Label _fps;
		private readonly List<string> _mobSceneNames = new ();
		private Label _gradeLabel;
		
		[Export] 
		private float _minMobSpawnRate = 0.5f;
		[Export]
		private int _targetWaveForMinSpawnRate = 10;
		
		private float _spawnRateDecrement;
		
		public override void _Ready()
		{
			_mobSceneNames.Add("mob_orc");
			_mobSceneNames.Add("mob_zombie");
			_mobSceneNames.Add("mob_knight");
			_gradeLabel = GetNode<Label>("UI/GradeCont/Label");
			_timer = GetNode<Timer>("GameTimer");
			_timeSecs = WaveTime;
			_fps = GetNode<Label>("UI/FpsCont/Label");
			_spawnRateDecrement = MobSpawner.CalcDecrementValue(_targetWaveForMinSpawnRate, _minMobSpawnRate);
			
			var player = GetNode<Player>("Player");
			player?.Connect(nameof(player.PlayerDied), new Callable(this, nameof(PlayerDeath)));
			
			UpdateTimerText(WaveTime / 60, WaveTime % 60);
			SetupUIListeners();
			
			GD.Print("TARGET SPAWN: " + MobSpawner.CalcDecrementValue(10,0.5f));
			
		}

		private void SetupUIListeners()
		{
			GetNode<MenuButton>("UI/GameInfoCont/MenuButton").GetPopup().PopupHide += ResumeGame;
			GetNode<Button>("DeathScreen/MarginContainer2/PlayAgainButton").Pressed += RestartGame;
			GetNode<Button>("DeathScreen/MarginContainer3/QuitButton").Pressed += QuitGame;
			GetNode<Button>("WinScreen/MarginContainer2/ContinueButton").Pressed += RestartGame;
			GetNode<Button>("WinScreen/MarginContainer3/GraduateButton").Pressed += QuitGame;
		}
		
		public override void _Process(double delta)
		{
			
			FrameCount++;
			if (FrameCount >= uint.MaxValue - 1) FrameCount = 0;
			_fps.Text = Engine.GetFramesPerSecond().ToString();
		}

		public void PauseSceneTree()
		{
			GetTree().Paused = true;
		}

		public void ResumeGame()
		{
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
		

		public void PlayerDeath()
		{
			if (_canGraduate)
			{
				GetNode<CanvasLayer>("WinScreen").Visible = true;
			}
			else
			{
				GetNode<CanvasLayer>("DeathScreen").Visible = true;
			}
			
			PauseSceneTree();
		}
		

		public void RestartGame()
		{
			GetTree().Paused = false;
			GetTree().ReloadCurrentScene();
			WaveCount = 1;
			Mob.StateValid = true;
			Score = 0;
			Grade = 5;
			_canGraduate = false;
		}
		

		public void QuitGame()
		{
			GetTree().ChangeSceneToFile("res://MainScene/main_menu.tscn");
		}

		public void UpdateScore()
		{
			Score++;
			GetNode<Label>("UI/ScoreMarginCont/Score").Text = "Score: " + Score;
			switch (Score)
			{
				case < 300:
					return;
				case >= 300 and < 400:
					Grade = 6;
					_gradeLabel.QueueFree();
					Color color = Color.FromHtml("#adebb0");
					_gradeLabel = LabelFactory.CreateLabel(color);
					GetNode<MarginContainer>("UI/GradeCont").AddChild(_gradeLabel);
					_canGraduate = true;
					
					break;
				case >= 400:
					Grade++;
					break;
			}
			
			_gradeLabel.Text = "Grade: " + Grade;
		}
		

		public void KillMob(Mob mob)
		{
			UpdateScore();
			
			mob.Death();
			
			var anim = mob.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			anim.Play("death");
			anim.AnimationFinished += () =>
			{
				mob.Visible = false;
				var mobsCont = GetNode<Node2D>("Mobs");
				mobsCont.GetChild<CharacterBody2D>(mob.GetIndex()).QueueFree();
			};

		}

		public void UpdateTimerText(int mins, int secs)
		{
			GetNode<Label>("UI/GameTimeMarginCont/GameTime").Text = $"{mins:D1}:{secs:D2}";
		}

		public void UpdateWaveLabel()
		{
			switch (WaveCount)
			{
				case 2:
					GetNode<Timer>("MobSpawnTimer").WaitTime = 1.1f;
					GD.Print("CHANGED TIME");
					break;
				case 4:
					GetNode<Timer>("MobSpawnTimer").WaitTime = 0.8f;
					GD.Print("CHANGED TIME");
					break;
				case > 9:
					GetNode<Timer>("MobSpawnTimer").WaitTime = 0.5f;
					GD.Print("CHANGED TIME");
					break;
				default:
				{
					if (WaveCount > 20)
					{
						GetNode<Timer>("MobSpawnTimer").WaitTime = 0.3f;
					}

					break;
				}
			}
			
			
			GetNode<Label>("UI/WaveNumCont/Label").Text = "Wave: " + WaveCount;
		}

		public void UpdateMenu()
		{
			if (WaveCount > 4) return;
			
			var menuPopup = GetNode<MenuButton>("UI/GameInfoCont/MenuButton").GetPopup();
			menuPopup.AddSeparator();
			if (WaveCount == 2)
			{
				Texture2D texture = ResourceLoader.Load<Texture2D>(ProjectPath.AssetsPath + "fx/slash6/image/slash6_00005.png");
				Image image = texture.GetImage();
				image.Resize(50, 50, Image.Interpolation.Lanczos);
				ImageTexture resizedTexture = ImageTexture.CreateFromImage(image);
				menuPopup.AddIconItem(resizedTexture,"Big damage, slow attack speed");
				
				
			} else if (WaveCount == 4)
			{
				Texture2D texture = ResourceLoader.Load<Texture2D>(ProjectPath.AssetsPath + "fx/slash5/image/slash5_1_00006.png");
				Image image = texture.GetImage();
				image.Resize(50, 50, Image.Interpolation.Lanczos);
				ImageTexture resizedTexture = ImageTexture.CreateFromImage(image);
				menuPopup.AddIconItem(resizedTexture,"Largest DPS (Damage Per Second");
			}
			
		}

		public void DisableCollisisonsPLayer()
		{
			var player = GetNode<Player>("Player");
			player.SetCollisionLayerValue(4,false);
			player.SetCollisionMaskValue(2,false);
		}

		public void EnableCollisisonsPLayer()
		{
			var player = GetNode<Player>("Player");
			player.SetCollisionLayerValue(4,true);
			player.SetCollisionMaskValue(2,true);
		}

		public void OnTimerTick()
		{
			if (--_timeSecs <= 0)
			{
				_timeSecs = WaveTime;
				WaveCount++;
				UpdateWaveLabel();
				UpdateMenu();
			}

			GetNode<Timer>("MobSpawnTimer").WaitTime -= _spawnRateDecrement;
			GD.Print(GetNode<Timer>("MobSpawnTimer").WaitTime);
			UpdateTimerText(_timeSecs / 60, _timeSecs % 60);
			
		}
		
		public int GetBiasedIndex()
		{
			if (WaveCount == 1) return 0;
			
			if (WaveCount == 2 )
			{
				return _rng.NextDouble() < 0.31 ? 1 : 0;
			}

			if (WaveCount > 2)
			{
				if (_rng.NextDouble() < 0.05)
				{
					return 2;
				}
				
				return _rng.NextDouble() < 0.7 ? 1 : 0;
			}

			return 0;
		}

		public Vector2 GetMobSpawnPos()
		{
			var player = GetNode<Player>("Player");
			var width = player.Position.X + ( (int) GetViewport().GetVisibleRect().Size[0] - player.Position.X);
			var height = player.Position.Y + (GetViewport().GetVisibleRect().Size[1] - player.Position.X);
			_mobSpawnPoints[0] = new Vector2(_rng.Next((int) width + 200, (int) width + 300), _rng.Next( (int)height - 150, (int)height)); // gore desno
			_mobSpawnPoints[1] = new Vector2(_rng.Next(200, 300), _rng.Next((int)height - 200,(int) height)); // gore levo
			_mobSpawnPoints[2] = new Vector2(_rng.Next(200, 300), _rng.Next((int)-height, -(int)height + 250)); // dolu levo
			_mobSpawnPoints[3] = new Vector2(_rng.Next(200,300), _rng.Next((int)-height, -(int)height + 250));// dolu desno
			
			// ova da sa optimizirat, samo na toj indeks so ke go izberite da presmetvit kaj da sa naogjat ne na site
			
			return _mobSpawnPoints[_rng.Next(_mobSpawnPoints.Length)];
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

