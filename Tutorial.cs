﻿using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using TargetGame.Entities;
using TargetGame.Settings;
using TargetGame.Utils;

namespace TargetGame
{
  /// <summary>
  /// Represents a step in tutorial with a text, an objective, and an optional failure condition
  /// </summary>
  public struct TutorialStep
  {
    public string sentence;
    public Action setup;
    public Func<bool> objective;
    public Func<bool> failure;

    public TutorialStep(string sentence_, Action setup_, Func<bool> objective_, Func<bool> failure_ = null)
    {
      sentence = Tutorial.sentences[sentence_];
      setup = setup_;
      objective = objective_;
      failure = failure_;
    }

    public void Reset()
    {
      setup();
    }
  }

  /// <summary>
  /// Tutorial class
  /// </summary>
  public class Tutorial
  {
    public static Dictionary<string, string> sentences = new Dictionary<string, string>();
    private List<TutorialStep> _steps;
    private int _currentStep;
    private Timer _timer;
    private Label _sentenceLabel;
    private bool _completed;

    public Tutorial()
    {
      InitOverlay();
      _timer = new Timer();
      _completed = false;
      sentences.Clear();
      _steps = new List<TutorialStep>();
      _currentStep = -1;
      sentences.Add("Welcome", "Welcome to Target, your soon to be favorite range shooter.");
      _steps.Add(new TutorialStep("Welcome", () => { }, () => { return (_timer.getDuration() > 5000); }));
      //Base Initiation
      sentences.Add("HUDIntiation1", "On your top left corner, you can find your health bar.");
      sentences.Add("HUDIntiation2", "On your top left corner, you can can also find your breath bar.");
      sentences.Add("HUDIntiation3", "The score indicator that show all the points you gather during a game.");
      sentences.Add("HUDIntiation4", "And the score multiplier, that applies to each kill you do.");
      sentences.Add("HUDIntiation5", "Now on the bottom right corner, you can see your magazine.");
      _steps.Add(new TutorialStep("HUDIntiation1", () => { }, () => { return (_timer.getDuration() > 5000); }));
      _steps.Add(new TutorialStep("HUDIntiation2", () => { }, () => { return (_timer.getDuration() > 5000); }));
      _steps.Add(new TutorialStep("HUDIntiation3", () => { }, () => { return (_timer.getDuration() > 5000); }));
      _steps.Add(new TutorialStep("HUDIntiation4", () => { }, () => { return (_timer.getDuration() > 5000); }));
      _steps.Add(new TutorialStep("HUDIntiation5", () => { }, () => { return (_timer.getDuration() > 5000); }));
      //Weapon Initiation
      sentences.Add("ShootInitiation", "Shoot some bullets, you will see the ammo getting depleted.");
      sentences.Add("ReloadInitiation", "Now reload to get back your full magazine. In target you have unlimited ammo, so don't hesitate.");
      _steps.Add(new TutorialStep("ShootInitiation", () => { }, () => { return (GameMain.player.getWeapon().getMagazine() < GameMain.player.getWeapon().getMaxMagazine()); }));
      _steps.Add(new TutorialStep("ReloadInitiation", () => { }, () => { return (GameMain.player.getWeapon().getMagazine() == GameMain.player.getWeapon().getMaxMagazine()); }));
      //Bonus Initiation
      sentences.Add("KevlarInitiation", "Pick up this kevlar armor, you will need it on the battlefield.");
      _steps.Add(new TutorialStep("KevlarInitiation", () => { spawnItem(ItemType.KevlarVest); }, () => { return (GameMain.items.Count == 0); }));
      //Target Initiation
      sentences.Add("SoldierInitiation", "Target is not a peaceful land, you can encounter soldiers.");
      sentences.Add("SoldierInitiation2", "Show him what you got by aiming with the crosshair, and shoot him.");
      sentences.Add("SoldierComplete", "Good Job! Now let me get you some tougher targets");
      sentences.Add("JuggernautInitiation", "This is a juggernaut, is more resistant than basic soldiers! Destroy him!");
      sentences.Add("JuggernautInitiation2", "Phew! Big one hm ?");
      sentences.Add("LegshotInitiation", "If you're really bad at aiming, you might shoot in the legs too, try some shots");
      sentences.Add("LegshotComplete", "As you can see, it was not efficient at all, you had to hit him four times to kill");
      sentences.Add("HeadshotInitiation", "Try to headshot this one, it will deal much more damage, and award double the score!");
      sentences.Add("HeadshotComplete", "Awesome right ?! Quake sounds are in!");
      sentences.Add("ZeppelinInitiation", "What about this moving enemy? Can you hit it ?");
      sentences.Add("HostageInitiation", "STOP!! This is an hostage. Don't shoot him or you will lose a lot of points! It will disappear over time.");
      _steps.Add(new TutorialStep("SoldierInitiation", () => { }, () => { return (_timer.getDuration() > 5000); }));
      _steps.Add(new TutorialStep("SoldierInitiation2", () => { spawnTarget("Soldier", false, true); },
        () => { return (GameMain.targets.Count == 0); }));
      _steps.Add(new TutorialStep("SoldierComplete", () => { }, () => { return (_timer.getDuration() > 5000); }));
      _steps.Add(new TutorialStep("JuggernautInitiation", () => { spawnTarget("Juggernaut", false, true); },
        () => { return (GameMain.targets.Count == 0); }));
      _steps.Add(new TutorialStep("JuggernautInitiation2", () => { }, () => { return (_timer.getDuration() > 5000); }));
      _steps.Add(new TutorialStep("LegshotInitiation", () => { GameMain.player.getStats().legshots = 0; spawnTarget("Juggernaut", false, true); },
        () => { return (GameMain.targets.Count == 0 && GameMain.player.getStats().legshots >= 1); },
        () => { return (GameMain.targets.Count == 0 && GameMain.player.getStats().legshots < 1); }));
      _steps.Add(new TutorialStep("LegshotComplete", () => { }, () => { return (_timer.getDuration() > 5000); }));
      _steps.Add(new TutorialStep("HeadshotInitiation", () => { GameMain.player.getStats().headshotsOrCritical = 0; spawnTarget("Juggernaut", false, true); },
        () => { return (GameMain.targets.Count == 0 && GameMain.player.getStats().headshotsOrCritical >= 1); },
        () => { return (GameMain.targets.Count == 0 && GameMain.player.getStats().headshotsOrCritical < 1); }));
      _steps.Add(new TutorialStep("HeadshotComplete", () => { }, () => { return (_timer.getDuration() > 5000); }));
      _steps.Add(new TutorialStep("ZeppelinInitiation", () => { GameMain.player.getStats().score = 0; spawnTarget("Zeppelin", false, true); },
        () => { return (GameMain.targets.Count == 0 && GameMain.player.getStats().score > 0); },
        () => { return (GameMain.targets.Count == 0 && GameMain.player.getStats().score == 0); }));
      _steps.Add(new TutorialStep("HostageInitiation", () => { GameMain.player.getStats().score = 0; spawnTarget("Hostage", false, true); },
        () => { return (_timer.getDuration() > 6000); },
        () => { return (GameMain.targets.Count == 0); }));
      //Contract Initiation
      sentences.Add("ContractInitiation", "As the time goes, you may need a better weapon. That's precisely why contracts can drop");
      sentences.Add("ContractInitiation2", "Gather this contract"); 
      sentences.Add("ContractInitiation3", "Good, now check the objective and reward on the top right corner");
      sentences.Add("ContractInitiation4", "Complete the contract to get an upgrade!");
      sentences.Add("ContractComplete", "Perfect! Your weapon was upgraded");
      _steps.Add(new TutorialStep("ContractInitiation", () => { }, () => { return (_timer.getDuration() > 5000); }));
      _steps.Add(new TutorialStep("ContractInitiation2", () => { spawnItem(ItemType.Contract); }, () => { return (GameMain.items.Count == 0); }));
      _steps.Add(new TutorialStep("ContractInitiation3",
        () => { GameMain.player.contracts[0].type = Contract.Type.HeadshotCombo; }, //Force headshot contract
        () => { return (_timer.getDuration() > 5000); }));
      _steps.Add(new TutorialStep("ContractInitiation4",
        () => { GameMain.player.getStats().contractsCompleted = 0; assignatePlayerContract(); for (int i = 0; i < 5; i++) spawnTarget("Soldier", false, true); },
        () => { return (GameMain.player.getStats().contractsCompleted == 1); },
        () => { return (GameMain.player.contracts.Count == 0 && GameMain.player.getStats().contractsCompleted == 0); }));
      _steps.Add(new TutorialStep("ContractComplete", () => { }, () => { return (_timer.getDuration() > 5000); }));
      //Bomb Initiation
      sentences.Add("BombInitiation", "Quick! You see this bomb ? Hold "+Options.Config.Bindings[GameAction.Defuse].ToString()+" while looking at it with your crosshair to start the defusal.");
      _steps.Add(new TutorialStep("BombInitiation", () => { GameMain.player.getStats().score = 0; GameMain.player.addHealth(100); spawnBomb(); },
        () => { return (GameMain.bombs.Count == 0 && GameMain.player.getStats().score == 0); },
        () => { return (GameMain.bombs.Count == 0 && GameMain.player.getStats().score == -500); }));
      //Finish
      sentences.Add("Ready", "Congratulations! You're now ready for battle. Have fun on target!");
      _steps.Add(new TutorialStep("Ready", () => { }, () => { if (_timer.getDuration() > 5000) CompleteTutorial(); return (false); }));
      NextStep();
    }

