﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TheQuest
{
    enum MoveDirection
    {
        MoveUP,
        MoveDown,
        MoveLeft,
        MoveRight
    }
    enum AttackDirection
    {
        AttacUP,
        AttacDown,
        AttacLeft,
        AttacRight
    }

    class Game
    {
        public List<Enemy> Enemies;
        public Weapon WeaponInRoom;

        private Player player;
        public Point PlayerLocation { get { return player.Location; } }
        public int PlayerHitPoints { get { return player.HitPoint; } }
        public List<string> PlayerWeapons { get { return player.Weapons; } }

        private int level = 0;
        public int Level { get { return level; } }

        private Rectangle boundaries;
        public Rectangle Boundaries { get { return boundaries; } }

        public Game(Rectangle boundaries)
        {
            this.boundaries = boundaries;
            player = new Player(this, new Point(boundaries.Left + 10, boundaries.Top + 70));
        }
        public void Move(MoveDirection playerMove, Random enemiesMove)
        {
            player.Move(playerMove);
            foreach(Enemy enemy in Enemies)
            {
                enemy.Move(enemiesMove);
        
                
            }
        }
        public void Equip(string weaponName)
        {
            player.Equip(weaponName);
            
        }
        public bool CheckPlayerInventory(string weaponName)
        {
            return player.Weapons.Contains(weaponName);
        }
        public void DamagedPlayer(int maxDamage , Random random)
        {
            player.Damaged(maxDamage, random);
        }
        public void IncreasePlayerHealth(int health , Random random)
        {
            player.IncreaseHealth(health, random);
        }

        public void Attack(MoveDirection PlayerAttack , Random random)
        {
            player.AttackEnemies(PlayerAttack, random);
            foreach(Enemy enemy in Enemies)
            {
                enemy.Move(random);
            }
        }

        public Point GetRandomLocation(Random random)
        {
            return new Point(boundaries.Left + random.Next(boundaries.Right / 10 - boundaries.Left / 10) * 10,
                             boundaries.Top + random.Next(boundaries.Bottom / 10 - boundaries.Top / 10) * 10);
        }

        public void NewLevel(Random random)
        {
            level++;
            switch (level)
            {
                case 1:
                    Enemies = new List<Enemy>();
                    Enemies.Add(new Bat(this, GetRandomLocation(random)));
                    WeaponInRoom = new Sword(this, GetRandomLocation(random));
                    break;

                case 2:
                    Enemies = new List<Enemy>();
                    Enemies.Add(new Ghost(this,GetRandomLocation(random)));
                    WeaponInRoom = new BluePotion(this, GetRandomLocation(random));

                    break;
                case 3:
                    Enemies = new List<Enemy>();
                    Enemies.Add(new Ghoul(this, GetRandomLocation(random)));
                    WeaponInRoom = new Bow(this, GetRandomLocation(random));
                    break;
                case 4:
                    Enemies = new List<Enemy>();
                    Enemies.Add(new Bat(this, GetRandomLocation(random)));
                    Enemies.Add(new Ghost(this, GetRandomLocation(random)));
                    WeaponInRoom = new BluePotion(this, GetRandomLocation(random));

                    break;
                case 5:
                    Enemies = new List<Enemy>();
                    Enemies.Add(new Bat(this, GetRandomLocation(random)));
                    Enemies.Add(new Ghoul(this, GetRandomLocation(random)));
                    WeaponInRoom = new RedPotion(this, GetRandomLocation(random));

                    break;
            }
        }
    }
}