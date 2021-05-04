using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using PolyOne;
using PolyOne.Collision;
using PolyOne.Engine;
using PolyOne.Scenes;
using PolyOne.Input;
using PolyOne.Components;

namespace WallJumpPhysics
{
    public class Player : Entity
    {
        private Texture2D texture;

        private Vector2 remainder;
        private Vector2 velocity;

        private bool isOnGround;
        private bool isOnWallRight;
        private bool isOnWallLeft;
        private bool isWallJumping;


        private int sign;
        private int lastSign;
        private bool controllerMode;
        private bool keyboardMode;
        private List<Keys> keyList = new List<Keys>(new Keys[] { Keys.W, Keys.A, Keys.S, Keys.D, Keys.Up,
                                                                 Keys.Down, Keys.Left, Keys.Right ,Keys.Space });

        private const float fallspeed = 6.0f;
        private const float airFriction = 0.96f;


        private const float initialJumpHeight = -5.8f;
        private const float normMaxHorizSpeed = 4.0f;
        private const float jumpWidthLeft = 5.0f;


        private const float runAccel = 1.00f;
        private const float turnMul = 0.9f;

        private const float gravityUp = 0.31f;
        private const float gravityDown = 0.21f;

        private bool buttonPushed;
        private const float graceTime = 66.9f;
        private const float graceTimePush = 66.9f;
        private const float stopMovement = 100.0f;

        CounterSet<string> counters = new CounterSet<string>();

