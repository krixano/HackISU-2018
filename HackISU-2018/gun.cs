﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace HackISU_2018
{
    class gun
    {
        static public Game1.SpriteStruct gunArm, shell;
        static public Game1.SpriteStruct[] bullet;
        static public int bulletSpeed, shotgunSpread;
        static public float bulletSize, shellSize, rateOfFire, tick;
        static public int ammo;
        static public bool isEmpty;

        
        public enum GunSelections
        {
            HANDGUN = 35,
            SMG = 25,
            ASSAULT_RIFLE = 10,
            SHOTGUN = 45
        }
        static public GunSelections gunSelection = GunSelections.ASSAULT_RIFLE;

        static public void gunInit()
        {
            //INITIALIZATION            
            shotgunSpread = 5;
            bulletSpeed = Game1.screenRectangle.Width / 15;

            shell.position_wp.X = gunArm.position_wp.X;
            shell.position_wp.Y = gunArm.position_wp.Y;

            //Rate Of Fire: The Higher it is the slower you shoot (out of 60)
            rateOfFire = (float)gunSelection;
            tick = 0;

            //Gun Arm Size
            gunArm.size.X = player.sprite.size.X;
            gunArm.size.Y = gunArm.size.X / 3;
            gunArm.effect = SpriteEffects.None;
            Console.WriteLine(gunArm.size.X + " " + gunArm.size.Y);

            //Bullet Sizes
            if (gunSelection == GunSelections.HANDGUN)
            {
                bulletSize = gunArm.size.X / 4;
                ammo = 10;
            }
            else if (gunSelection == GunSelections.ASSAULT_RIFLE)
            {
                bulletSize = gunArm.size.X / 8;
                ammo = 30;
            }
            else if (gunSelection == GunSelections.SMG)
            {
                bulletSize = gunArm.size.X / 6;
                ammo = 25;
            }
            else if (gunSelection == GunSelections.SHOTGUN)
            {
                bulletSize = gunArm.size.X / 12;
                shell.size.X = bulletSize;
                shell.size.Y = bulletSize;
                ammo = 2;
                Game1.gunArmTexture = Game1.shotgunTexture;
            }

            bullet = new Game1.SpriteStruct[100]; 
            for (int i=0; i< bullet.Length; i++)
            {
                bullet[i].isFired = false;
                bullet[i].size.X = bulletSize;
                bullet[i].size.Y = bulletSize;
                bullet[i].position_wp.X = gunArm.position_wp.X + gunArm.size.X / 2 - bullet[i].size.X / 2;
                bullet[i].position_wp.Y = gunArm.position_wp.Y + gunArm.size.Y / 2 - bullet[i].size.Y / 2;
            }
            
            //Gun Arm Position Init
            gunArm.position_wp.X = player.sprite.position_wp.X + (player.sprite.size.X / 2);
            gunArm.position_wp.Y = player.sprite.position_wp.Y + (player.sprite.size.Y / 2);            

        }
        public static void gunUpdate()
        {
            if (Game1.keyboard.IsKeyDown(Keys.F) && Game1.prevKeyboard.IsKeyUp(Keys.F))
            {
                gunSelection = GunSelections.SHOTGUN;
                ammo = 2;
            }
            else
            {
                gunSelection = GunSelections.ASSAULT_RIFLE;
                ammo = 30;
            }
            if (Game1.mouse.X < player.sprite.position_wp.X - World.offset_b.X * World.BLOCK_SIZE)
            {
                gunArm.effect = SpriteEffects.FlipVertically;
            } else
            {
                gunArm.effect = SpriteEffects.None;
            }
            checkForBulletCollision();

            if (ammo == 0)
            {
                isEmpty = true;                
            }
            if (Game1.keyboard.IsKeyDown(Keys.R))
            {
                //RELOAD!!!
                if (gunSelection == GunSelections.HANDGUN)
                    ammo = 10;
                if (gunSelection == GunSelections.ASSAULT_RIFLE)
                {
                    ammo = 30;
                    isEmpty = false;
                }
                if (gunSelection == GunSelections.SMG)
                    ammo = 25;
                if (gunSelection == GunSelections.SHOTGUN)
                {
                    ammo = 2;
                    isEmpty = false;
                }
            }
            //Gun Arm Position Update
            gunArm.position_wp.X = player.sprite.position_wp.X + player.sprite.size.X / 2;
            gunArm.position_wp.Y = player.sprite.position_wp.Y + player.sprite.size.Y / 2;
            gunArm.rotation = (float) getMouseAngle();
            tick++;

            //Shoots SHOTGUN at SHOTGUN (50) Rate of Fire
            if (Game1.mouse.LeftButton == ButtonState.Pressed
                && tick % rateOfFire == 0 && gunSelection == GunSelections.SHOTGUN && !isEmpty)
            {
                shootGun();
            }

            

            //Shoots gun at selected Rate of Fire
            else if (Game1.mouse.LeftButton == ButtonState.Pressed
                && tick % rateOfFire == 0 && !isEmpty)
            {
                shootGun();
                
            }
            for (int i = 0; i < bullet.Length; i++)
            {
                if (bullet[i].isFired)
                {
                    //This is what happens when a bullet is fired
                    bullet[i].position_wp.X += bulletSpeed * (float)Math.Cos(bullet[i].rotation);
                    bullet[i].position_wp.Y += bulletSpeed * (float)Math.Sin(bullet[i].rotation);
                    shell.position_wp.X = gunArm.position_wp.X;
                    shell.position_wp.Y = gunArm.position_wp.Y;
                    //if (bullet[i].position_wp.X > Game1.screenRectangle.Width * 2 || bullet[i].position_wp.Y > Game1.screenRectangle.Height * 2
                    //    || bullet[i].position_wp.X < Game1.screenRectangle.Width * -2 || bullet[i].position_wp.Y < Game1.screenRectangle.Height * -2)
                    //    bullet[i].isFired = false;
                
                if (bullet[i].position_wp.X > World.WORLD_SIZE.X || bullet[i].position_wp.X < World.WORLD_SIZE.X
                    || bullet[i].position_wp.Y > World.WORLD_SIZE.Y || bullet[i].position_wp.Y < World.WORLD_SIZE.Y)
                    bullet[i].visible = false;
                }

            }
        }
        public static double getMouseAngle()
        {
            //Returns angle of mouse and aim
            Vector2_Double distance;
            double angle;            
            distance.X = Game1.mouse.X - (gunArm.position_wp.X - World.offset_b.X * World.BLOCK_SIZE);
            distance.Y = Game1.mouse.Y - (gunArm.position_wp.Y - World.offset_b.Y * World.BLOCK_SIZE);                                  
            angle =  Math.Atan2(distance.Y, distance.X);                        
            return angle;
        }
        public static void shootGun()
        {
            ammo--;            

            //Do this whenever you click/bullet fired
            for (int i=0; i<bullet.Length; i+= shotgunSpread)
            {              
                
                if (!bullet[i].isFired && gunSelection == GunSelections.SHOTGUN)
                {                    
                    
                    for (int j = 0; j < shotgunSpread; j++)
                    {
                        bullet[j].position_wp.X = player.sprite.position_wp.X + player.sprite.size.X / 2;
                        bullet[j].position_wp.Y = player.sprite.position_wp.Y + player.sprite.size.Y / 2;
                        bullet[j].isFired = true;
                        //gives shotgun spread (Math.PI * 1/3 is how spread out)
                        bullet[j].rotation = (float)( (Math.PI * 1/3) / shotgunSpread * j) + (float) (gunArm.rotation - (Math.PI * 1/3) / 2);
                    }
                    break;
                }
                else if (!bullet[i].isFired)
                    {
                        bullet[i].position_wp.X = player.sprite.position_wp.X + player.sprite.size.X / 2;                    
                        bullet[i].position_wp.Y = player.sprite.position_wp.Y + player.sprite.size.Y / 2;
                        bullet[i].isFired = true;                    
                        bullet[i].rotation = gunArm.rotation;
                        break;
                    }
            }
        }
        public static void checkForBulletCollision()
        {
            for (int i = 0; i < bullet.Length; i++)
            {
                for (int j = 0; j < enemy.enemySprite.Length; j++)
                {
                    if (bullet[i].isFired)
                    {
                        if (bullet[i].position_wp.X >= enemy.enemySprite[j].position_wp.X && bullet[i].position_wp.X <= enemy.enemySprite[j].position_wp.X + enemy.enemySprite[j].size.X
                            && bullet[i].position_wp.Y <= enemy.enemySprite[j].position_wp.Y + enemy.enemySprite[j].size.Y && bullet[i].position_wp.Y >= enemy.enemySprite[j].size.Y)
                        {
                            enemy.enemySprite[j].health -= 35; // 5% hit/decrease
                            bullet[i].isFired = false;
                            //enemy.enemySprite[j].visible = false;
                            //enemy.spawnRate -= 100;
                            //enemy.enemiesLeft--;
                        }
                    }
                    //if (bullet[i].position_wp.X )
                }
            }
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Game1.font, "Ammo: " + ammo, Game1.fontVector, Color.White);
            Game1.fontVector = new Vector2(0, 0);
            for (int i = 1; i < ammo + 1; i++)
            {
                
                
                if (gunSelection == GunSelections.SHOTGUN)
                {
                    int x = (int)(Game1.screenRectangle.Left  + 10 * i ^2);
                    int y = (int)(Game1.screenRectangle.Top + 10 * i * i);
                    spriteBatch.Draw(Game1.shotgunShell, new Rectangle(x, (int) (Game1.fontVector.Y + Game1.font.MeasureString("Ammo").Y), (int)Game1.shotgunShell.Width, (int)Game1.shotgunShell.Height ), Color.White);
                }
                else
                {
                    int x = (int)(Game1.screenRectangle.Left + bullet[i].size.X * i);
                    int y = 30;
                    
                    spriteBatch.Draw(Game1.bulletTexture, new Rectangle(x, y, (int)Game1.bulletTexture.Width, (int)Game1.bulletTexture.Height), Color.White);
                }
            }
        }
        public static void switchWeapons()
        {
            
        }
    }

}
