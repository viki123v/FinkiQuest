# FinkiQuest

> [!NOTE]
> Играта има извршна верзија, погледнете во 'Releases'!
## Опис на играта

Finki Quest се одвива во замислен свет од иднината, во новата модерна зграда на Факултетот за Информатички Науки и Компјутерско Инжинерство. Главен играч сте вие: студент на ФИНКИ и ваша главна цел е целта на секој студент: да дипломирате, односно да ги положите сите предмети. Finki Quest содржи 3 игри/испити:

- Space Algorithms
- Block Networking
- Probability Survival

Секоја игра претставува предмет на факултетот што мора да биде положен за да дипломирате.  
 За да дипломирате мора да добиете барем оцена 6 во секоја од игрите.

## Користени Технологии

Finki Quest е направена со помош на Game Engine - [Godot](https://godotengine.org/) , со јазична поддршка за C#.

## Содржина

- ### Компоненти
  - [Стартно Мени](#стартно-мени)
  - [Мени за Селекција на Игри](#мени-за-селекција-на-игри)
- ### Игри
  - [Space Algorithms](#space-algorithms)(Мартин Џаков - 223078)
  - [Block Networking](#block-networking)(Виктор Христовски - 226026)
  - [Probability Survival](#probability-survival)(Стефан Тосковски - 225144)

# Стартно Мени

Најпрво играчот се запознава со новата, модерна зграда на ФИНКИ. Нејзиниот изглед е прикажан на сликата.

![](/BasicNetworking/README_Assets/WelcomePage.png)

# Мени за Селекција на Игри

Наредниот прозорец е прозорецот за селекција на испит/игра. Овај прозорец, исто така служи како досие за успесите на студентот, прикажувајќи ги најдобрите оценки,освоени по предметите/игрите. Со положувањето на секој предмет, пристигувате на еден чекор поблиску до дипломата, која што можете да ја подигнете со притискање на копчето **_"Graduate"_**, откако ќе ги положите сите испити.

![](/BasicNetworking/README_Assets/SelectGame.png)

## Space Algorithms

### Опис на играта:

Во оваа игра, играчот управува со вселенски брод кој лета низ галаксија полна со предизвици и опасности, сите инспирирани од темите на предметот **Алгоритми и Податочни Структури**.

### Механика на играта:

- **Противници и поени**: Играта започнува со борба против различни типови непријатели кои претставуваат вселенски објекти. Уништувањето на овие непријатели носи поени, со максимален износ од 40 поени.

- **Главен непријател (Испит)**: По одредено време на борење со овие непријатели, се појавува главниот непријател - Испитот. Испитот е огромен и моќен непријател кој ги тестира вештини што играчот ги стекнал во текот на играта. За да се заврши играта успешно и да се „положи“ предметот, играчот мора да го уништи Испитот, при што се добиваат 60 поени, доволни за основна положена оценка (6).

- **Формирање на конечната оценка**: Оценката се формира така што се додаваат поените добиени од непријателите како и од испитот (Вкупно 100 поени).

### Функционалности

Играта се одвива во класата `public partial class Game : Node2D` која претставува сцена од тип `Node2D`.

#### Играч

Играчот претставува вселенски брод. Се движи со `WASD` или со `Arrow keys`. Се следи движењето со пратење Input настани во секоја рамка од играта и се составува Vector2. Потоа брзината на движењето се множи со delta за да се обезбеди конзистентно движење на играчот без разлика на големината на екранот.

```C#
Vector2 inputVector = Input.GetVector("space_algorithms_player_left", "space_algorithms_player_right", "space_algorithms_player_up", "space_algorithms_player_down");

Position += inputVector * moveSpeed * (float)delta;
```

Кога играчот напаѓа со притискање на копчето `space` се инстанцира нов ласер и се додава на главната сцена.

```C#
if (Input.IsActionJustPressed("space_algorithms_player_shoot")){
  Laser laser = (Laser)laserPrefab.Instantiate();
  laser.Position = Position;
  this.GetParent().AddChild(laser);
}
```

Доколку играчот е во колизија со непријател или напад на непријател тој се уништува и играта завршува.

```C#
private void _on_area_entered(Area2D area){
  if(area is EnemyLaser || area is Enemy || area is Meteor || area is BossSpecial){
    QueueFree();
  }
}
```

#### Непријатели

Непријателите ги добиваат имињата по случаен избор од листа на зборови кои претставуваат концепти од Алгоритми. При секое отчукување на тајмерот `private void _on_enemy_timer_timeout()` доколку има преостанати имиња во листата на концепти се иницијализра нов непријател на случајна позиција и се додава на главната сцена.

```C#
Random r = new Random();
Enemy enemy = (Enemy)enemyPrefab.Instantiate();
enemy.Position = new Vector2(_screenSize.X + 30, r.Next(40, (int)_screenSize.Y - 40));
enemyLabel.Text = enemies[randomIndex];
AddChild(enemy);
```

Непријателите се уништуваат доколку се во колизија со ласерот на играчот. Се додаваат соодветните поени и се бриши од сцената.

```C#
private void _on_area_entered(Area2D area){
		if(area is Laser){
      		var gameNode = (Game) this.GetParent();
			gameNode.incrementPoints();
			QueueFree();
    	}
}
```

#### Испит (Boss)

Испитот се појавува кога играчот ќе ги совлада сите концепти (обични непријатели)

```C#
if(enemies.Count == 0){
	boss = (Boss)bossPrefab.Instantiate();
	boss.Position = new Vector2(_screenSize.X - 400, _screenSize.Y / 2);
	this.AddChild(boss);
}
```

Испитот има иницијална енергија(hp) која се намалува при колизија со ласер на играчот.

```C#
private void _on_area_entered(Area2D area)
    {
        if (area is Laser)
        {
          hp--;
        }
    }
```

Испитот напаѓа со 3 различни напади активирани од тајмери: 3 ласери кои не се уништуваат, штит кој ги блокира нападите на играчот и инстанцирање на 3 метеори со случајна позиција.

### Слики од играта

#### Стартна сцена
![start](https://github.com/user-attachments/assets/b135dc34-24aa-4a96-abfb-efb2687f1cdc)

#### Инструкции
![instructions](https://github.com/user-attachments/assets/cc994f81-e2ce-4e06-aa02-f307ce5c54b0)

#### Игра
![game](https://github.com/user-attachments/assets/a3dea021-9acf-445c-9701-bf75f4e2abae)

#### Испит
![boss](https://github.com/user-attachments/assets/217fccd5-06ee-4c08-8f6c-ee0ab40477c1)

#### Постигната оценка
![grade](https://github.com/user-attachments/assets/8750ccad-9888-456c-b568-832bf2768210)

## Block Networking

### Цел на играта и начин на играње

Block Networking е градена во духот на познатата игра [Tetris](https://tetris.com/play-tetris), со мал финес. Блоковите во овај тетрис се мрежни уреди и успешното завршување на играта претставува успешно положување на предметот **Мрежи**. Од вас како играч, се очекува успешно да се справите со паѓачките фигури _(групи од блокови со определена форма)_ , кои постојано се појавуваат од една иста точка. Секоја фигура започнува во "подвижна" состојба, односно состојба во која на одредени временски интервали фигурата се придвижува за еден блок надолу _(секој блок од фигурата се поместува за еден блок надолу)_ и состојба во која вие,играчот, ќе можите да управувате со истата _(да ја ротирате или придвижувате)_. Поради допирот со долниот дел од екранот или со друга "неподвижна" фигура, "подвижната" фигура преминува во состојба "неподвижна". Во неподвижна состојба, играчот не може веќе да управува со фигурата и истата не се движи кога останатите "подвижни" фигури се движат. Доколку играчот успее да подреди цела редица со неподвижни блкови _(дел од "неподвижна" фигура)_ , истата ќе биде избришана и притоа сите останатите неподвижни блокови над избришаната редица ќе започнат да паѓаат и истите ќе престанат кога ќе допрат со крајот на екранот или со друг неподвижен блок.

Главната цел во играта е играчот да опстане одредено време. Потоа, врз основа на опстанатото време играчот добива оценка, која се прикажува во посебен прозорец _(опстојување од 45s се смета доволно за оценка 6)_.
Брзината на движењата и фреквенцијата на појавувањето на блоковите се пресметуваат како функција од поминатото време, што значи дека се зголемуваат со текот на времето, што впрочем значи дека играчот сепак ќе треба да се потруди за повисока оценка.

Играта се игра со помош на arrow keys, притоа акциите се ивршуваат само на последниот блок во паѓање. Акциите кои се дозволени и копчината кои ги овозможуваат истите, се следниве:

- **Up arrow:**&nbsp;&nbsp;ротирање на фигурата.
- **Left arrow:** &nbsp;&nbsp;движење на фигурата за еден блок на лево.
- **Right arrow:** &nbsp;&nbsp;движење на фигурата за еден блок на десно.
- **Down arrow:**&nbsp;&nbsp;движење на фигурата за еден блок надолу.

### Oпис на соочените проблеми

Проблемите со кои се соочив во изработката на оваа игра и нивните решенија се следниве:

- **Детектирање на стопиран блок и правилно движење на блоковите**. Пред било какво движење на блок потребно да е осигураме дека:

  - **Новата позиција не излегува од екранот**. Овај услов го проверувам во А1,А2,Б1. Променливата `isDownMove` е битна заради тоа што сакаме во играта само при надолно движење _(било какво дивежење кое би предивикало блокот да се помести еден блок надолу)_ , блокот да се фиксира, односно да стани
    неподвижен. Променливата `positions` е листа на новите, недоделени позиции, односно позиции за проверка. `UpdateCount()` е функција која го ажурира `CountByRow` _(потребата од променливата е објаснета подолу за проблемот "Оптимално пратење на пополнетоста на редиците" )_, заради тоа што блокот станал фиксен, на местата каде што се користи `UpdateCount()` и според тоа потребно е промената да се евидентира.

  - **Новата позиција не е окупирана од некој друг блок**. Овај услов го проверувам во Б2, со што се проверува дали на новата позиција `SharedBitMap[x, y]` има веќе поставен блок и дали се на различни фигури.
    Проверката дали блоковите се на различни фигури е важно за да не ги погрешиме претходните позиции на блоковите од истата група со новите позициите на истите, бидејќи блоковите од иста група се движат заедно.

```C#
 private bool CanMove(List<Vector2> positions, bool isDownMove)
	{
		var rowNum = FinkiTetrisPlayingGame.Dimensions.Item1;
		var columnNum = FinkiTetrisPlayingGame.Dimensions.Item2;

		foreach (var pos in positions)
		{
			var x = (int)pos.X;
			var y = (int)pos.Y;

			//А1
			if (x >= rowNum && !isDownMove)
			{
				return false;
			}

			//А2
			if (x >= rowNum && isDownMove)
			{
				IsStopped = true;
				UpdateCount();
				return false;
			}

			//Б1
			if (y < 0 || y >= columnNum)
			{
				return false;
			}

			//Б2
			if (SharedBitMap[x, y] is not null && SharedBitMap[x, y].Parent != this)
			{
				if (isDownMove)
				{
					IsStopped = true;
					UpdateCount();
				}

				return false;
			}
		}

		return true;
	}
```

- **Пратење на позицијата на блоковите**. За решавање на овај проблем користам матрица со фиксна големина од 20x10 _(`BitMap` во `BlockNetworkingPlaying`)_, која постојано мора да ја рефлектира состојбата на екранот. Ажурирањето на матрицата е оставено на индивидуалните блокови _(инстаци од `Block` класата)_, кои при секое поместување или ротирање вршат промени на оваа структура. Се одлучив за користење
  на оваа структура заради тоа што многу брзо и лесно можам да добијам кој е блокот во дадена ќелија, исто така и која е фигурата во која припаѓа блокот _(фигурата е претставена преку `Parent`)_ .

- **Оптимално пратење на пополнетоста на редиците**. Со цел избегнување на постојаното O(n^2) време потребно да се провери целата матрица за целосно пополнета редица, ja користам низа `CountByRow`, чија намена е постојано да го прати бројот на неподвижни блокови во редиците. Нејзиното ажурирање, исто како и за`BitMap` структурата, е оставено на самите блокови.

- **Ротирање на фигури**. Секој тип на група е претставен со интерфејсот `IGroupTypes` и неговите имплементации. Позициите на индивидуалните блокови се претставени преку distance вектори од некоја пивот позиција, тоа ми овозможи само преку една точка на екранот да ја конструирам целата група. Во пивот точката може да замислиме дека има две оски x,y, чии насоки зависат од тоа колку пати веќе сме ја ротирале фигурата.
  Користењето на оските ни овозможува ротацијата да ја извршиме со смена на насоките на оските. На пр. ако пивот точка ни е (0,0) и имаме блок со distance вектор од таа точка (0,1), тоа значи дека во неротирана состојба _(кога x oската има насока (1,0) и y оската има насока (0,-1))_ блокот би се наоѓал над пивот точката _((0,0) + 0\*(1,0)+1\*(0,-1)=(0,-1))_ . За да ја ротираме групата за 90 степени само ги ротираме оските за 90 степени што значи дека x оската би имала насока (0,1) и y оската би имала насока (1,0). Сега ако се обидиме да ја изградиме групата би добиле дека (0,0)+1*(1,0)+0*(0,-1)=(1,0),односно блокот би се наоѓал десно од пивот точката. Во имплементацијата `pivotPosition` ја содржи позицијата на пивот точката _(склаирана во `Block` матрицата)_ , `fromPivotToBlcok` се distance векторите на сите блокови во тој тип на група и `direction` ги содржи насоките
  на оските, од кои зависи самата ротација.

```C#
public static List<Vector2> FindBlocksBitmapPosition(Vector2 pivotPosition, Vector2[] fromPivotToBlcok,
		(Vector2, Vector2) direction)
	{
		List<Vector2> newPositions = new List<Vector2>(fromPivotToBlcok.Length);
		newPositions.Add(pivotPosition);

		for (int i = 1; i < fromPivotToBlcok.Length; i++)
		{
			var newPosition = new Vector2(pivotPosition.X, pivotPosition.Y);

			var currentDisFromPivot = fromPivotToBlcok[i];

			var xAxis = direction.Item1;
			var yAxis = direction.Item2;

			var xAxisTranslation = new Vector2(currentDisFromPivot.X * xAxis.X, currentDisFromPivot.X * xAxis.Y);
			var yAxisTranslation = new Vector2(currentDisFromPivot.Y * yAxis.X, currentDisFromPivot.Y * yAxis.Y);

			newPosition.X += xAxisTranslation.X + yAxisTranslation.X;
			newPosition.Y += xAxisTranslation.Y + yAxisTranslation.Y;

			newPositions.Add(newPosition);
		}

		return newPositions;
	}
```

- **Анимации за бришење и паѓање**. Анимациите за бришење и паѓање се синхронизираат со помош на тајмери _(Godot ни овозможува анимациите да ги реализираме преку "Nodes" од типот `AnimationPlayer`, но ако истите ги искористев во проблемот, тогаш за секој блок на екранот ќе имав посебен `AnimationPlayer` и истото би имало големи последици на преформасите)_ . Синхронизацијата на тајмерите се реализира преку интеркација со инстанца од класата `TimerMenager`. Целта на `TimerMenager` е полесно координирање на низа на акции во однос на повеќе тајмери, кои се потребни како на пр. за замрзнување на екранот при бришење на блокови, каде што потребно е `_timerTracker`,`_spawnTimer`,`MoveTimer` да се запрат. Класата овозможува избегнување на проблеми од типот на "Заборавив да го исклучам тајмер за движење во еднава функција" или пак "Заборавив да го активирам тајмерот за инстанцирање во другава функција".

### Слики од играта

- **Почетен прозорец**

![](/BasicNetworking/README_Assets/BasicNetworkingIntro.png)

- **Прозорец за играње**

![](/BasicNetworking/README_Assets/BlockNetworkingGamePlay.png)

- **Прозорец со освоената оценка**

![](/BasicNetworking/README_Assets/BasicNetworkingEnding.png)

## Probability Survival

Оваа игра ја прати тематиката на Endless Survival (тип на игри во кои што целта е да се преживее што подолго).  
Играчот е постојано бркан од непријатели, чија цел е да го убијат. Непријателите се појавуваат во бранови, каде секој бран трае 30 секунди.

Што повеќе време помиува во еден бран, толку почесто се појавуваат непријатели, и што повеќе бранови поминуваат, непријателите стануваат се` посилни и се појавуваат нови типови на непријатели.  
Играчот, со убивање на непријатели добива парички кои носат поени. Од поените зависи оценката прикажана во горниот лев агол на екранот. Играчот добива оценка доволна за положување кога ќе стекне барем 500 поени. Потоа се добиваат повисоки оценки се до 1200 поени.

Играта завршува кога играчот ќе умре или ако се напушти играта. Може да се игра произволен број на пати со цел да си ги подобрите поените и оценката.

Правилата на играта се опишани пред да почнете:

![image](https://github.com/user-attachments/assets/987d17b0-64f2-48c1-b07d-ef7f20d80a6f)

# Функционалности

## Играч

- Играчот го движите со `WASD` копчињата напаѓате со лев клик на глувчето и може да "скокнете"(dash) со голема брзина со притискање на `SpaceBar`.  
  Секој напад е уникатен, така што има различна брзина на напаѓање и сила на напад.  
  Бирате напади со притискање на `R` копчето, со секое притискање се бира следниот напад.  
  Играчот има вкупно 3 напади, но сите не се достапни од почеток.
  - Првиот напад го добивате веднаш, при почеток на играта
  - Вториот напад го добивате на третиот бран
  - Третиот напад го добивате на петтиот бран

### Движење

- Движењето на играчот се следи така што се пратат Input настани во секоја рамка од играта и се составува Vector2 според притиснатите копчиња, па се нормализира. Така ја добиваме насоката на движење.

```C#
Vector2 direction = Input.GetVector(
                "FINKISURVIVE_player_left",
                "FINKISURVIVE_player_right",
                "FINKISURVIVE_player_up",
                "FINKISURVIVE_player_down").Normalized();
```

- Вака со притискање на две копчиња заедно (пример `W` и `D`), играчот може да се движи и дијагонално, односно во 8 насоки.
  При движење, играчот ја прати позицијата на глувчето, т.е насоката. Со помош на анимации играчот се врти во насока на глувчето.

### Напади

Единствениот начин играчот да се справи непријателите е со помош на неговите напади.

- Секој напад е уникатен и има свои карактеристики како: рата на напаѓање, сила на напад и домет на напад.
- Сите напади наследуваат од абстрактната класа `BaseAttack`.
- Нападите се вчитуваат и се појавуваат секогаш кога играчот ќе го притисне копчето за напад `LMB`.
- Нападите се поставуваат на одредена позиција оддалечени од играчот, секогаш во насока на покажувачот на глувчето и се започнува анимацијата за напад.
- Ако стапат во колизија со некој непријател, животната енергија на тој непријател се намалува според силата на нападот.
- Кога ќе заврши анимацијата, нападот се брише.

  ![Screenshot from 2024-09-01 23-16-15](https://github.com/user-attachments/assets/e2e4122a-d21c-4ed1-844e-9ad9c66da22a)

## Непријатели

- Непријателите функционираат така што се појавуваат во случајни позиции во однос на играчот. Од кога ќе се појават, тие постојано се движат кон него, со цел да го убијат. Кога ќе се доближат доволно блиску, ја повикуваат својата анимација за напад па го напаѓаат играчот.
  Засега има 3 типа на непријатели: `OrcMob`,`ZombieMob` и `KnightMob`. Секој непријател наследува од абстрактна класа `Mob` од каде што ги добива основните функционалности, па потоа си имплементира свои специфични функционалности како силата и брзината на неговиот напад, како и брзината на движење.

#### _Движење_

- Една од најважните функционалности на непријателите е да се движат кон играчот, но како точно го прават тоа?
- - Секој непријател добива информација за позицијата на играчот во секоја рамка од играта и според таа позиција, ја пресметува насоката во која треба да се движи. Потоа непријателот движи со предодредена брзина.
- Е сега, ова решение функционира, но движењето на непријателите изгледа баш неприродно, бидејќи тие одма се насочуваат кон играчот, при секое негово движење. Би сакале некако непријателите постепено да се насочуваат кон играчот со цел движењето да изгледа поприродно. Решение за ова е т.н `steering` однесување.

```C#
Vector2 desired = (playerPos - Position).Normalized() * (Speed * (float) delta);
        Vector2 steer = desired - Velocity;

        if (steer.Length() > SeekForce)
        {
            steer *= SeekForce;
        }

        Acceleration = steer;
        Velocity += Acceleration;
```

**_Steering всушност претставува кратење на должината на векторот за насока._**

- `desired` е векторот на насока што покажува кон играчот. `(playerPos - Position).Normalized()`_(насоката кон играчот)_ се множи со `Speed`(_основната брзина на непријателот_) за да ја добиеме посакуваната _крајна_ брзина со која сакаме да се движи непријателот во таа насока.
- `steer` претставува потребната промена на сегашната брзина `Velocity` за да стигне до `desired`. Во овој момент се случува кратењето, така што проверуваме дали должината на `steer` векторот е поголема од нашата посакувана должина `SeekForce`. Ако е поголема тогаш таа ја кратиме да биде точно `SeekForce`.

```C#
protected float SeekForce = 0.05f;
```

- Со менување на вредноста на `SeekForce`, може да контролираме како ќе се однесува даден непријател, во однос на неговото движење.Поголеми вредности се асоцирани со побрзи и нагли движења, а помали вредности постепени и поспори движења.

> [!NOTE]
> > _Секој тип на непријател дава различна вредност на `SeekForce`, со цел да има разновидно движење на различни типови непријатели_

#### _Напади_

- Нападите на непријателите се доста едноставни. Напад се повикува кога непријател ќе стапи во колизија со играчот.

- Секој непријател има своја имплементација за напад, но генерално сите слично функционираат, така што ја викаат функцијата `Player.TakeDamage(int amount)`, каде `amount` е специфичната сила на напад на дадениот непријател.

> [!NOTE]
> >  Во Godot, секој Node има т.н слоеви на колизии _(collision layers)_.
> > На секој Node може да се постави конфигурација за `Layer` и `Mask`.
> > **Layer** - На кој слој е дадениот Node.
> > **Mask** - Кој слоеви може да ги "гледа" дадениот Node.
> > Со овој систем може да се регулира кои објекти _(Nodes)_ стапуваат во интеракција еден со друг.    

```C#
public abstract void Attack();

//Имплементација во OrcMob:

public override void Attack()
		{
			if (!CanAttack) return;
			GetNode<Player>("/root/Level/Player").TakeDamage(GetDamage());
			_animSprite.Play("attack");
			CanAttack = false;

		}

```

## Скалабилност на непријатели

Основна проблематика во оваа игра беше тоа како играта да станува потешка со текот на времето.Следејќи ја тематиката на други Endless Survival игри, избрав два начини.

- ### Непријателите да се појавуваат почесто што подолго трае играта.

  - Со секое отчукување на тајмерот за бран `WaveTimer` (слика тука), се намалува времето на чекање за отчукување на _тајмерот за појавување на непријатели_.`(MobSpawnTimer)`
    _(Тајмерот за бран отчукува секоја секунда)_

  ```C#
  private void AdjustMobSpawnTimer(Timer mobSpawnTimer)
  	{
  		mobSpawnTimer.WaitTime -= _spawnRateDecrement;
  	}
  ```

  - `_spawnRateDecrement` е фиксна бројка, членка на главната класа `Game`, која се пресметува на почеток на играта од статичниот метод `CalcDecrementValue` на класата `MobSpawner`.

  ```C#
  public static float CalcDecrementValue(int targetWave, float targetSpawnRate)
    {
        return (BaseSpawnRate - targetSpawnRate) / (targetWave * Main.Game.WaveTime);
    }
  ```

  - оваа метода ни враќа вредност така што:
    - Ако го намалуваме времето на чекање на `MobSpawnTimer` со оваа вредност,
      времето на чекање на `MobSpawnTimer` во `targetWave` ќе биде `targetSpawnRate`.
  - Пример: Ако сакаме во Wave 10 да имаме рата на појавување на непријателите од 0.5s:

  ```
  _spawnRateDecrement = MobSpawner.CalcDecrementValue(10, 0.5f);
  _spawnRateDecrement = 0.0026666664 според формулата
  ```

  - **_Со ова непријателите се појавуваат се` почесто со текот на времето_**.

- ### Да се зголемува животната енергија на непријателите после секој бран

  - За оваа цел е одговорна класата `MobScaleHandler` заедно со методата `GetScaledHp` во абстрактната `Mob` класа. Идејата е следна:

    - Секој непријател пред да се појави, одредува колку ќе изнесува неговата животна енергија (`hp`). _(Секој тип на непријател има различна стратегија за скалирање на животната енергија, изразена преку вредноста на променливата `_hpScaleFactor`)_
    - Се повикува функцијата `ScaleHp` која е абстрактна функција и има имплементација во сите типови на непријатели.
    - Функцијата `ScaleHp`, од дадената инстанца на непријател, ја повикува функцијата `GetScaledHp`.

    - Главната логика се одвива во `GetScaledHp`:

  ```C#
   protected int GetScaledHp(float hpScaleFactor,MobType type)
    {
        int hp = MobScaleHandler.GetInstance().GetHpEntry(type);
        hp += (int) Math.Ceiling(hp * (hpScaleFactor + Main.Game.WaveCount / 1000.0f));
        MobScaleHandler.GetInstance().AddEntry(type, hp);
        GD.Print("HpScalingList: " + HpScaling.ToString());
        return hp;
    }

  ```

  - Во функцијата `GetScaledHp` се пресметува новиот зголемен `hp`, според зададената формула ` hp += (int) Math.Ceiling(hp * (hpScaleFactor + Main.Game.WaveCount / 1000.0f`. Tука доаѓа во игра класата `MobScaleHandler`. `MobScaleHandler` го прати шаблонот за дизајн на софтвер Singleton. Оваа класа е одговорна за синхронизација на животните енергии на сите непријатели врз основа во кој бран се наоѓаат, односно сите непријатели од даден тип да имаат исто ниво на `hp` во дадениот бран.  
  - Класата `MobScaleHandler`:

```C#
	public class MobScaleHandler
{
    private Dictionary<MobType, (bool EntryAdded,int Hp)> MobTypeToLastScaledHp = new();
    private static MobScaleHandler instance;

    private MobScaleHandler()
    {
        MobTypeToLastScaledHp[MobType.Orc] = (false, OrcMob.BaseHp);
        MobTypeToLastScaledHp[MobType.Zombie] = (false, ZombieMob.BaseHp);
        MobTypeToLastScaledHp[MobType.Knight] = (false,KnightMob.BaseHp);
    }


    public  void ResetEntries()
    {
        foreach (var key in MobTypeToLastScaledHp.Keys)
        {
            var tuple = MobTypeToLastScaledHp[key];
            tuple.EntryAdded = false;
            MobTypeToLastScaledHp[key] = tuple;
        }
    }

    public void AddEntry(MobType type, int hp)
    {
        var entry = MobTypeToLastScaledHp[type];
        if(entry.EntryAdded) return;

        MobTypeToLastScaledHp[type] = (true, hp);
    }

    public int GetHpEntry(MobType type)
    {
        return MobTypeToLastScaledHp[type].Hp;
    }

    public static MobScaleHandler GetInstance()
    {
        return instance ?? (instance = new MobScaleHandler());
    }
}
```

- Оваа класа, воедно и функцијата `GetScaledHp` функционираат вака:

  - Именикот `MobTypeToLastScaledHp` служи за да води евиденција, за дали даден тип на непријател веќе има скалиран `hp`, како и за вредноста на скалираниот `hp`.
  - Именикот се користи како референца во функцијата `GetScaledHp` за избегнување на редундантни пресметки и за да се превземи сегашната вредност на животна енергија.
  - Од кога ќе се пресмета новата вредност на `hp`, се додава во именикот, ако таа не била веќе запишана за тој тип на непријател односно ако `EntryAdded = false`_.(Ова значи дека првиот непријател што се појавува во даден бран е тој што запишува, другите сите само читаат)_
  - На крај на секој бран, се ресетира `EntryAdded`за сите типови и процесот започнува одново.

  **На овој начин непријателите стануваат се посилни и претставуваат се поголем предизвик што подолго играте**

  ![image](https://github.com/user-attachments/assets/12822da1-2b89-4d0c-a028-4dad787f3f54)
