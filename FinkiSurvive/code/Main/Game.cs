using System;
using FinkiAdventureQuest.FinkiSurvive.code.Util;
using FinkiAdventureQuest.FinkiSurvive.code.WaveStates;
using FinkiAdventureQuest.FinkiSurvive.Misc;
using FinkiAdventureQuest.MainScene;
using Godot;
using GameNames = FinkiAdventureQuest.MainScene.GameNames;

namespace FinkiAdventureQuest.FinkiSurvive.code.Main
{
	public partial class Game : Node2D
	{
		
		public static int WaveCount = 1;
		public static int Score = 0;
		public static int Grade = 5;
		public static int WaveTime = 30;

		public WaveState CurrentWaveState;
		
		private readonly Random _rng = new();

		private Timer _timer;
		private Timer _spacebarIconTimer;
		private int _timeSecs;
		
		private int _checkpointWave = 12;
		private bool _canGraduate;

	
		private Label _fps;
		
		private Label _gradeLabel;
		
		[Export] 
		private float _minMobSpawnRate = 0.5f;
		[Export]
		private int _targetWaveForMinSpawnRate = 10;
		
		private float _spawnRateDecrement;

		//[Signal]
		//public delegate void SendGameStatsEventHandler(int score, int grade, int waveCount);
		
		public override void _Ready()
		{
			_gradeLabel = GetNode<Label>("UI/GradeCont/Label");
			_timer = GetNode<Timer>("GameTimer");
			_timeSecs = WaveTime;
			_fps = GetNode<Label>("UI/FpsCont/Label");
			_spawnRateDecrement = MobSpawner.CalcDecrementValue(_targetWaveForMinSpawnRate, _minMobSpawnRate);
			
			var player = GetNode<Player>("Player");
			player?.Connect(nameof(player.PlayerDied), new Callable(this, nameof(PlayerDeath)));
			
			UpdateTimerText(WaveTime / 60, WaveTime % 60);
			SetupUIListeners();

			_spacebarIconTimer = new Timer();
			_spacebarIconTimer.WaitTime = 0.1f;
			_spacebarIconTimer.OneShot = true;
			_spacebarIconTimer.Timeout += SpacebarTimerDone;
			AddChild(_spacebarIconTimer);
			
			GD.Print("TARGET SPAWN: " + MobSpawner.CalcDecrementValue(10,0.5f));
			

			TreeExiting += () => ChooseGame.AddGradeEntry(GameNames.FinkiSurvive,Grade);
			TreeExiting += ResetStats;
			UpdateMenu();
			
			MobScaleHandler.Initialize();

		}

		// ReSharper disable once InconsistentNaming
		private void SetupUIListeners()
		{
			GetNode<MenuButton>("UI/GameInfoCont/MenuButton").GetPopup().PopupHide += ResumeGame;
			GetNode<Button>("DeathScreen/MarginContainer2/PlayAgainButton").Pressed += RestartGame;
			GetNode<Button>("DeathScreen/MarginContainer3/QuitButton").Pressed += QuitToGameSelection;
			GetNode<Button>("WinScreen/MarginContainer2/ContinueButton").Pressed += RestartGame;
			GetNode<Button>("WinScreen/MarginContainer3/GraduateButton").Pressed += QuitToGameSelection;
		}
		
		public override void _Process(double delta)
		{
			_fps.Text = Engine.GetFramesPerSecond().ToString();
		}

		private void PauseSceneTree()
		{
			GetTree().Paused = true;
		}

		private void ResumeGame()
		{
			GetTree().Paused = false;
			Mob.StateValid = true;
			GetNode<CanvasLayer>("WinScreen").Visible = false;
		}

		private void ClearMobs()
		{
			var mobs = GetNode<Node2D>("Mobs");
			foreach (var mob in mobs.GetChildren())
				mob.QueueFree();
		}

		 void PauseGame()
		{
			GetNode<Timer>("MobSpawnTimer").Stop();
			GetNode<Timer>("GameTimer").Stop();
			Mob.StateValid = false;
		}
		

