using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Services;

namespace Robotwarfare
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private AssetsManager _assetsManager;
        private SceneManager _sceneManager;
        private InputsManager _inputsManager;

        public static Rectangle canvas = new Rectangle(0, 0, 1920, 960);
        private RenderTarget2D renderTarget;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            ServiceLocator.RegisterService(Content);

            _inputsManager = new InputsManager();
            ServiceLocator.RegisterService((InputsService)_inputsManager);

            IsMouseVisible = true;
            Window.AllowUserResizing = true;            
        }

        protected override void Initialize()
        {
            renderTarget = new RenderTarget2D(GraphicsDevice, canvas.Width, canvas.Height);

            _graphics.PreferredBackBufferWidth = canvas.Width;
            _graphics.PreferredBackBufferHeight = canvas.Height;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Register services
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ServiceLocator.RegisterService(_spriteBatch);

            _assetsManager = new AssetsManager();
            ServiceLocator.RegisterService((AssetsService)_assetsManager);

            _sceneManager = new SceneManager();
            ServiceLocator.RegisterService((SceneManager)_sceneManager);

            // Load Assets
            _assetsManager.LoadAsset<Texture2D>("MonoGame_Logo");
            _assetsManager.LoadAsset<Texture2D>("TileSet");
            _assetsManager.LoadAsset<Texture2D>("SelectionCursor");
            _assetsManager.LoadAsset<SpriteFont>("DefaultFont");

            // Load Scenes
            _sceneManager.Register("Game", new GameScene());
            _sceneManager.Load("Game");
        }

        protected override void Update(GameTime gameTime)
        {
            _sceneManager.Update(gameTime);
            _inputsManager.UpdateState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(new Color(100,100,100));

            _spriteBatch.Begin();
            _sceneManager.Draw();
            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);


            // -----------------------------------------------------------------------[ Draw Canvas ]-----------------------------------------------------------------------
            float ratio = 1;
            int marginV = 0;
            int marginH = 0;
            float currentAspect = Window.ClientBounds.Width / (float)Window.ClientBounds.Height;
            float virtualAspect = (float)canvas.Width / (float)canvas.Height;

            if (canvas.Height != this.Window.ClientBounds.Height)
            {
                if (currentAspect > virtualAspect)
                {
                    ratio = Window.ClientBounds.Height / (float)canvas.Height;
                    marginH = (int)((Window.ClientBounds.Width - canvas.Width * ratio) / 2);
                }
                else
                {
                    ratio = Window.ClientBounds.Width / (float)canvas.Width;
                    marginV = (int)((Window.ClientBounds.Height - canvas.Height * ratio) / 2);
                }
            }

            Rectangle dst = new Rectangle(marginH, marginV, (int)(canvas.Width * ratio), (int)(canvas.Height * ratio));

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            _spriteBatch.Draw(renderTarget, dst, Color.White);
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}