    public void assignatePlayerContract()
    {
      if (GameMain.player.contracts.Count == 0)
      {
        Contract ctr = new Contract();
        ctr.type = Contract.Type.HeadshotCombo;
        GameMain.player.contracts.Add(ctr);
      }
    }

    public void spawnBomb()
    {
      Bomb bomb = new Bomb();
      bomb.setPosition(new Point(Options.Config.Width / 2, Options.Config.Height / 2));
      bomb.activate();
      GameMain.bombs.Add(bomb);
    }

    public void spawnItem(ItemType type)
    {
      var item = Resources.items.First((it) => { return (it.type == type); }).Copy();
      item.setPosition(new Point(Options.Config.Width / 2, Options.Config.Height / 2));
      item.move = new Point(0, 0);
      item.activate();
      GameMain.items.Add(item);
    }

    public void spawnTarget(string name, bool randomPos, bool inoffensive)
    {
      var target = Resources.targets.First((it) => { return (it.name == name); }).Copy();
      if (name == "Zeppelin") target.setPosition(new Point(Options.Config.Width, Options.Config.Height / 2));
      else target.setPosition(new Point(Options.Config.Width / 2, Options.Config.Height / 2));
      if (inoffensive) target.damage = 0;
      target.activate();
      GameMain.targets.Add(target);
    }

    public void InitOverlay()
    {
      Stylesheet.Current.LabelStyle.Font = Resources.regularFont;
      _sentenceLabel = new Label();
      _sentenceLabel.Top = -200;
      _sentenceLabel.HorizontalAlignment = HorizontalAlignment.Center;
      _sentenceLabel.VerticalAlignment = VerticalAlignment.Center;
      GameMain.hud.addIndicator(_sentenceLabel);
    }

    public void NextStep()
    {
      _currentStep++;
      _sentenceLabel.Text = _steps[_currentStep].sentence;
      _timer.StartOver();
      _steps[_currentStep].Reset();
    }

    public void CompleteTutorial()
    {
      _completed = true;
      GameMain.hud.removeIndicator(_sentenceLabel);
    }

    /**************************
     * Update/Draw Methods
     **************************/

    public void Update(GameTime gameTime)
    {
      if (_completed)
      {
        GameMain.ExitToMain();
      }
      if (_steps[_currentStep].objective())
      {
        NextStep();
      }
      else if (_steps[_currentStep].failure != null && _steps[_currentStep].failure())
      {
        _steps[_currentStep].Reset();
        _timer.StartOver();
      }
      _timer.Update(gameTime);
    }
  }
}
