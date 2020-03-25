﻿// Decompiled with JetBrains decompiler
// Type: Target.Resources
// Assembly: Target, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 210E06DD-6036-47D0-ADB5-DBEC4EDD925B
// Assembly location: D:\Projets\Target\Target.exe

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Target
{
  class Resources
  {
    public static SpriteFont titleFont;
    public static SpriteFont regularFont;

    public static List<Target> targets;

    public static Texture2D menuBackground;
    public static Texture2D hitmarker;
    public static Texture2D bloodsplat;
    public static Texture2D r700;
    public static Texture2D reloadingCursor;
    public static Texture2D crosshair;
    public static Texture2D bullet;
    public static Texture2D itemHealth;
    public static Texture2D itemFastReload;
    public static Texture2D itemDeath;
    public static Texture2D itemSpawnReducer;
    public static Texture2D itemNuke;
    public static Texture2D mapWoods;
    public static Texture2D gamepadKeys;
    public static Texture2D keyboardKeys;

    public static Song menuTheme;
    public static SoundEffect menuClick;
    public static SoundEffect fire;
    public static SoundEffect burst;
    public static SoundEffect reload;
    public static SoundEffect headhunter;
    public static SoundEffect headshot;
    public static SoundEffect breath;
    public static SoundEffect outbreath;
    public static SoundEffect heartbeat;
    public static SoundEffect pain1;
    public static SoundEffect pain2;

    public static void LoadContent(ContentManager content)
    {
      //UI
      Resources.titleFont = content.Load<SpriteFont>("Font/title");
      Resources.regularFont = content.Load<SpriteFont>("Font/regular");
      Resources.menuBackground = content.Load<Texture2D>("GFX/GUI/main");

      //Enemy
      Resources.targets = JsonConvert.DeserializeObject<List<Target>>(File.ReadAllText("Content/Data/targets.json"));
      foreach (var it in Resources.targets)
      {
        loadTargetResource(content, it);
      }
      //Bonus
      Resources.itemHealth = content.Load<Texture2D>("GFX/Entity/health");
      Resources.itemFastReload = content.Load<Texture2D>("GFX/Entity/fastReload");
      Resources.itemDeath = content.Load<Texture2D>("GFX/Entity/death");
      Resources.itemSpawnReducer = content.Load<Texture2D>("GFX/Entity/time");
      Resources.itemNuke = content.Load<Texture2D>("GFX/Entity/nuke");

      //HUD
      Resources.hitmarker = content.Load<Texture2D>("GFX/GUI/hitmarker");
      Resources.bloodsplat = content.Load<Texture2D>("GFX/Player/bloodsplat");
      Resources.reloadingCursor = content.Load<Texture2D>("GFX/Player/reloading");
      Resources.crosshair = content.Load<Texture2D>("GFX/Player/crosshair");
      Resources.bullet = content.Load<Texture2D>("GFX/Weapons/bullet");
      Resources.r700 = content.Load<Texture2D>("GFX/Weapons/r700");
      
      //Help
      Resources.gamepadKeys = content.Load<Texture2D>("GFX/GUI/gamepadKeys");
      Resources.keyboardKeys = content.Load<Texture2D>("GFX/GUI/keyboardKeys");

      //Maps
      Resources.mapWoods = content.Load<Texture2D>("GFX/Maps/woods");

      //Sounds
      Resources.menuTheme = content.Load<Song>("Sound/GUI/menu_theme");
      Resources.menuClick = content.Load<SoundEffect>("Sound/GUI/click");
      Resources.fire = content.Load<SoundEffect>("Sound/Weapons/fire");
      Resources.burst = content.Load<SoundEffect>("Sound/Weapons/burst");
      Resources.reload = content.Load<SoundEffect>("Sound/Weapons/reload");
      Resources.headhunter = content.Load<SoundEffect>("Sound/Target/headhunter");
      Resources.headshot = content.Load<SoundEffect>("Sound/Target/headshot");
      Resources.breath = content.Load<SoundEffect>("Sound/Player/breath");
      Resources.outbreath = content.Load<SoundEffect>("Sound/Player/outbreath");
      Resources.heartbeat = content.Load<SoundEffect>("Sound/Player/heartbeat");
      Resources.pain1 = content.Load<SoundEffect>("Sound/Player/pain1");
      Resources.pain2 = content.Load<SoundEffect>("Sound/Player/pain2");
    }

    public static void loadTargetResource(ContentManager content, Target target)
    {
      TargetResource resource = new TargetResource();

      resource.idle = content.Load<Texture2D>("GFX/Target/" + target.name.ToLower() + "/idle");
      resource.idle_hitbox = content.Load<Texture2D>("GFX/Target/" + target.name.ToLower() + "/idle_hitbox");
      if (target.damage > 0)
      {
        resource.firing = content.Load<Texture2D>("GFX/Target/" + target.name.ToLower() + "/firing");
        resource.firing_hitbox = content.Load<Texture2D>("GFX/Target/" + target.name.ToLower() + "/firing_hitbox");
      }
      target.resource = resource;
    }
  }
}
