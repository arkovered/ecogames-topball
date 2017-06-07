using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopBall
{
   public class personagem
    {
        /*--------Declaração de Variáveis--------*/
        public Texture2D texture;
        public Vector2 position;
        public  Vector2 velocity;
     

        public personagem(Texture2D newTexture, Vector2 newPosition)
        {
            texture = newTexture;
            position = newPosition;
        }

        public void Update(GameTime gameTime)
        {
            position += velocity;            
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position);
         
        }

    }
}

