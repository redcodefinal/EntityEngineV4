using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public interface IComponent
    {
        string Name { get; }

        int Id { get; }

        bool Active { get; }

        bool Visible { get; }

        void Update(GameTime gt);

        void Draw(SpriteBatch sb);

        void Destroy(IComponent i = null);
    }
}