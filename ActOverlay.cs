using System;
using System.Collections.Generic;
using System.Text;

using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace CPUTempOverlay
{
	public class ActOverlay : IDisposable
	{
		private readonly GraphicsWindow _window;
        private readonly StickyWindow _stickwindow;
        private readonly Graphics _graphics;

        private readonly Dictionary<string, SolidBrush> _brushes;
		private readonly Dictionary<string, Font> _fonts;
		private readonly Dictionary<string, Image> _images;

		public string CPUInfoText = "";
		public IntPtr iptr = IntPtr.Zero;

		public ActOverlay( int posx, int posy )
		{
			_brushes = new Dictionary<string, SolidBrush>();
			_fonts = new Dictionary<string, Font>();
			_images = new Dictionary<string, Image>();

            _graphics = new Graphics()
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                UseMultiThreadedFactories = false,
                VSync = true,
                WindowHandle = IntPtr.Zero
            };
            _window = new GraphicsWindow(posx, posy, 1600, 800, _graphics)
			{
				FPS = 60,
				IsTopmost = true,
				IsVisible = true
            };

			_window.DestroyGraphics += _window_DestroyGraphics;
			_window.DrawGraphics += _window_DrawGraphics;
			_window.SetupGraphics += _window_SetupGraphics;
		}

		private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
		{
			var gfx = e.Graphics;

			if (e.RecreateResources)
			{
				// 塗りつぶし色を再定義するためにいったん全消し
				foreach (var pair in _brushes) pair.Value.Dispose();
				foreach (var pair in _images) pair.Value.Dispose();
			}

			// 塗りつぶし色の再定義
			_brushes["black"] = gfx.CreateSolidBrush(0, 0, 0);
			_brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
			_brushes["red"] = gfx.CreateSolidBrush(255, 0, 0);
			_brushes["green"] = gfx.CreateSolidBrush(0, 255, 0);
			_brushes["blue"] = gfx.CreateSolidBrush(0, 0, 255);
			_brushes["background"] = gfx.CreateSolidBrush(0x33, 0x36, 0x3F);
			_brushes["grid"] = gfx.CreateSolidBrush(255, 255, 255, 0.2f);
			_brushes["random"] = gfx.CreateSolidBrush(0, 0, 0);
			_brushes["none"] = gfx.CreateSolidBrush(0, 0, 0, 0);

            // 塗りつぶし色再定義が目的ならここで抜ける
            if (e.RecreateResources) return;

			// フォントの定義
			_fonts["arial"] = gfx.CreateFont("Arial", 12);
			_fonts["consolas"] = gfx.CreateFont("Consolas", 14);
            _fonts["rog fonts"] = gfx.CreateFont("ROG FONTS", 48);

		}

		private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
		{
			foreach (var pair in _brushes) pair.Value.Dispose();
			foreach (var pair in _fonts) pair.Value.Dispose();
			foreach (var pair in _images) pair.Value.Dispose();
		}

		private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
		{
			var gfx = e.Graphics;

			var padding = 16;
			var infoText = new StringBuilder()
				.Append("FPS: ").Append(gfx.FPS.ToString().PadRight(padding)).Append("\n")
				.Append(CPUInfoText)
				.ToString();

			gfx.ClearScene();
			//gfx.ClearScene(_brushes["background"]);

			gfx.DrawText(_fonts["rog fonts"], _brushes["green"], 58, 20, infoText);

		}

		public void Run()
		{
            _window.PlaceAbove(iptr);
            _window.Create();
            _window.Join();
		}

		~ActOverlay()
		{
			Dispose(false);
		}

		#region IDisposable Support
		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				_window.Dispose();

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
