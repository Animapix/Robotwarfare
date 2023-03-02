using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Services
{
    public interface AssetsService
    {
        public T GetAsset<T>(string name);
    }

    public class AssetsManager : AssetsService
    {
        private Dictionary<String, object> _assets = new Dictionary<string, object>();

        public AssetsManager()
        {
            ServiceLocator.RegisterService<AssetsManager>(this);

            Texture2D texture = new Texture2D(ServiceLocator.GetService<SpriteBatch>().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData(new[] { Color.White });
            _assets["OnePixel"] = texture;
        }

        public void LoadAsset<T>(string name)
        {
            ContentManager content = ServiceLocator.GetService<ContentManager>();
            T asset = content.Load<T>(name);
            this._assets[name] = asset;
        }

        public T GetAsset<T>(string name)
        {
            return (T)_assets[name];
        }
    }
}
