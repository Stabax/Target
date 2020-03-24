﻿// Decompiled with JetBrains decompiler
// Type: Target.Target
// Assembly: Target, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 210E06DD-6036-47D0-ADB5-DBEC4EDD925B
// Assembly location: D:\Projets\Target\Target.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using Target.Utils;

namespace Target
{
  public struct TargetResource
  {
    public Texture2D idle;
    public Texture2D idle_hitbox;
    public Texture2D firing;
    public Texture2D firing_hitbox;
  }

  public class Target
  {
    //Target Enums
    private enum Type
    {
      Terrorist
    }

    public enum State
    {
      Idle,
      Firing
    }

    private string _name;
    public string name { get { return (_name); } set { _name = value; } }

    private State _state;

    private int _health;
    public int health { get { return (_damage); } set { _health = value; } }

    private int _damage;
    public int damage { get { return (_damage); } set { _damage = value; } }

    private static Random randomGenerator = new Random();
    private int _posX, _posY;
    private Rectangle _sprite;

    private TargetResource _resource;
    public TargetResource resource {
      set
      {
        _resource = value;
      }
    }

    private Timer _attackTimer;
    private bool _isActive;

    public Target() //default ctor
    {
      _state = State.Idle;
      _isActive = true;
    }

    public Target Copy()
    {
      return (Target)this.MemberwiseClone();
    }

    public void randomizeSpawn()
    {
      _posX = randomGenerator.Next(0, Options.Config.Width - getTexture().Width);
      _posY = randomGenerator.Next(0, Options.Config.Height - getTexture().Height);
      _sprite = new Rectangle(_posX, _posY, _resource.idle.Width, _resource.idle.Height);
      if (_damage > 0)
      {
        _attackTimer = new Timer();
        _attackTimer.addAction(TimerDirection.Forward, 2000, TimeoutBehaviour.StartOver, () => {
          fire();
        });
        _attackTimer.addAction(TimerDirection.Forward, 250, TimeoutBehaviour.None, () => {
          _state = State.Idle;
        }); //Reset state
        _attackTimer.Start();
      }
    }

    public bool getActivity()
    {
      return _isActive;
    }

    private Texture2D getTexture()
    {
      switch (_state)
      {
        case State.Idle:
          return _resource.idle;
        case State.Firing:
          return _resource.firing;
      }
      return _resource.idle;
    }
    private Texture2D getHitbox()
    {
      switch (_state)
      {
        case State.Idle:
          return _resource.idle_hitbox;
        case State.Firing:
          return _resource.firing_hitbox;
      }
      return _resource.idle_hitbox;
    }

    public void checkCollision()
    {
      if (!_sprite.Contains((int)HUD._target.X, (int)HUD._target.Y)) return;
      Color[] hitColor = new Color[1];
      getHitbox().GetData<Color>(0, new Rectangle((int)HUD._target.X - _sprite.X, (int)HUD._target.Y - _sprite.Y, 1, 1), hitColor, 0, 1);
      
      if (hitColor[0].A == 0) return; //Transparent, no hit
      GameMain._player.setBulletsHit(1);
      if (hitColor[0].R >= 255) //Headshot
      {
        GameMain.hud.setAction("Headshot!");
        Resources.headshot.Play(Options.Config.SoundVolume, 0f, 0f);
        GameMain._player.setScore(40);
        GameMain._player.setComboHeadshot(1);
      }
      else
      {
        if (hitColor[0].G >= 255) //Legshot
        {
          GameMain.hud.setAction("Legshot...");
          GameMain._player.setScore(10);
        }
        else //Regular
        {
          GameMain._player.setScore(20);
        }
        GameMain._player.resetComboHeadshot(0);
      }
      
      _isActive = false;
    }

    public void fire()
    {
      _state = State.Firing;
      Resources.burst.Play(Options.Config.SoundVolume, 0f, 0f);
      GameMain.hud.setBloodsplat();
      if (randomGenerator.Next(1, 3) == 1)
        Resources.pain1.Play(Options.Config.SoundVolume, 0f, 0f);
      else
        Resources.pain2.Play(Options.Config.SoundVolume, 0f, 0f);
      if (_damage > 0) GameMain._player.setHealth(-_damage);
    }

    public void Update(GameTime gameTime)
    {
      if (_damage > 0) _attackTimer.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(getTexture(), _sprite, Color.White);
    }
  }
}
