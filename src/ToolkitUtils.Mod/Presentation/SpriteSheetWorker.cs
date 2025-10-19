// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace ToolkitUtils.Mod.Presentation;

/// <summary>A specialized worker for animating frames from a sprite sheet.</summary>
/// <param name="frames">The individual frames of the animation sprite sheet.</param>
/// <remarks>
///     This class does not display (i.e., draw) the current frame of the animation to the screen; it's the view
///     layer's job to draw the current frame.
/// </remarks>
public class SpriteSheetWorker(IReadOnlyList<Texture2D> frames)
{
    private int _index;
    private Timer? _timer;

    /// <summary>The total number of frames that exist within the sprite sheet.</summary>
    public int TotalFrames => frames.Count;

    /// <summary>Returns the current frame of the sprite sheet.</summary>
    public Texture CurrentFrame => frames[_index];

    /// <summary>Whether the worker's internal <see cref="Timer" /> is currently running.</summary>
    public bool Running { get; private set; }

    /// <summary>Starts the worker's internal <see cref="Timer" />.</summary>
    /// <param name="milliseconds">The amount of milliseconds to wait between frame changes.</param>
    public void Start(int milliseconds)
    {
        ChangeTimer(milliseconds);
    }

    /// <summary>Stops the worker's internal <see cref="Timer" />.</summary>
    public void Stop()
    {
        ChangeTimer(Timeout.Infinite);
    }

    /// <summary>Sets the current frame being displayed.</summary>
    /// <param name="frame">The current frame to display.</param>
    /// <remarks>The <see cref="frame" /> parameter should be between 1 and <see cref="TotalFrames" />.</remarks>
    public void SetFrame(int frame)
    {
        _index = (frame - 1) % frames.Count;

        ChangeTimer(Timeout.Infinite);
    }

    /// <summary>Moves to the next frame of the sprite sheet.</summary>
    public void NextFrame()
    {
        _index = (_index + 1) % frames.Count;
    }

    /// <summary>Moves to the last frame of the sprite sheet.</summary>
    public void ToLastFrame()
    {
        SetFrame(frames.Count);
    }

    /// <summary>Moves to the first frame of the sprite sheet.</summary>
    public void ToFirstFrame()
    {
        SetFrame(1);
    }

    private void ChangeTimer(int period)
    {
        _timer ??= new Timer(
            callback: w =>
            {
                if (w is SpriteSheetWorker worker) worker.NextFrame();
            },
            this,
            Timeout.Infinite,
            Timeout.Infinite
        );

        Running = period != Timeout.Infinite && _timer?.Change(dueTime: 0, period) == true;
    }

    /// <summary>Creates a new instance of a <see cref="SpriteSheetWorker" /> from a sprite sheet image.</summary>
    /// <param name="texture">The sprite sheet image to create an animation from.</param>
    /// <remarks>
    ///     This method extracts frames from the sprite sheet either left to right, or top to bottom, depending on the
    ///     orientation of the image. Extrapolation does not support images that contain multiple rows if the image is
    ///     horizontally larger, or columns if the image is vertically larger. Sprite sheets must be one, continuous strip with
    ///     each frame immediately after the previous.
    /// </remarks>
    public static SpriteSheetWorker CreateInstance(Texture2D texture)
    {
        Texture2D[] extrapolateFrames = ExtrapolateFrames(texture);

        return new SpriteSheetWorker(extrapolateFrames);
    }

    /// <summary>Creates a new instance of a <see cref="SpriteSheetWorker" /> with an array of frames.</summary>
    /// <param name="frames">The individual frames that compose a sprite sheet animation.</param>
    public static SpriteSheetWorker CreateInstance(Texture2D[] frames) => new(frames);

    private static Texture2D CopyImage(Texture original)
    {
        RenderTexture tmpTexture = RenderTexture.GetTemporary(original.width, original.height);

        Graphics.Blit(original, tmpTexture);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmpTexture;

        var newTexture = new Texture2D(original.width, original.height, tmpTexture.graphicsFormat, TextureCreationFlags.None);

        newTexture.ReadPixels(new Rect(x: 0f, y: 0f, tmpTexture.width, tmpTexture.height), destX: 0, destY: 0);
        newTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmpTexture);

        return newTexture;
    }

    private static Texture2D[] ExtrapolateFrames(Texture2D texture)
    {
        Texture2D copy = CopyImage(texture);

        int imageSize = Mathf.Min(copy.width, copy.height);
        bool isVertical = copy.height > copy.width;
        int totalFrames = Mathf.Max(copy.width, copy.height) / imageSize;
        var container = new Texture2D[totalFrames];

        for (var i = 0; i < totalFrames; i++)
        {
            var frame = new Texture2D(imageSize, imageSize, texture.format, mipChain: false, linear: true);
            int x = isVertical ? 0 : i * imageSize;
            int y = isVertical ? i * imageSize : 0;

            frame.SetPixels(copy.GetPixels(x, y, imageSize, imageSize));
            frame.Apply();

            container[i] = frame;
        }

        return container;
    }
}
