﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Target.Utils;

namespace Target.Entities
{
  public class Bomb
  {
    private static Random _randomGenerator = new Random();

    private bool _active;
    private Point _position;
    private Texture2D _texture;

    private HorizontalProgressBar _indicator;

    private Timer _durationTimer;
    private Timer _soundTimer;

    public Bomb()
    {
      _active = true;
      _position = new Point(_randomGenerator.Next(0, Options.Config.Width), _randomGenerator.Next(0, Options.Config.Height));
      _texture = Resources.bomb;

      _durationTimer = new Timer();
      _durationTimer.addAction(TimerDirection.Backward, -1, TimeoutBehaviour.Stop, () => { CompleteDefusal(); });
      _durationTimer.addAction(TimerDirection.Forward, 10000, TimeoutBehaviour.Stop, () => { Explode(); });
      _durationTimer.Start();

      _soundTimer = new Timer();
      _soundTimer.addAction(TimerDirection.Forward, 1000, TimeoutBehaviour.StartOver, () => { Resources.bombtick.Play(Options.Config.SoundVolume, 0f, 0f); });
      _soundTimer.Start();

      _indicator = GameMain.hud.addBombIndicator(_position, 10000);
    }

    public bool getActivity()
    {
      return _active;
    }

    public void Destroy()
    {
      _active = false;
      GameMain.hud.removeBombIndicator(_indicator);
      GameMain._player.defusing = false;
    }

    public void CompleteDefusal()
    {
      Destroy();
      Resources.defusal.Play(Options.Config.SoundVolume, 0f, 0f);
      GameMain._player.setDefuser(false);
      //GameMain._player.stats.bombsDefused += 1;
    }

    public void Explode()
    {
      Resources.explosion.Play(Options.Config.SoundVolume, 0f, 0f);
      Destroy();
    }

    public void Defuse()
    {
      GameMain._player.defusing = true;
      _soundTimer.Reset();
      _durationTimer.Reverse();
      if (GameMain._player.hasDefuser()) _durationTimer.setTimeScale(3);
      else _durationTimer.setTimeScale(2);
    }
    public void Rearm()
    {
      GameMain._player.defusing = false;
      _soundTimer.Start();
      _durationTimer.Reverse();
      _durationTimer.setTimeScale(1);
    }

    public Rectangle getRectangle()
    {
      return (new Rectangle(_position.X, _position.Y, _texture.Width, _texture.Height));
    }

    public void Update(GameTime gameTime, DeviceState state, DeviceState prevState)
    {
      if (GameMain.hud.crosshair.checkCollision(getRectangle()))
      {
        if (Options.Config.Bindings[GameAction.Defuse].IsControlPressed(state, prevState)) Defuse(); //User started defusing
        else if (!Options.Config.Bindings[GameAction.Defuse].IsControlDown(state) && _durationTimer.getDirection() != TimerDirection.Forward) Rearm(); //Not holding, rearm
        //Fall here if holding, nothing happens, let the timer go out
      }
      else if (_durationTimer.getDirection() != TimerDirection.Forward) Rearm(); //None, rearm
      _soundTimer.Update(gameTime);
      _durationTimer.Update(gameTime);
      _indicator.Value = (float)_durationTimer.getDuration();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, getRectangle(), Color.White);
    }
  }
}