		public void PlayerDeath()
		{
			//EmitSignal(nameof(SendGameStats));
			if (_canGraduate) GetNode<CanvasLayer>("WinScreen").Visible = true;
			else GetNode<CanvasLayer>("DeathScreen").Visible = true;
			
			
			ChooseGame.AddGradeEntry(GameNames.FinkiSurvive,Grade);
			MobScaleHandler.ResetEntries();
			PauseSceneTree();
			ResetStats();
		}

		private void ResetStats()
		{
			Score = 0;
			Grade = 5;
			_canGraduate = false;
			WaveCount = 1;
		}


		private void RestartGame()
		{
			GetTree().Paused = false;
			GetTree().ReloadCurrentScene();
			Mob.StateValid = true;
			ResetStats();
			
		}


		private void QuitToGameSelection()
		{
			GetTree().ChangeSceneToFile("res://MainScene/choose_game.tscn");
			ResetStats();
		}

		private void UpdateScoreLabel()
		{
			GetNode<Label>("UI/ScoreMarginCont/Score").Text = "Score: " + Score;
		}

		private void UpdateGradeLabel()
		{
			if (!_canGraduate && Grade == 6)
			{
				_gradeLabel.QueueFree();
				Color color = Color.FromHtml("#adebb0");
				_gradeLabel = LabelFactory.CreateLabel(color);
				GetNode<MarginContainer>("UI/GradeCont").AddChild(_gradeLabel);
				_canGraduate = true;
			}
			
			_gradeLabel.Text = "Grade: " + Grade;
		}

		private void UpdateScore(int value)
		{
			GD.Print(value + "Val");
			Score += value;
			UpdateScoreLabel();
			
			int previousGrade = Grade;
			UpdateGradeBasedOnScore();

			if (Grade != previousGrade)
			{
				UpdateGradeLabel();
			}
		
		}

		private void UpdateGradeBasedOnScore()
		{
			if (Score >= 1200)
				Grade = 10;
			else if (Score >= 900)
				Grade = 9;
			else if (Score >= 700)
				Grade = 8;
			else if (Score >= 600)
				Grade = 7;
			else if (Score >= 500)
				Grade = 6;
		}
		

