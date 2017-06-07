using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TopBall;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;

namespace TopBall
{
    public class Game1 : Game
    {
        /*--------DECLARAÇÃO DE VARIÁVEIS--------*/

        static public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D backgroundText, ballText, gameOverText, ball1Text;
        bool isGameOver;
        Vector2 ballPos, ball1Pos, gameOverPos;  //Variáveis que vão guardar o local em que os objetos aparecem no ecrã.
        float ballXSpeed, ballYSpeed, ball1XSpeed, ball1YSpeed;

        //Personagens:
        personagem jogador1;
        personagem jogador2;

        Song musica;
        int scorefinal;
        bool segundabola;

        int score; //Variável que regista os pontos.
        SpriteFont font; //Para escrever no ecrã.
        public enum GameState { mainMenu, inGame, GameOver } //Diferentes estados do jogo (menu, a jogar, game-over).
        public GameState gameState;
        private List<List<GUIElement>> menus;

        //Variáveis que determinam se os jogadores saltaram ou não:
        bool hasjumped = false;
        bool hasjumped1 = false;




        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Tamanho da janela:
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 450;

            //Torna o cursor visível:
            IsMouseVisible = true;
        }





        protected override void Initialize()
        {
            //Diferentes menus (inicial, game-over):
            menus = new List<List<GUIElement>>
            {   //MainMenu             
                    (new List<GUIElement> //Menu
                {
                      new GUIElement("menu"),
                      new GUIElement("play"),
                }),
                     (new List<GUIElement> //GameOver
                     {
                      new GUIElement("gameover"),
                      new GUIElement("restart"),
                }),
            };




            for (int i = 0; i < menus.Count; i++)
            {
                foreach (GUIElement button in menus[i])
                {
                    button.clickEvent += OnClick;
                }
            }

            //Inicializa as diferentes imagens:
            ballText = Content.Load<Texture2D>("ball");
            ball1Text = Content.Load<Texture2D>("ball1");
            gameOverText = Content.Load<Texture2D>("gameover");

            //Inicializa as bolas nas suas posições:
            ballPos = new Vector2(GraphicsDevice.Viewport.Width / 2 - ballText.Width / 2, 0);
            ball1Pos = new Vector2(GraphicsDevice.Viewport.Width / 2 - ball1Text.Width / 2, 0);

            //Velocidade das bolas:
            ballXSpeed = 2; ballYSpeed = 2;
            ball1XSpeed = 2; ball1YSpeed = 2;

            //Posição da imagem de GameOver:
            gameOverPos = new Vector2(GraphicsDevice.Viewport.Width / 2 - gameOverText.Width / 2, GraphicsDevice.Viewport.Height / 2 - gameOverText.Height / 2);

            //Inicializa sempre os scores a 0:
            score = 0;
            scorefinal = 0; 

            segundabola = false;
           
            base.Initialize();
        }





        protected override void LoadContent()
        {
            //Load de tudo o que precisamos que apareça no jogo:
            LoadContentMenu(Content, new Size(800, 600));
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundText = Content.Load<Texture2D>("background");
            jogador1 = new TopBall.personagem(Content.Load<Texture2D>("HeadPlayer2"), new Vector2(700, 500));
            jogador2 = new TopBall.personagem(Content.Load<Texture2D>("HeadPlayer1"), new Vector2(0, 500));
            musica = Content.Load<Song>("stadiumSound");
            font = Content.Load<SpriteFont>("gameFont");
        }


        public void LoadContentMenu(ContentManager content, Size windowSize)
        {
            for (int i = 0; i < menus.Count; i++)
            {
                foreach (GUIElement button in menus[i])
                {
                    button.LoadContent(content);
                    button.CenterElement(windowSize);
                }
                //Funções que permitem mover os objetos conforme as coordenadas:
                menus[0].Find(x => x.ElementName == "menu").MoveElement(0, -25);
                menus[0].Find(x => x.ElementName == "play").MoveElement(0, 25);
                menus[1].Find(x => x.ElementName == "gameover").MoveElement(0, -100);
                menus[1].Find(x => x.ElementName == "restart").MoveElement(0, 100);

            }
        }




        //Através desta função, sabendo qual o objeto que "sofre" o clique, altera-se o game state.
        public void OnClick(string element)
        {
            if (element == "play") //Começar a jogar.
            {
                gameState = GameState.inGame;
            }
            if(element == "restart") //Começar de novo.
            {
                gameState = GameState.inGame;
            }
        }




    protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }



        //Colisões, áudio, input... (update)
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Para cada game state, diferentes funções a serem corridas:
            switch (gameState)
            {
                case GameState.mainMenu:
                    foreach (GUIElement button in menus[0]) //MainMenu
                    {
                        button.Update();
                    }
                    break;




                case GameState.inGame:
                    scorefinal = score;
                    jogador1.Update(gameTime);
                    jogador2.Update(gameTime);

                    if(MediaPlayer.State != MediaState.Playing)
                    MediaPlayer.Play(musica);
                    


                    /*--------Controlos do JOGADOR 1:--------*/
                    if (Keyboard.GetState().IsKeyDown(Keys.Right)) jogador1.velocity.X = 10f;
                    else if (Keyboard.GetState().IsKeyDown(Keys.Left)) jogador1.velocity.X = -10f;
                    else jogador1.velocity.X = 0;

                    //Função que permite ao jogador 1 saltar:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) && hasjumped == false)
                    {
                        jogador1.position.Y -= 10f;
                        jogador1.velocity.Y = -5f;
                        hasjumped = true;
                    }

                    //Saltar:
                    if (hasjumped == true)
                    {
                        float i = 1;
                        jogador1.velocity.Y += 0.35f * i;
                    }

                    //Verificar se o jogador saltou ou não.
                    if (jogador1.position.Y + jogador1.texture.Height >= 450)
                       hasjumped = false;

                    if (hasjumped == false)
                        jogador1.velocity.Y = 0f;
                    /*-----------------------------------------------------*/     




                    /*--------Controlos do JOGADOR 2:--------*/
                    if (Keyboard.GetState().IsKeyDown(Keys.D)) jogador2.velocity.X = 10f;
                    else if (Keyboard.GetState().IsKeyDown(Keys.A)) jogador2.velocity.X = -10f;
                    else jogador2.velocity.X = 0;
                    if (Keyboard.GetState().IsKeyDown(Keys.Space) && hasjumped1 == false)
                    {
                        jogador2.position.Y -= 10f;
                        jogador2.velocity.Y = -5f;
                        hasjumped1 = true;
                    }

                    if (hasjumped1 == true)
                    {
                        float i = 1;
                        jogador2.velocity.Y += 0.35f * i;
                    }

                    if (jogador2.position.Y + jogador2.texture.Height >= 450)
                       hasjumped1 = false;

                    if (hasjumped1 == false)
                        jogador2.velocity.Y = 0f;
                    /*--------------------------------------------------*/



                    //Funções que não permitem que os objetos saiam do ecrã:
                    if (!isGameOver)
                    {
                        ballPos.X += ballXSpeed;
                        ballPos.Y += ballYSpeed;

                        if (segundabola == true)
                        {
                            ball1Pos.X += ball1XSpeed;
                            ball1Pos.Y += ball1YSpeed;
                        }

                        int maxX = GraphicsDevice.Viewport.Width - ballText.Width;
                        int maxY = GraphicsDevice.Viewport.Height - ballText.Height;
                        int maxX1 = GraphicsDevice.Viewport.Width - ball1Text.Width;
                        int maxY1 = GraphicsDevice.Viewport.Height - ball1Text.Height;


                        if (ballPos.X >= maxX || ballPos.X <= 0)
                        {
                            ballXSpeed = -ballXSpeed;
                            if (ballXSpeed <= -4)
                            {
                                segundabola = true;
                                ballXSpeed = 4;
                            }
                            if (ballXSpeed >= 4)
                            {
                                ballXSpeed = -4;
                            }
                        }

                        if (ballPos.Y >= maxY || ballPos.Y <= 0)
                        {
                            ballYSpeed = -ballYSpeed;
                            if (ballYSpeed >= 4)
                            {
                                ballYSpeed = -4;
                            }
                            if (ballYSpeed >= -4)
                            {
                                ballYSpeed = 4;
                            }
                        }

                        if (ball1Pos.X >= maxX || ball1Pos.X <= 0)
                        {
                            ball1XSpeed = -ball1XSpeed;
                            if (ball1XSpeed <= -4)
                            {                           
                                ball1XSpeed = 4;
                            }
                            if (ball1XSpeed >= 4)
                            {
                                ball1XSpeed = -4;
                            }
                        }

                        if (ball1Pos.Y > maxY1 || ball1Pos.Y < 0)
                        {
                            ball1YSpeed = -ball1YSpeed;
                            if (ball1YSpeed >= 4)
                            {
                                ball1YSpeed = -4;
                            }
                            if (ball1YSpeed >= -4)
                            {
                                ball1YSpeed = 4;
                            }
                        }

                        jogador1.position.X = MathHelper.Clamp(jogador1.position.X, 0, GraphicsDevice.Viewport.Width - jogador1.texture.Width);
                        jogador1.position.Y = MathHelper.Clamp(jogador1.position.Y, 0, GraphicsDevice.Viewport.Height - jogador1.texture.Height);

                        jogador2.position.X = MathHelper.Clamp(jogador2.position.X, 0, GraphicsDevice.Viewport.Width - jogador2.texture.Width);
                        jogador2.position.Y = MathHelper.Clamp(jogador2.position.Y, 0, GraphicsDevice.Viewport.Height - jogador2.texture.Height);


                        /*--------COLISÕES--------*/

                        //Colisão do jogador 1 com a bola 1, alteração da velocidade da bola e update do score:
                        if (ballPos.Y >= jogador1.position.Y - ballText.Width)
                        {
                            if (ballPos.X + ballText.Width >= jogador1.position.X & ballPos.X <= jogador1.position.X + jogador1.texture.Width)
                            {
                                ballYSpeed = -ballYSpeed - 1f;
                                ballXSpeed = ballXSpeed + 1f;
                                score += 1;
                            }
                        }

                        //Quando a bola 1 toca no chão o game state muda para GameOver:
                        if (ballPos.Y >= GraphicsDevice.Viewport.Height - ballText.Width)
                            gameState = GameState.GameOver;


                        //Colisão do jogador 2 com a bola 1, alteração da velocidade da bola 2 e update do score:
                        if (ballPos.Y >= jogador2.position.Y - ballText.Width)
                        {
                            if (ballPos.X + ballText.Width >= jogador2.position.X & ballPos.X <= jogador2.position.X + jogador2.texture.Width)
                            {
                                {
                                    ballYSpeed = -ballYSpeed - 1f;
                                    ballXSpeed = ballXSpeed + 1f;
                                    score += 1;
                                }
                            }
                        }
                        if (score == 5)
                        {
                            segundabola = true;
                        }



                        //Colisão do jogador 1 com a bola 2, alteração da velocidade da bola e update do score:
                        if (segundabola == true)
                        {
                            if (ball1Pos.Y >= jogador1.position.Y - ball1Text.Width)
                            {
                                if (ball1Pos.X + ball1Text.Width >= jogador1.position.X & ball1Pos.X <= jogador1.position.X + jogador1.texture.Width)
                                {
                                    ball1YSpeed = -ball1YSpeed - 1f;
                                    ball1XSpeed = ball1XSpeed + 1f;
                                    score += 1;
                                }
                            }

                            //Colisão do jogador 2 com a bola 2, alteração da velocidade da bola 2 e update do score:
                            if (ball1Pos.Y >= jogador2.position.Y - ball1Text.Width)
                            {
                                if (ball1Pos.X + ball1Text.Width >= jogador2.position.X & ball1Pos.X <= jogador2.position.X + jogador2.texture.Width)
                                {
                                    {
                                        ball1YSpeed = -ball1YSpeed - 1f;
                                        ball1XSpeed = ball1XSpeed + 1f;
                                        score += 1;
                                    }
                                }
                            }

                            //Quando a segunda bola 2 toca no chão o game state muda para GameOver:
                            if (ball1Pos.Y >= GraphicsDevice.Viewport.Height - ball1Text.Width)
                                gameState = GameState.GameOver;
                        }                      
                    }
                    break;


                case GameState.GameOver:
                    foreach (GUIElement button in menus[1]) 
                    {
                        MediaPlayer.Stop();
                        score = 0;
                        ballXSpeed = 2; ballYSpeed = 2;
                        ball1XSpeed = 2; ball1YSpeed = 2;
                        ballPos = new Vector2(GraphicsDevice.Viewport.Width / 2 - ballText.Width / 2, 0);
                        ball1Pos = new Vector2(GraphicsDevice.Viewport.Width / 2 - ball1Text.Width / 2, 0);
                        
                        jogador1 = new TopBall.personagem(Content.Load<Texture2D>("HeadPlayer2"), new Vector2(700, 500));
                        jogador2 = new TopBall.personagem(Content.Load<Texture2D>("HeadPlayer1"), new Vector2(0, 500));
                        segundabola = false;
                        button.Update();
                    }
                    break;
            }
            base.Update(gameTime);
        }


        //Faz com que os elementos apareçam no ecrã no posição escolhida:
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.mainMenu:
                    foreach (GUIElement button in menus[0]) //MainMenu
                    {
                        if (!isGameOver)
                        {                          
                            button.Draw(spriteBatch);
                        }
                    }
                    break;


                case GameState.inGame:
                    spriteBatch.Draw(backgroundText,new Vector2 (0,0),Color.White);
                    spriteBatch.Draw(ballText, ballPos, null);
                    jogador1.Draw(spriteBatch);
                    jogador2.Draw(spriteBatch);
                    spriteBatch.DrawString(font, "Score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 10, GraphicsDevice.Viewport.TitleSafeArea.Y + 10), Color.White);
                    if (segundabola == true)
                    {
                        spriteBatch.Draw(ball1Text, ball1Pos, null);
                    }
                    break;


                case GameState.GameOver:
                    foreach (GUIElement button in menus[1])
                    {
                        spriteBatch.DrawString(font, "Final Score : " + scorefinal, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 10, GraphicsDevice.Viewport.TitleSafeArea.Y + 10), Color.White);
                        button.Draw(spriteBatch);
                    }
                    break;
            }
                    spriteBatch.End();
                    base.Draw(gameTime);
            }
        }
    }