        public Player(Vector2 position)
            : base(position)
        {
            this.Tag((int)GameTags.Player);
            this.Collider = new Hitbox((float)16.0f, (float)16.0f);
            this.Visible = true;

            texture = Engine.Instance.Content.Load<Texture2D>("Player");

            this.Add(counters);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void Update()
        {

            isOnGround = base.CollideCheck((int)GameTags.Solid, this.Position + Vector2.UnitY);

            if(isOnGround == true || counters["graceTimer"] > 0) {
                isWallJumping = false;
            }

            isOnWallRight = base.CollideCheck((int)GameTags.Solid, this.Position + Vector2.UnitX);
            isOnWallLeft = base.CollideCheck((int)GameTags.Solid, this.Position - Vector2.UnitX);

            base.Update();
            Input();

            if(counters["stopMovement"] <= 0) {
                Movement();
            }

            Jump();
            WallJump();

            if(counters["stopMovement"] <= 0) {
                velocity.X = MathHelper.Clamp(velocity.X, -normMaxHorizSpeed, normMaxHorizSpeed);
                MovementHorizontal(velocity.X);
            }
            else {
                velocity.X = MathHelper.Clamp(velocity.X, -jumpWidthLeft, jumpWidthLeft);
                MovementHorizontal(velocity.X);
            }

            velocity.Y = MathHelper.Clamp(velocity.Y, initialJumpHeight, fallspeed);
            MovementVerical(velocity.Y);
        }

        private void InputMode()
        {
            foreach (Keys key in keyList)
            {
                if (PolyInput.Keyboard.Check(key) == true)
                {
                    controllerMode = false;
                    keyboardMode = true;
                }
            }
            if (PolyInput.GamePads[0].ButtonCheck() == true)
            {
                controllerMode = true;
                keyboardMode = false;
            }

            if (controllerMode == false && keyboardMode == false) {
                keyboardMode = true;
            }
        }


        private void Input()
        {
            InputMode();

            if(isWallJumping == false) {
                sign = 0;
            }
   
            if (controllerMode == true)
            {
                if (PolyInput.GamePads[0].LeftStickHorizontal(0.3f) > 0.4f ||
                    PolyInput.GamePads[0].DPadRightCheck == true)
                {
                    sign = 1;
                }
                else if (PolyInput.GamePads[0].LeftStickHorizontal(0.3f) < -0.4f ||
                         PolyInput.GamePads[0].DPadLeftCheck == true)
                {
                    sign = -1;
                }

                if (sign != 0) {
                    lastSign = sign;
                }
            }
            else if (keyboardMode == true)
            {
                if (PolyInput.Keyboard.Check(Keys.Right) ||
                    PolyInput.Keyboard.Check(Keys.D))
                {
                    sign = 1;
                }
                else if (PolyInput.Keyboard.Check(Keys.Left) ||
                         PolyInput.Keyboard.Check(Keys.A))
                {
                    sign = -1;
                }

                if (sign != 0) {
                    lastSign = sign;
                }
            }
        }

        private void Movement()
        {
            float currentSign = Math.Sign(velocity.X);

            if (isOnGround == false) {
                velocity.X *= airFriction;
            }

            if (sign != 0 && currentSign != sign) {
                velocity.X *= turnMul;
            }

            if (graceTimePush > 0 && velocity.Y < 0 && isOnGround == false) {
                velocity.Y += gravityUp;
            }
            else if (isOnGround == false) {
                velocity.Y += gravityDown;
            }

            if (counters["stopTimer"] <= 0) {
                velocity.X += runAccel * sign;
            }

            if (sign == 0 && isWallJumping == false){
                velocity.X = 0.0f;
            }
        }


        private void Jump()
        {
            if (isOnGround == true) {
                counters["graceTimer"] = graceTime;
            }

            if (controllerMode == true)
            {
                if (PolyInput.GamePads[0].Pressed(Buttons.A) == true) {
                    counters["graceTimerPush"] = graceTimePush;
                }

                if (counters["graceTimerPush"] > 0)
                {
                    if (isOnGround == true || counters["graceTimer"] > 0)
                    {
                        counters["graceTimerPush"] = 0.0f;
                        velocity.Y = initialJumpHeight;
                    }
                }
                else if (PolyInput.GamePads[0].Released(Buttons.A) == true && velocity.Y < 0.0f)
                {
                    counters["graceTimerPush"] = 0.0f;
                    velocity.Y = 0.0f;
                }

            }
            else if (keyboardMode == true)
            {
                if (PolyInput.Keyboard.Pressed(Keys.Space) == true) {
                    counters["graceTimerPush"] = graceTimePush;
                }

                if (counters["graceTimerPush"] > 0)
                {
                    if (isOnGround == true || counters["graceTimer"] > 0)
                    {
                        counters["graceTimerPush"] = 0.0f;
                        velocity.Y = initialJumpHeight;
                    }
                }
                else if (PolyInput.Keyboard.Released(Keys.Space) == true && velocity.Y < 0.0f)
                {
                    counters["graceTimerPush"] = 0.0f;
                    velocity.Y = 0.0f;
                }
            }
        }

        private void WallJump()
        {
            if (isOnWallLeft == true) {
                counters["graceTimerLeft"] = graceTime;
            }

            if (isOnWallRight == true) {
                counters["graceTimerRight"] = graceTime;
            }

            if (controllerMode == true)
            {
                if (PolyInput.GamePads[0].Pressed(Buttons.A) == true) {
                    counters["graceTimerPush"] = graceTimePush;
                }

            }
            else if(keyboardMode == true)
            {
                if (PolyInput.Keyboard.Pressed(Keys.Space) == true) {
                    counters["graceTimerPush"] = graceTimePush;
                }
            }

            if (counters["graceTimerPush"] > 0)
            {
                if (isOnWallLeft == true || counters["graceTimerLeft"] > 0)
                {
                    if (isOnGround == false || counters["graceTimer"] <= 0)
                    {
                        sign = 1;
                        isWallJumping = true;
                        counters["graceTimerPush"] = 0.0f;
                        velocity.X = jumpWidthLeft;
                        velocity.Y = -4.0f;
                        counters["stopMovement"] = stopMovement;
                    }
                }
                else if (isOnWallRight == true || counters["graceTimerRight"] > 0)
                {
                    if (isOnGround == false || counters["graceTimer"] <= 0)
                    {
                        sign = -1;
                        isWallJumping = true;
                        counters["graceTimerPush"] = 0.0f;
                        velocity.X = -jumpWidthLeft;
                        velocity.Y = -4.0f;
                        counters["stopMovement"] = stopMovement;
                    }
                }
            }
        }

        private void MovementHorizontal(float amount)
        {
            remainder.X += amount;
            int move = (int)Math.Round((double)remainder.X);

            if (move != 0)
            {
                remainder.X -= move;
                int sign = Math.Sign(move);

                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(sign, 0);
                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null)
                    {
                        remainder.X = 0;
                        break;
                    }
                    Position.X += sign;
                    move -= sign;
                }
            }
        }

        private void MovementVerical(float amount)
        {
            remainder.Y += amount;
            int move = (int)Math.Round((double)remainder.Y);

            if (move < 0)
            {
                remainder.Y -= move;
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, -1.0f);
                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }
                    Position.Y += -1.0f;
                    move -= -1;
                }
            }
            else if (move > 0)
            {
                remainder.Y -= move;
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, 1.0f);
                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }

                    Position.Y += 1.0f;
                    move -= 1;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            Engine.SpriteBatch.Draw(texture, this.Position, Color.White);
        }
    }
}
