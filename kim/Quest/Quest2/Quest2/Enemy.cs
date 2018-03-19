﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Quest2
{
    abstract class Enemy : Mover , IHitable
    {
        protected Rectangle boundaries;
        public Enemy(Game game, Point location, int hitPoints) : base(game, location)
        {
            this.hitPoints = hitPoints;
            boundaries = game.Boundaries;
            
        }

        //전투-------------------------------
        private int hitPoints;
        protected int maxDamage;
        public int HitPoints { get { return hitPoints; } }

        public void Hit(int maxDamage, Random random)
        {
            hitPoints -= random.Next(1, maxDamage);
        }
        //이동-------------------------------
        public abstract void Move(Random random);

        protected virtual void MoveControl(Direction NewDirection,Random random)
        {
             switch (NewDirection)
            {
                case Direction.Up:
                    if (location.Y - MoveInterval >= boundaries.Top)
                    {
                        location.Y -= MoveInterval;
                    }

                    break;
                case Direction.Down:
                    if (location.Y + MoveInterval <= boundaries.Bottom)
                    {
                        location.Y += MoveInterval;
                    }
                    break;
                case Direction.Left:
                    if (location.X - MoveInterval >= boundaries.Left)
                    {
                        location.X -= MoveInterval;
                    }
                    break;
                case Direction.Right:
                    if (location.X + MoveInterval <= boundaries.Right)
                    {
                        location.X += MoveInterval;
                    }
                    break;
                default:
                    break;
            }

            if (NearPlayer())
            {
                game.GiveDamageToPlayer(maxDamage, random);
            }


        }


        //인접 체크---------------------------------
        private const int NearPlayerDistance = 25;

        protected bool NearPlayer()
        {
            return (Nearby(game.PlayerLocation, NearPlayerDistance));
        }
        //자신으로부터 플레이어 위치 확인

        protected Direction FindPlayerDirection()
        {
            Direction directiontoMove;

            if (location.Y > game.PlayerLocation.Y)
            {
                directiontoMove = Direction.Up;
            }

            else if (location.Y < game.PlayerLocation.Y)
            {
                directiontoMove = Direction.Down;
            }

            else if (location.X > game.PlayerLocation.X)
            {
                directiontoMove = Direction.Left;
            }
            else  
                directiontoMove = Direction.Right;

            return directiontoMove;
        }

    }
}