		public void KillMob(Mob mob)
		{
			
			var tween = GetTree().CreateTween();
			var coin = mob.DropCoin().Instantiate<Coin>();
			coin.Connect(nameof(coin.CoinPickedUp), new Callable(this, nameof(UpdateScore)));
			coin.Position = mob.Position;
			CallDeferred("add_child", coin);
			
			Vector2 startUpPos = coin.Position + new Vector2(10,-50);
			Vector2 middlePos = startUpPos + new Vector2(30, -20);
			Vector2 endPos = middlePos + new Vector2(10, 50);
			tween.TweenProperty(coin, "position", startUpPos, 0.3f);
			tween.TweenProperty(coin, "position", middlePos, 0.3f);
			tween.TweenProperty(coin, "position", endPos, 0.3f);
			
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

		private void UpdateTimerText(int mins, int secs)
		{
			GetNode<Label>("UI/GameTimeMarginCont/GameTime").Text = $"{mins:D1}:{secs:D2}";
		}

		private void UpdateWaveLabel()
		{
			GD.Print("Mob Spawn Time: " + GetNode<Timer>("MobSpawnTimer").WaitTime);
			
			GetNode<Label>("UI/WaveNumCont/Label").Text = "Wave: " + WaveCount;
		}

		private void UpdateMenu()
		{
		
			if (WaveCount > 5) return;
			
			var menuPopup = GetNode<MenuButton>("UI/GameInfoCont/MenuButton").GetPopup();
			menuPopup.AddSeparator();
			
			if (WaveCount == 1)
			{
				Texture2D texture = ResourceLoader.Load<Texture2D>(ProjectPaths.AssetsPath + "/fx/attack1Image.png");
				Image image = texture.GetImage();
				image.Resize(50, 50, Image.Interpolation.Lanczos);
				ImageTexture resizedTexture = ImageTexture.CreateFromImage(image);
				menuPopup.AddIconItem(resizedTexture,"This attack is very fast but has relatively low damage. Abiilty: Heals player on hit");
			}
			else if (WaveCount == 3)
			{
				Texture2D texture = ResourceLoader.Load<Texture2D>(ProjectPaths.AssetsPath + "fx/slash6/image/slash6_00005.png");
				Image image = texture.GetImage();
				image.Resize(50, 50, Image.Interpolation.Lanczos);
				ImageTexture resizedTexture = ImageTexture.CreateFromImage(image);
				menuPopup.AddIconItem(resizedTexture,"Slow attack speed but packs a big punch");
				
				
			} else if (WaveCount == 5)
			{
				Texture2D texture = ResourceLoader.Load<Texture2D>(ProjectPaths.AssetsPath + "fx/slash5/image/slash5_1_00006.png");
				Image image = texture.GetImage();
				image.Resize(50, 50, Image.Interpolation.Lanczos);
				ImageTexture resizedTexture = ImageTexture.CreateFromImage(image);
				menuPopup.AddIconItem(resizedTexture,"This attack offers a balance of speed and  high damage");
			}
			
		}

		public override void _Input(InputEvent @event)
		{
			if (@event.IsActionPressed("FINKISURVIVE_dash"))
			{
				GetNode<TextureRect>("UI/CurrentWeaponCont/Panel/SpaceBarIcon").Modulate = Color.FromHtml("#acacac");
				_spacebarIconTimer.Start();
			}

			
		}

		public void SpacebarTimerDone()
		{
			GetNode<TextureRect>("UI/CurrentWeaponCont/Panel/SpaceBarIcon").Modulate = Color.FromHtml("#ffffff");
		}
			

		public void DisablePlayerCollisions()
		{
			var player = GetNode<Player>("Player");
			player.SetCollisionLayerValue(4,false);
			player.SetCollisionMaskValue(2,false);
		}

		public void EnablePlayerCollisions()
		{
			var player = GetNode<Player>("Player");
			player.SetCollisionLayerValue(4,true);
			player.SetCollisionMaskValue(2,true);
		}
		
		private void OnGameTimerTick()
		{
			var mobSpawnTimer = GetNode<Timer>("MobSpawnTimer");

			_timeSecs--;

			if (_timeSecs <= 0)
			{
				HandleWaveTransition(mobSpawnTimer);
				_timeSecs = WaveTime;
				UpdateWaveLabel();
				UpdateMenu();
				MobScaleHandler.ResetEntries();
			}

			AdjustMobSpawnTimer(mobSpawnTimer);

			GD.Print(mobSpawnTimer.WaitTime);

			UpdateTimerText(_timeSecs / 60, _timeSecs % 60);
		}

		private void AdjustMobSpawnTimer(Timer mobSpawnTimer)
		{
			mobSpawnTimer.WaitTime -= _spawnRateDecrement;
			
			if (mobSpawnTimer.WaitTime < 0.5f)
			{
				mobSpawnTimer.WaitTime = 0.5f;
			}
		}

		private void HandleWaveTransition(Timer mobSpawnTimer)
		{
			WaveCount++;

			if (WaveCount == 11)
			{
				WaveTime = 10;
				mobSpawnTimer.Stop();
				ClearMobs();
			}
			else if (WaveCount > 11)
			{
				WaveTime = 30;
				if (mobSpawnTimer.IsStopped())
				{
					mobSpawnTimer.Start();
				}
			}
		}

		private void GenerateMobs()
		{
			CurrentWaveState = WaveState.GetWaveState(WaveCount);
			
			PackedScene mobScene = MobSpawner.GetMobScene(CurrentWaveState.GetMobSceneIndex());
			var instance = mobScene.Instantiate<Mob>();
			instance.Connect(nameof(Mob.MobDamaged),new Callable(this,nameof(KillMob)));

			var player = GetNode<Player>("Player");
			float width = player.Position.X + (GetViewport().GetVisibleRect().Size[0] - player.Position.X);
			float height = player.Position.Y + (GetViewport().GetVisibleRect().Size[1] - player.Position.Y);
			
			instance.Position = MobSpawner.GetMobSpawnPosition((int)width,(int)height); 
			GetNode<Node2D>("Mobs").AddChild(instance);
		}
	}
}

